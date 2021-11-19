namespace House.API.CronJobs
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using HLL.Alert.Interfaces;
    using HLL.News.Interfaces;
    using Microsoft.Extensions.Hosting;
    using NCrontab;
    using Serilog;

    public class AutoConsumeNews : BackgroundService
    {
        private readonly CrontabSchedule _schedule;
        private readonly IAutoNewsConsumer _autoNewsConsumer;
        private readonly string _scheduleExp = @"0,15,30,45 * * * *"; //11:59 every week day

        public AutoConsumeNews(IAutoNewsConsumer autoNewsConsumer)
        {
            _schedule = CrontabSchedule.Parse(_scheduleExp, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            _autoNewsConsumer = autoNewsConsumer;
        }

        private DateTime NextOccurrence => _schedule.GetNextOccurrence(DateTime.Now);

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"Stopping {nameof(AutoConsumeNews)}");

            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                if (DateTime.Now > NextOccurrence)
                {
                    Process();
                }
                await Task.Delay(5000, stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private void Process()
        {
            Debug.WriteLine("lol");
        }
    }
}
