using Imsat.Bashmaky.Model.Database;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Bashmaky.Model.Database.Types;
using Imsat.Bashmaky.Model.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Timers;

namespace Imsat.Bashmaky.Web.Services
{
    public class ConnectionCheckerService : BackgroundService
    {
        private System.Timers.Timer _aTimer;
        private readonly Func<DomainService> _dbContextFactory;

        private readonly int _maxDelayTime;
        private readonly int _checkConnectionTime;
        public ConnectionCheckerService(Func<DomainService> dbContextFactory, IOptions<AppConfig> options)
        {
            _dbContextFactory = dbContextFactory;
            _maxDelayTime = options.Value.MaxConnectionDelayTimeMin;
            _checkConnectionTime = options.Value.CheckConnectionTimeMin;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetTimer();
            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_aTimer != null)
            {
                _aTimer.Stop();
                _aTimer.Dispose();
            }
            return Task.CompletedTask;
        }
        private void SetTimer()
        {
            _aTimer = new System.Timers.Timer(TimeSpan.FromMinutes(_checkConnectionTime));
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.AutoReset = true;
            _aTimer.Enabled = true;
        }

        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var domain = _dbContextFactory.Invoke();
            var timeNow = DateTime.UtcNow;
            var devices = await domain.GetDevices();
            foreach (var dev in devices)
            {
                var delay = timeNow - dev.TS;
                if (delay > TimeSpan.FromMinutes(_maxDelayTime))
                {
                    dev.Connection = Connection.Disconnect;
                }
                else
                {
                    dev.Connection = Connection.Connect;
                }
            }
            await domain.SaveAsync();
        }
    }
}
