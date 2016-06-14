using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weatherbackend.Models
{
    public class Measurements
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
    }
}