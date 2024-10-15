using Imsat.Bashmaky.Model.Database.Entities.Devices;

namespace Imsat.Bashmaky.Web.WebApi.Dto
{
    public class BashmakDto
    {
        public string? Name { get; set; }
        public string Mac { get; set; } = "";

        public static BashmakDto Create(Bashmak bashmak)
        {
            return new BashmakDto
            {
                Name = bashmak.DeviceName,
                Mac = bashmak.Mac
            };
        }
    }
}
