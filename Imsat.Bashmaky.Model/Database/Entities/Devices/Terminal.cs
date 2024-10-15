using Imsat.Bashmaky.Model.Database.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imsat.Bashmaky.Model.Database.Entities.Devices
{
    public class Terminal : BaseDevice
    {
        public string? NET {  get; set; }
        public int RSSI { get; set; }
        public override DeviceType DS => DeviceType.Terminal;
    }
}
