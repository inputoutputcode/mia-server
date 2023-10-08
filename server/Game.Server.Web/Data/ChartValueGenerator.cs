using Game.Server.Web.Hubs;

using Microsoft.AspNetCore.SignalR;


namespace Game.Server.Web.Data
{
    public class ChartValueGenerator : BackgroundService
    {
        private readonly IHubContext<ScoreHub> _hub;
        private readonly Buffer<Point> _data;

        public ChartValueGenerator(IHubContext<ScoreHub> hub, Buffer<Point> data)
        {
            _hub = hub;
            _data = data;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hub.Clients.All.SendAsync(
                    "addChartData",
                    _data.AddNewRandomPoint(),
                    cancellationToken: stoppingToken
                );

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
