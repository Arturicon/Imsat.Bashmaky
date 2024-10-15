using Imsat.Bashmaky.Model.Database.Entities.Devices;

namespace Imsat.Bashmaky.Web.WebApi.Dto
{
    public class BoxDto
    {
        public string? Name { get; set; }
        public string Mac { get; set; } = "";

        public static BoxDto Create(Box bashmak)
        {
            return new BoxDto
            {
                Name = bashmak.DeviceName,
                Mac = bashmak.Mac
            };
        }
    }
}
