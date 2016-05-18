namespace WeatherBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeatherMeasurements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Temperature = c.Double(nullable: false),
                        Humidity = c.Double(nullable: false),
                        Pressure = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WeatherMeasurements");
        }
    }
}
