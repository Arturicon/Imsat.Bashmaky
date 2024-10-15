using Imsat.Bashmaky.Model.Database.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imsat.Bashmaky.Model.Database.Entities.Devices
{
    public class Bashmak : BaseDevice
    {
        public string Mac { get; set; }
        public override DeviceType DS => DeviceType.Bashmak;
        public List<Railway> Railways { get; set; } = new List<Railway>();
        public Box? Box {  get; set; } 
        public int? BoxId {  get; set; } 
        public string RailwayStr => Railways.Any()? Railways.Select(x => x.Name).Aggregate((x, y) => $"{x}, {y}") : string.Empty;
    }
}
