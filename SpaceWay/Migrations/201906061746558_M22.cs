namespace SpaceWay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M22 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Flights", "Destination_StationID", "dbo.Stations");
            DropForeignKey("dbo.Flights", "Origin_StationID", "dbo.Stations");
            DropIndex("dbo.Flights", new[] { "Destination_StationID" });
            DropIndex("dbo.Flights", new[] { "Origin_StationID" });
            AddColumn("dbo.Stations", "Flight_FlightID", c => c.Int());
            CreateIndex("dbo.Stations", "Flight_FlightID");
            AddForeignKey("dbo.Stations", "Flight_FlightID", "dbo.Flights", "FlightID");
            DropColumn("dbo.Flights", "OriginID");
            DropColumn("dbo.Flights", "DestinationID");
            DropColumn("dbo.Flights", "Destination_StationID");
            DropColumn("dbo.Flights", "Origin_StationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Flights", "Origin_StationID", c => c.Int());
            AddColumn("dbo.Flights", "Destination_StationID", c => c.Int());
            AddColumn("dbo.Flights", "DestinationID", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "OriginID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Stations", "Flight_FlightID", "dbo.Flights");
            DropIndex("dbo.Stations", new[] { "Flight_FlightID" });
            DropColumn("dbo.Stations", "Flight_FlightID");
            CreateIndex("dbo.Flights", "Origin_StationID");
            CreateIndex("dbo.Flights", "Destination_StationID");
            AddForeignKey("dbo.Flights", "Origin_StationID", "dbo.Stations", "StationID");
            AddForeignKey("dbo.Flights", "Destination_StationID", "dbo.Stations", "StationID");
        }
    }
}
