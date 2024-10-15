using Imsat.Bashmaky.Model.Database;
using Imsat.Bashmaky.Model.Database.Entities;
using Imsat.Bashmaky.Model.Database.Entities.Devices;
using Imsat.Bashmaky.Model.Database.Types;
using Imsat.Bashmaky.Web.Services;
using Imsat.Bashmaky.Web.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Imsat.Bashmaky.Web.WebApi
{
    [ApiController()]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly DomainService _domainService;

        public EntityController(AppDbContext appDbContext, DomainService domainService)
        {
            _appDbContext = appDbContext;
            _domainService = domainService;
        }

        [HttpGet("[action]/{stationId}")]
        public IEnumerable<Box> Boxes(int stationId)
        {
            var boxes = _appDbContext.Boxes.Where(x => x.StationId == stationId).Include(x => x.Bashmaks).Include(x => x.Station).AsEnumerable();
            return boxes;
        }
        [HttpGet("[action]")]
        public IEnumerable<Station> Stations()
        {
            var stations = _appDbContext.Stations.Include(x => x.Signals).AsEnumerable();
            return stations;
        }

        [HttpGet("[action]")]
        public IEnumerable<TrainAttachmentDto> GetAttachments()
        {
            var attachments = _appDbContext.TrainAttachments.Include(x => x.Bashmakies);
            return attachments.Select(TrainAttachmentDto.Create);
        }

        [HttpPost("[action]")]
        public async Task SaveAttachments([FromBody] IEnumerable<TrainAttachmentDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var entity = await _appDbContext.TrainAttachments.Include(x => x.Bashmakies).FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (entity == null) continue;
                entity.Bashmakies.Clear();
                foreach (var bashmakDto in dto.Bashmakies)
                {
                    var bashmak = await _appDbContext.Bashmaks.FirstOrDefaultAsync(x => x.Mac == bashmakDto.Mac);
                    if (bashmak == null) continue;
                    entity.Bashmakies.Add(bashmak);
                }
            }
            await _appDbContext.SaveChangesAsync();
        }

        [HttpPost("[action]")]
        public async Task SaveBoxes([FromBody] IEnumerable<BoxDto> dtos)
        {
            foreach(var dto in dtos)
            {
                var box = await _appDbContext.Boxes.FirstOrDefaultAsync(x => x.Mac == dto.Mac);
                if(box is null)
                    continue;
                box.DeviceName = dto.Name;
            }
            await _appDbContext.SaveChangesAsync();
        }

        [HttpPost("[action]")]
        public async Task SaveBashmaks([FromBody] IEnumerable<BashmakDto> bashmaks)
        {
            foreach (var dto in bashmaks)
            {
                var bashmak = await _appDbContext.Bashmaks.FirstOrDefaultAsync(x => x.Mac == dto.Mac);
                if (bashmak is null)
                    continue;
                bashmak.DeviceName = dto.Name;
            }
            await _appDbContext.SaveChangesAsync();
        }

        [HttpPost("[action]")]
        public IEnumerable<Bashmak> GetBashmaks([FromBody] IEnumerable<ST> types)
        {
            if (types is null || !types.Any())
                return _appDbContext.Bashmaks;
            return _appDbContext.Bashmaks.Where(x => types.Contains(x.St)).AsEnumerable();
        }


        [HttpPost("[action]")]
        public async Task DeleteAttachments([FromBody] IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                _appDbContext.Remove(new TrainAttachment { Id = id });
            }
            await _appDbContext.SaveChangesAsync();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<TrainAttachment>> CreateAttachment()
        {
            var attachment = new TrainAttachment
            {
                AttachingTimeUtc = DateTime.UtcNow,
                RailwayNumber = 0,
                WagonNumber = new Random().Next(1, 100),
                AttachingFitter = "Rurik",
                DetachingFitter = "Lobo",

            };

            try
            {
                await _domainService.AddEntity(attachment);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return attachment;
        }
    }
}
