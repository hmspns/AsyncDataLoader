using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gribov.pro
{
    public sealed class AsyncDataLoader<T>
    {
        /// <summary>
        /// Инициализирует новый объек.
        /// </summary>
        public AsyncDataLoader()
        {
            EmergencyPeriod = TimeSpan.Zero;
            MinResultsCount = 0;
        }

        /// <summary>
        /// Инициализирует новый объект.
        /// </summary>
        /// <param name="dataLoaders">Функции, загружающие данные.</param>
        /// <param name="loadDataPeriod">Время, в течении которого данные должны быть загруженны.</param>
        public AsyncDataLoader(IEnumerable<Func<T>> dataLoaders, TimeSpan loadDataPeriod)
            : this(dataLoaders, loadDataPeriod, 0, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Инициализирует новый объект.
        /// </summary>
        /// <param name="dataLoaders">Функции, загружающие данные.</param>
        /// <param name="loadDataPeriod">Время, в течении которого данные должны быть загруженны.</param>
        /// <param name="minimalResultsCount">Минимально необходимое количество загруженных результатов.</param>
        /// <param name="emergencyPeriod">Время, в течении которого будет происходить дозагрузка данных.</param>
        public AsyncDataLoader(IEnumerable<Func<T>> dataLoaders, TimeSpan loadDataPeriod, int minimalResultsCount, TimeSpan emergencyPeriod)
        {
            DataLoaders = dataLoaders;
            LoadDataPeriod = loadDataPeriod;
            EmergencyPeriod = emergencyPeriod;
            MinResultsCount = minimalResultsCount;    
        }

        /// <summary>
        /// Возвращает или устанавливает время, в течении которого будут предприниматься попытки догрузить данные, если получено недостаточное количество результатов.
        /// </summary>
        public TimeSpan EmergencyPeriod
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает или устанавливает минимально допустимое количество результатов.
        /// </summary>
        public int MinResultsCount
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает или устанавливает функции, загружающие данные.
        /// </summary>
        public IEnumerable<Func<T>> DataLoaders
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает или устанавливает время, в течении которого должны быть загруженны данные.
        /// </summary>
        public TimeSpan LoadDataPeriod
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает или устанавливает признак пропуска нулевых результатов.
        /// </summary>
        public bool SkipDefaultResults
        {
            get;
            set;
        }

        /// <summary>
        /// Асинхронно загружает результаты и возвращает их.
        /// </summary>
        /// <returns>Загруженные результаты.</returns>
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
                token = CancellationToken.None; //не будем мешать отладке
#endif
                await Task.Factory.ContinueWhenAll(tasks.ToArray(), (t) =>
                    {
                    }, token);
            }
            catch (TaskCanceledException ex) //не всё успели? ничего страшного.
            {
                isAllCompleted = false;
            }

            if (!isAllCompleted && EmergencyPeriod > TimeSpan.Zero) //надо ли пробовать загружать дальше
            {
                Func<bool> isReadyHandler = () => results.Count >= MinResultsCount; //ок, результатов недостаточно, пытаемся грузить пока есть время.
                await WaitWhenReady(isReadyHandler, EmergencyPeriod);
            }
            if (SkipDefaultResults)
                return results.Where(r => !object.Equals(r, default(T))).ToArray();
            return results.ToArray();
        }

        /// <summary>
        /// Ждёт пока догрузятся данные.
        /// </summary>
        /// <param name="isReadyValidator">Функция, проверяющая, догрузились ли данные.</param>
        /// <param name="totalDelay">Задержка ожидания.</param>
        /// <param name="iterationsCount">Количество итерация для проверки.</param>
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
