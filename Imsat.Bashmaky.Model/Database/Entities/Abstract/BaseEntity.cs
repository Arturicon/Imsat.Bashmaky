using Imsat.Bashmaky.Model.Database.Types;

namespace Imsat.Bashmaky.Model.Database.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime TS { get; set; }
        public string? Imei { get; set; }
        public int? StationId { get; set; }
        public Station? Station { get; set; }
        public virtual DeviceType DS { get; }
        public float LAT { get; set; }
        public float LON { get; set; }
        public double VDD { get; set; }
        public ST St { get; set; }
    }
}
