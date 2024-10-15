using Imsat.Bashmaky.Model.Database.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imsat.Bashmaky.Model.Database.Entities.Devices
{
    public class Box : BaseDevice
    {
        public string Mac {  get; set; }
        public override DeviceType DS => DeviceType.Box;
        public List<Bashmak> Bashmaks { get; set; } = new List<Bashmak>();
        public int MaxBashmaks => 10;
        public double PercentageOfFilling => Math.Round((double)Bashmaks.Count / (double)MaxBashmaks * 100);
    }
}
