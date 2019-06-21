namespace SpaceWay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M28 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stations", "Flight_FlightID", "dbo.Flights");
            DropIndex("dbo.Stations", new[] { "Flight_FlightID" });
            AddColumn("dbo.Flights", "Destination_StationID", c => c.Int());
            AddColumn("dbo.Flights", "Origin_StationID", c => c.Int());
            CreateIndex("dbo.Flights", "Destination_StationID");
            CreateIndex("dbo.Flights", "Origin_StationID");
            AddForeignKey("dbo.Flights", "Destination_StationID", "dbo.Stations", "StationID");
            AddForeignKey("dbo.Flights", "Origin_StationID", "dbo.Stations", "StationID");
            DropColumn("dbo.Stations", "Flight_FlightID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stations", "Flight_FlightID", c => c.Int());
            DropForeignKey("dbo.Flights", "Origin_StationID", "dbo.Stations");
            DropForeignKey("dbo.Flights", "Destination_StationID", "dbo.Stations");
            DropIndex("dbo.Flights", new[] { "Origin_StationID" });
            DropIndex("dbo.Flights", new[] { "Destination_StationID" });
            DropColumn("dbo.Flights", "Origin_StationID");
            DropColumn("dbo.Flights", "Destination_StationID");
            CreateIndex("dbo.Stations", "Flight_FlightID");
            AddForeignKey("dbo.Stations", "Flight_FlightID", "dbo.Flights", "FlightID");
        }
    }
}
