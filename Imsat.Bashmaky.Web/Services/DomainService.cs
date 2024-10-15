using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace Imsat.Bashmaky.Web.Services
{
    public class DomainService
    {
        private readonly AppDbContext _context;
        public static event Action<BaseSignal> SignalEvent;
        public DomainService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        #region add
        public async Task AddEntity<T>(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task AddTerminalAsync(SignalTerminal signal)
        {
            var terminal = _context.Terminals.FirstOrDefault(x => x.Imei == signal.Imei);
            if (terminal == null)
            {
                terminal = new Terminal { Imei = signal.Imei };
                await _context.AddAsync(terminal);
            }
            FillFields(signal, terminal);
            await _context.SaveChangesAsync();

            void FillFields(SignalTerminal signal, Terminal terminal)
            {
                terminal.LON = signal.LON;
                terminal.LAT = signal.LAT;
                terminal.NET = signal.NET;
                terminal.RSSI = signal.RSSI;
                terminal.VDD = signal.VDD;
                terminal.TS = signal.TS;
                terminal.StationId = signal.StationId;
            }
        }
        public async Task AddBoxAsync(SignalBox signal)
        {
            var box = _context.Boxes.FirstOrDefault(x => x.Mac == signal.Mac);
            if (box == null)
            {
                box = new Box { Mac = signal.Mac };
                await _context.AddAsync(box);
            }
            FillFields(signal, box);
            await _context.SaveChangesAsync();

            void FillFields(SignalBox signal, Box box)
            {
                box.LON = signal.LON;
                box.LAT = signal.LAT;
                box.Imei = signal.Imei;
                box.VDD = signal.VDD;
                box.TS = signal.TS;
                box.St = signal.St;
                box.StationId = signal.StationId;
            }
        }
        public async Task AddBashmakAsync(SignalBashmak signal)
        {
            var bashmak = _context.Bashmaks.FirstOrDefault(x => x.Mac == signal.Mac);
            if (bashmak == null)
            {
                bashmak = new Bashmak { Mac = signal.Mac };
                await _context.AddAsync(bashmak);
            }
            FillFields(signal, bashmak);
            await _context.SaveChangesAsync();

            void FillFields(SignalBashmak signal, Bashmak bashmak)
            {
                bashmak.LON = signal.LON;
                bashmak.LAT = signal.LAT;
                bashmak.Imei = signal.Imei;
                bashmak.VDD = signal.VDD;
                bashmak.TS = signal.TS;
                bashmak.St = signal.St;
                bashmak.StationId = signal.StationId;
            }
        }
        public async Task AddSignalTerminalAsync(string json)
        {
            var signal = JsonConvert.DeserializeObject<SignalTerminal>(json);
            await AddSignalTerminalAsync(signal);
        }

        public async Task AddSignalTerminalAsync(SignalTerminal? signal)
        {
            if (signal is null)
                return;
            await _context.AddAsync(signal);
            await _context.SaveChangesAsync();
            await AddTerminalAsync(signal);
            SignalEvent?.Invoke(signal);
        }

        public async Task AddSignalBashmakAsync(string json)
        {
            var signal = JsonConvert.DeserializeObject<SignalBashmak>(json);
            await AddSignalBashmakAsync(signal);
        }

        public async Task AddSignalBashmakAsync(SignalBashmak? signal)
        {
            if(signal is null) return;  
            await _context.AddAsync(signal);
            await _context.SaveChangesAsync();
            await AddBashmakAsync(signal);
            SignalEvent?.Invoke(signal);
        }

        public async Task AddSignalBoxAsync(string json)
        {
            var signal = JsonConvert.DeserializeObject<SignalBox>(json);
            await AddSignalBoxAsync(signal);
        }

        public async Task AddSignalBoxAsync(SignalBox? signal)
        {
            if (signal is null) return;
            await _context.AddAsync(signal);
            await _context.SaveChangesAsync();
            await AddBoxAsync(signal);
            SignalEvent?.Invoke(signal);
        }
        #endregion

        #region get


        public IEnumerable<T> GetEntities<T>() where T : class
        {
            return _context.Set<T>();
        }
        public DbSet<T> GetDbSet<T>() where T : class
        {
            return _context.Set<T>();
        }

        public Task<IEnumerable<BaseSignal>> GetSignals()
        {
            return Task.Run(() => GetEntities<BaseEntity>().Where(x => x is BaseSignal).Select(x => x as BaseSignal));
        }
        public Task<IEnumerable<BaseDevice>> GetDevices()
        {
            return Task.Run(() => GetEntities<BaseEntity>().Where(x => x is BaseDevice).Select(x => x as BaseDevice));
        }
        #endregion
    }
}
