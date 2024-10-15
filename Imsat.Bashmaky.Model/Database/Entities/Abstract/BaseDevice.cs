using Imsat.Bashmaky.Model.Database.Types;

namespace Imsat.Bashmaky.Model.Database.Entities
{
    public class BaseDevice : BaseEntity
    {           
        public Connection Connection { get; set; }
        public string? DeviceName { get; set; }
    }
}
