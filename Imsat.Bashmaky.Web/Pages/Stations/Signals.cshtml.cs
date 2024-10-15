using Imsat.Bashmaky.Model.Database;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Settings;
using Imsat.Bashmaky.Web.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Imsat.Bashmaky.Web.Pages
{
    public class SignalsModel : PageModel
    {
        private readonly DomainService _domainService;

        public SignalsModel(DomainService domainService, IOptions<AppConfig> appConfig)
        {
            _domainService = domainService;
            UpdateTimeSec = appConfig.Value.PageUpdateTimeSec;
        }
        public int StationId { get; private set; }
        public string PageUrl { get; private set; }
        public int UpdateTimeSec { get; private set; }
        public IEnumerable<BaseSignal> Signals { get; private set; }
        public List<Station> Stations { get; private set; }
        public void OnGet(int id)
        {
            this.PageUrl = HttpContext.Request.GetEncodedUrl();
            this.StationId = id;
            Signals = _domainService.GetSignals().Result.Where(x => x.StationId == id);
            Stations = _domainService.GetEntities<Station>().ToList();
        }
    }
}
