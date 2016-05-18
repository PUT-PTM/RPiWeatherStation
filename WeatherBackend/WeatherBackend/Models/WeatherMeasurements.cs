using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WeatherBackend.Models
{
    public class WeatherMeasurements
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure {get;set;}
        public DateTime Created { get; set; }
    }

    public class WeatherMeasurementsDBContext : DbContext
    {
        public DbSet<WeatherMeasurements> Weather { get; set; }
    }

}
