# Quartz.Net-Fluent

### Basic Usage

    var factory = new SchedulerFactory();

    var scheduler = factory.Create()

    var scheduledAsync = scheduler.Schedule(async (info, token) =>
    {
                await Task.Delay(TimeSpan.FromSeconds(30), token);

    }, configuration => configuration.WithInterval(TimeSpan.FromSeconds(1)));

    scheduledAsync.Dispose();

    var scheduledSync = scheduler.Schedule((info, token) =>
    {
                Task.Delay(TimeSpan.FromSeconds(30), token).Wait();

    }, configuration => configuration.WithInterval(TimeSpan.FromSeconds(1)));

    scheduledSync.Dispose();




Quartz.Net Fluent
