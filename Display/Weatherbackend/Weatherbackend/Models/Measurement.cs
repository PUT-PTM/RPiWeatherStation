using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherBackend.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
    }
}