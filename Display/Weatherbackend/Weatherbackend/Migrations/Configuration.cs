namespace WeatherBackend.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WeatherBackend.Models.WeatherBackendContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WeatherBackend.Models.WeatherBackendContext context)
        {
            //  This method will be called after migrating to the latest version.

            //You can use the DbSet<T>.AddOrUpdate() helper extension method
            //to avoid creating duplicate seed data.E.g.

              context.Measurements.AddOrUpdate(
                p => p.Id,
                new Measurement { Id=1,Temperature = 20.5, Pressure=999.5},
                new Measurement { Id = 1, Temperature = 21.5, Pressure = 999.9 }
              );

        }
    }
}
