using Imsat.Bashmaky.Model.Database;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Bashmaky.Model.Settings;
using Imsat.Bashmaky.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Imsat.Bashmaky.Web.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class IndexModel : PageModel
    {
        private readonly DomainService _domainService;
        private readonly AppDbContext _appDbContext;
        private int _shortSignalListCount;
        public IEnumerable<Bashmak> Bashmaks { get; private set; }
        public IEnumerable<Terminal> Terminals { get; private set; }
        public IEnumerable<Box> Boxes { get; private set; }
        public IEnumerable<BaseSignal> Signals { get; private set; }
        public IEnumerable<BaseSignal> ShortSignals { get; private set; }
        public List<Station> Stations { get; private set; }

        public int StationId { get; private set; }
        public string PageUrl { get; private set; }
        public int UpdateTimeSec { get; private set; }

        public IndexModel(DomainService domainService, AppDbContext appDbContext, IOptions<AppConfig> appConfig, ILogger<IndexModel> logger)
        {
            _domainService = domainService;
            _appDbContext = appDbContext;

            _shortSignalListCount = appConfig.Value.ShortSignalListCount;
            UpdateTimeSec = appConfig.Value.PageUpdateTimeSec;
        }


        public void OnGet(int id) //TODO: get info from one source
        {
            this.PageUrl = HttpContext.Request.GetEncodedUrl();
            this.StationId = id;
            Stations = _domainService.GetEntities<Station>().ToList();
            Bashmaks = _appDbContext.Bashmaks.Include(x => x.Railways).Where(x => x.StationId == id);
            Terminals = _appDbContext.Terminals.Where(x => x.StationId == id);
            Boxes = _appDbContext.Boxes.Where(x => x.StationId == id).Include(x => x.Bashmaks);
            Signals = _domainService.GetSignals().Result.Where(x => x.StationId == id);
            ShortSignals = Signals.OrderByDescending(x => x.TS).Take(_shortSignalListCount);
        }
    }
}