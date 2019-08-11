namespace SpaceWay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Aircraft",
                c => new
                    {
                        AircraftID = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        Seats = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AircraftID);
            
            CreateTable(
                "dbo.Flights",
                c => new
                    {
                        FlightID = c.Int(nullable: false, identity: true),
                        NumOfPassengers = c.Int(nullable: false),
                        AircraftID = c.Int(nullable: false),
                        OriginID = c.Int(nullable: false),
                        DestinationID = c.Int(nullable: false),
                        Duration = c.Double(nullable: false),
                        Distance = c.Double(nullable: false),
                        Departure = c.DateTime(nullable: false),
                        Arrival = c.DateTime(nullable: false),
                        Price = c.Double(nullable: false),
                        Destination_StationID = c.Int(),
                        Origin_StationID = c.Int(),
                    })
                .PrimaryKey(t => t.FlightID)
                .ForeignKey("dbo.Aircraft", t => t.AircraftID, cascadeDelete: true)
                .ForeignKey("dbo.Stations", t => t.Destination_StationID)
                .ForeignKey("dbo.Stations", t => t.Origin_StationID)
                .Index(t => t.AircraftID)
                .Index(t => t.Destination_StationID)
                .Index(t => t.Origin_StationID);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        StationID = c.Int(nullable: false, identity: true),
                        Planet = c.String(),
                        Name = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.StationID);
            
            CreateTable(
                "dbo.Passengers",
                c => new
                    {
                        PassengerID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Stars = c.Int(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        TotalDistance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PassengerID);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        ReservationID = c.Int(nullable: false, identity: true),
                        PassengerID = c.Int(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        OutboundID = c.Int(nullable: false),
                        InboundID = c.Int(nullable: false),
                        NumOfTickets = c.Int(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                        Inbound_FlightID = c.Int(),
                        Outbound_FlightID = c.Int(),
                    })
                .PrimaryKey(t => t.ReservationID)
                .ForeignKey("dbo.Flights", t => t.Inbound_FlightID)
                .ForeignKey("dbo.Flights", t => t.Outbound_FlightID)
                .ForeignKey("dbo.Passengers", t => t.PassengerID, cascadeDelete: true)
                .Index(t => t.PassengerID)
                .Index(t => t.Inbound_FlightID)
                .Index(t => t.Outbound_FlightID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reservations", "PassengerID", "dbo.Passengers");
            DropForeignKey("dbo.Reservations", "Outbound_FlightID", "dbo.Flights");
            DropForeignKey("dbo.Reservations", "Inbound_FlightID", "dbo.Flights");
            DropForeignKey("dbo.Flights", "Origin_StationID", "dbo.Stations");
            DropForeignKey("dbo.Flights", "Destination_StationID", "dbo.Stations");
            DropForeignKey("dbo.Flights", "AircraftID", "dbo.Aircraft");
            DropIndex("dbo.Reservations", new[] { "Outbound_FlightID" });
            DropIndex("dbo.Reservations", new[] { "Inbound_FlightID" });
            DropIndex("dbo.Reservations", new[] { "PassengerID" });
            DropIndex("dbo.Flights", new[] { "Origin_StationID" });
            DropIndex("dbo.Flights", new[] { "Destination_StationID" });
            DropIndex("dbo.Flights", new[] { "AircraftID" });
            DropTable("dbo.Reservations");
            DropTable("dbo.Passengers");
            DropTable("dbo.Stations");
            DropTable("dbo.Flights");
            DropTable("dbo.Aircraft");
        }
    }
}
