using Imsat.Bashmaky.Model.Database;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Bashmaky.Model.Database.Types;
using Imsat.Bashmaky.Web.Pages;
using Microsoft.EntityFrameworkCore;

namespace Imsat.Bashmaky.Web.Services
{
    public class BoxCompletionService : BackgroundService
    {
        private readonly Func<DomainService> _dbContextFactory;
        private readonly ILogger<BoxCompletionService> _logger;

        public BoxCompletionService(Func<DomainService> dbContextFactory, ILogger<BoxCompletionService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DomainService.SignalEvent += CheckBashmakeState;
            return Task.CompletedTask;
        }
        private async void CheckBashmakeState(BaseSignal signal)
        {
            if (signal is SignalBashmak bashmakSignal)
            {
                var domainService = _dbContextFactory.Invoke();

                switch (bashmakSignal.St)
                {
                    case ST.Raised:
                        HandleRaisedSignal(bashmakSignal, domainService);
                        break;
                    case ST.PutInBox:
                        HandlePutInSignal(bashmakSignal, domainService);
                        break;
                    case ST.PutOnRail:
                        HandlePutOnRailSignal(bashmakSignal, domainService);
                        break;

                }

                await domainService.SaveAsync();
            }

        }


        private void HandlePutOnRailSignal(SignalBashmak bashmakSignal, DomainService domainService)
        {
            var bashmak = domainService.GetDbSet<Bashmak>().Include(x => x.Box).First(x => x.Mac == bashmakSignal.Mac);
            bashmak.St = bashmakSignal.St;
        }
        private void HandlePutInSignal(SignalBashmak bashmakSignal, DomainService domainService)
        {
            var bashmak = domainService.GetDbSet<Bashmak>().Include(x => x.Box).First(x => x.Mac == bashmakSignal.Mac);
            bashmak.St = bashmakSignal.St;
            if (bashmak.Box != null)
            {
                if (bashmak.Box.St != ST.BoxOpened)
                    _logger.LogWarning($"При сигнале о помещении башмака {bashmakSignal.Mac} в ящик соответствующий ящик не был открыт");

                return;
            }
            else
            {
                var box = FindBoxByCoordinate(domainService, bashmakSignal);
                if (box == null)
                {
                    _logger.LogWarning($"При сигнале о помещении башмака {bashmakSignal.Mac} в ящик не обнаружен ни один открытый ящик");
                    return;
                }
                if (!box.Bashmaks.Contains(bashmak))
                    box.Bashmaks.Add(bashmak);
                bashmak.Box = box;
            }
        }
        private void HandleRaisedSignal(SignalBashmak bashmakSignal, DomainService domainService)
        {
            var bashmak = domainService.GetDbSet<Bashmak>().Include(x => x.Box).First(x => x.Mac == bashmakSignal.Mac);
            bashmak.St = bashmakSignal.St;
            if (bashmak.Box != null)
            {
                if (bashmak.Box.St != ST.BoxOpened)
                {
                    _logger.LogWarning($"Ящик {bashmak.Box.Mac} не открыт при поднятии принадлежащего ему башмака {bashmak.Mac}");
                }
                //bashmak.Box.Bashmaks.Remove(bashmak);
                //bashmak.Box = null;
            }
            else
            {
                var box = FindBoxByCoordinate(domainService, bashmakSignal);
                if (box == null)
                    _logger.LogWarning($"При сигнале о поднятии башмака {bashmakSignal.Mac} не обнаружен ни один открытый ящик");
                else
                {
                    //box.Bashmaks.Remove(bashmak);
                }
            }

        }

        private Box? FindBoxByCoordinate(DomainService domainService, SignalBashmak signal)
        {
            var boxes = domainService.GetEntities<Box>().Where(x => x.St == ST.BoxOpened);
            if (!boxes.Any())
            {
                return null;
            }
            return FindNearestPoint(boxes, signal);
        }

        private Box FindNearestPoint(IEnumerable<Box> boxes, SignalBashmak signal)
        {
            double minDistance = double.MaxValue;
            Box nearestBox = null;

            foreach (var box in boxes)
            {
                double distance = HaversineDistance(signal, box);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestBox = box;
                }
            }

            return nearestBox;
        }

        private double HaversineDistance(SignalBashmak signal, Box box)
        {
            const double R = 6371; // Earth radius in kilometers

            double dLat = DegreeToRadian(box.LAT - signal.LAT);
            double dLon = DegreeToRadian(box.LAT - signal.LAT);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreeToRadian(signal.LAT)) * Math.Cos(DegreeToRadian(box.LAT)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;

            double DegreeToRadian(double degree)
            {
                return degree * Math.PI / 180.0;
            }
        }
    }
}

