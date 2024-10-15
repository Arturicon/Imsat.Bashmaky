using Imsat.Bashmaky.Model.Database;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imsat.Bashmaky.Web.Services;
using Imsat.Bashmaky.Web;

namespace TestProject
{
    internal class CustomAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbConnStr;

        public CustomAppFactory(string host, int port, string password)
        {
            var sb = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = "test_ci_database",
                Username = "postgres",
                Password = password
            };
            _dbConnStr = sb.ConnectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Удалим зарегистрированный DataContext
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Зарегистрируем снова с указанием на тестовую БД
                services.AddDbContextPool<AppDbContext>(opts => opts.UseNpgsql(_dbConnStr));

                //Обеспечим создание БД
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<AppDbContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                var domainFactory = scopedServices.GetRequiredService<Func<DomainService>>();

                TestAdding(domainFactory);


            });
        }



        private async Task TestAdding(Func<DomainService> domainFactory)
        {
            var sig1 = new SignalTerminal { Imei = "231112190534", TS = DateTime.UtcNow, VDD = 220, NET = "Bee Line GSM", RSSI = -61, LAT = 59.923130f, LON = 30.245016f, StationId = 1 };
            var sig2 = new SignalTerminal { Imei = "388929992919", TS = DateTime.UtcNow, VDD = 220, NET = "Bee Line GSM", RSSI = -61, LAT = 59.923130f, LON = 30.245016f, StationId = 1 };
            var sig3 = new SignalTerminal { Imei = "388929992920", TS = DateTime.UtcNow, VDD = 333, NET = "Bee Line GSM", RSSI = -61, LAT = 59.923130f, LON = 30.245016f, StationId = 1 };
            var sig4 = new SignalBashmak { Mac = "C2-E2-82-AD-B5-56", TS = DateTime.UtcNow, VDD = 220, St = ST.PutOnRail, Imei = "388929992919", LAT = 59.923130f, LON = 30.245016f, StationId = 1, };
            var sig5 = new SignalBashmak { Mac = "C2-E2-82-AD-B5-57", TS = DateTime.UtcNow, VDD = 220, St = ST.PutOnRail, Imei = "388929992919", LAT = 59.923130f, LON = 30.245016f, StationId = 1 };
            var sig6 = new SignalBashmak { Mac = "C2-E2-82-AD-B5-59", TS = DateTime.UtcNow, VDD = 220, St = ST.PutOnRail, Imei = "388929992919", LAT = 59.923130f, LON = 30.245016f, StationId = 1 };
            var sig8 = new SignalBox { Mac = "45-78-92-AA-33-12", TS = DateTime.UtcNow, VDD = 333, St = ST.PutOnRail, Imei = "388929992919", LAT = 59.923130f, LON = 30.245016f, StationId = 1 };
            var sig9 = new SignalBox { Mac = "45-78-92-AA-33-13", TS = DateTime.UtcNow, VDD = 333, St = ST.PutOnRail, Imei = "388929992919", LAT = 59.923130f, LON = 30.245016f, StationId = 1 };
            var sig10 = new SignalBox { Mac = "45-78-92-AA-33-14", TS = DateTime.UtcNow, VDD = 333, St = ST.BoxOpened, Imei = "388929992919", LAT = 19.923130f, LON = 20.245016f, StationId = 1 };
            var sig11 = new SignalBox { Mac = "45-78-92-AA-33-15", TS = DateTime.UtcNow, VDD = 333, St = ST.BoxOpened, Imei = "388929992919", LAT = 50.923130f, LON = 35.245016f, StationId = 1 };
            var sig12 = new SignalBox { Mac = "45-78-92-AA-33-16", TS = DateTime.UtcNow, VDD = 333, St = ST.BoxOpened, Imei = "388929992919", LAT = 70.923130f, LON = 60.245016f, StationId = 1 };
            var sig7 = new SignalBashmak { Mac = "C2-E2-82-AD-B5-59", TS = DateTime.UtcNow, VDD = 333, St = ST.PutInBox, Imei = "388929992919", LAT = 70.923130f, LON = 60.245016f, StationId = 1 };

            try
            {
                DomainService domain = domainFactory.Invoke();
                foreach (SignalTerminal i in new List<SignalTerminal>() { sig1, sig2, sig3 })
                    await domain.AddSignalTerminalAsync(i);

                foreach (SignalBox i in new List<SignalBox>() { sig8, sig9, sig10, sig11, sig12 })
                    await domain.AddSignalBoxAsync(i);

                foreach (SignalBashmak i in new List<SignalBashmak>() { sig4, sig5, sig6, sig7 })
                    await domain.AddSignalBashmakAsync(i);


                domain = domainFactory.Invoke();
                var railways = domain.GetEntities<Railway>().ToList();
                var bashmaks = domain.GetEntities<Bashmak>().ToList();
                railways[0].Bashmaks.AddRange(new Bashmak[] { bashmaks[0], bashmaks[1], bashmaks[2] });
                bashmaks[0].Railways.Add(railways[0]);
                bashmaks[1].Railways.Add(railways[0]);

                railways[1].Bashmaks.AddRange(new Bashmak[] { bashmaks[2] });
                bashmaks[2].Railways.AddRange(new Railway[] { railways[0], railways[1] });
                await domain.SaveAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
