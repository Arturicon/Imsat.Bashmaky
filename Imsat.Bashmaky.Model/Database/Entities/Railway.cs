using Imsat.Bashmaky.Model.Database.Entities.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imsat.Bashmaky.Model.Database.Entities
{
    public class Railway
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Bashmak> Bashmaks { get; set; } = new List<Bashmak>();

    }
}
