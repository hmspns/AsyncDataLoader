## AsyncDataLoader
AsyncDataLoader - tiny library to help load data from many sources with one request in asynchronous way.

### How to use
    AsyncDataLoader<string> loader = new AsyncDataLoader<string>();
    loader.LoadDataPeriod = TimeSpan.FromSeconds(1);
    loader.MinResultsCount = 3;
    loader.EmergencyPeriod = TimeSpan.FromSeconds(1);

    loader.DataLoaders = new Func<string>[]
    {
        new Func<string>
            (() =>
            {
                double sleep = 1.7d;
                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                return $"Loaded in {sleep} second";
            }), 
            (() =>
            {
                double sleep = 1d;
                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                return $"Loaded in {sleep} second";
            }),
            (() =>
            {
                double sleep = 1.25d;
                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                return $"Loaded in {sleep} second";
            }),
            (() =>
            {
                double sleep = 0.01d;
                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                return $"Loaded in {sleep} second";
            }),
    };

    return await loader.GetResultsAsync();