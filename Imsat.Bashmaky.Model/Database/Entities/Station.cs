using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imsat.Bashmaky.Model.Database.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IEnumerable<BaseEntity> Signals { get; set; }
    }
}
