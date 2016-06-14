using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Display
{
    public sealed class Measurement
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
    }
}
