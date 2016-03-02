using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gribov.pro.Async
{
    /// <summary>
    /// Data loader by asynchronous way.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AsyncDataLoader<T>
    {
        /// <summary>
        /// Initialize new object.
        /// </summary>
        public AsyncDataLoader()
        {
            EmergencyPeriod = TimeSpan.Zero;
            MinResultsCount = 0;
        }

        /// <summary>
        /// Initialize new object.
        /// </summary>
        /// <param name="dataLoaders">Enumerable of load data functions.</param>
        /// <param name="loadDataPeriod">Load data period.</param>
        public AsyncDataLoader(IEnumerable<Func<T>> dataLoaders, TimeSpan loadDataPeriod)
            : this(dataLoaders, loadDataPeriod, 0, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Initialize new object.
        /// </summary>
        /// <param name="dataLoaders">Enumerable of load data functions.</param>
        /// <param name="loadDataPeriod">Load data period.</param>
        /// <param name="minimalResultsCount">How many results should be loaded at least.</param>
        /// <param name="emergencyPeriod">Additional time to try to load data until <paramref name="minimalResultsCount"/> if not enough results were loaded during <paramref name="loadDataPeriod"/>.</param>
        public AsyncDataLoader(IEnumerable<Func<T>> dataLoaders, TimeSpan loadDataPeriod, int minimalResultsCount, TimeSpan emergencyPeriod)
        {
            DataLoaders = dataLoaders;
            LoadDataPeriod = loadDataPeriod;
            EmergencyPeriod = emergencyPeriod;
            MinResultsCount = minimalResultsCount;    
        }

        /// <summary>
        /// Get or set additional time to try to load data until MinimalResultsCount if not enough results were loaded during LoadDataPeriod.
        /// </summary>
        public TimeSpan EmergencyPeriod
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set count of results that should be loaded at least.
        /// </summary>
        public int MinResultsCount
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set enumerable of load data functions.
        /// </summary>
        public IEnumerable<Func<T>> DataLoaders
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set load data period.
        /// </summary>
        public TimeSpan LoadDataPeriod
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set boolean value to check should be null results skipped or not.
        /// </summary>
        public bool SkipDefaultResults
        {
            get;
            set;
        }

        /// <summary>
        /// Load data asynchronously.
        /// </summary>
        /// <returns>Loaded results.</returns>
        public async Task<T[]> GetResultsAsync()
        {
            BlockingCollection<T> results = new BlockingCollection<T>();
            List<Task> tasks = new List<Task>();

            tasks.AddRange(DataLoaders.Select(handler => Task.Factory.StartNew(() =>
            {
                T result = handler.Invoke();
                results.Add(result);
            }, CancellationToken.None, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness, TaskScheduler.Default)));

            bool isAllCompleted = true;
            try
            {
                CancellationTokenSource source = new CancellationTokenSource(LoadDataPeriod);
                CancellationToken token = source.Token;
#if DEBUG
                token = CancellationToken.None; //skip cancellation during debugging.
#endif
                await Task.Factory.ContinueWhenAll(tasks.ToArray(), (t) =>
                    {
                    }, token);
            }
            catch (TaskCanceledException ex) //didn't load data until deadline? it's ok.
            {
                isAllCompleted = false;
            }

            if (!isAllCompleted && EmergencyPeriod > TimeSpan.Zero) //check to does we have additional time to try to load results
            {
                Func<bool> isReadyHandler = () => results.Count >= MinResultsCount; //we haven't enough results. Try to loading when we have time
                await WaitWhenReady(isReadyHandler, EmergencyPeriod);
            }
            if (SkipDefaultResults)
                return results.Where(r => !object.Equals(r, default(T))).ToArray();
            return results.ToArray();
        }

        /// <summary>
        /// Waiting when data loader.
        /// </summary>
        /// <param name="isReadyValidator">Function to check does data loaded or not.</param>
        /// <param name="totalDelay">Delay between checks.</param>
        /// <param name="iterationsCount">Count of checking iterations.</param>
        private async Task WaitWhenReady(Func<bool> isReadyValidator, TimeSpan totalDelay, int iterationsCount = 7)
        {
            if (isReadyValidator())
                return;

            double milliseconds = totalDelay.TotalMilliseconds / iterationsCount;
            TimeSpan delay = TimeSpan.FromMilliseconds(milliseconds);

            for (int i = 0; i < iterationsCount; i++)
            {
                if (isReadyValidator())
                    return;

                await Task.Delay(delay);
            }
        }
    }
}
