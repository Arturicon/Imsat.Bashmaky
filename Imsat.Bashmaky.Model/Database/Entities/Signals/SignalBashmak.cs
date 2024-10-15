using Imsat.Bashmaky.Model.Database.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imsat.Bashmaky.Model.Database.Entities.Devices
{
    public class SignalBashmak : BaseSignal
    {
        public string Mac { get; set; }
        public override DeviceType DS => DeviceType.Bashmak;
    }
}
