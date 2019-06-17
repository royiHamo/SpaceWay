namespace SpaceWay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M23 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservations", "Inbound_FlightID", "dbo.Flights");
            DropForeignKey("dbo.Reservations", "Outbound_FlightID", "dbo.Flights");
            DropIndex("dbo.Reservations", new[] { "Inbound_FlightID" });
            DropIndex("dbo.Reservations", new[] { "Outbound_FlightID" });
            AddColumn("dbo.Flights", "Reservation_ReservationID", c => c.Int());
            CreateIndex("dbo.Flights", "Reservation_ReservationID");
            AddForeignKey("dbo.Flights", "Reservation_ReservationID", "dbo.Reservations", "ReservationID");
            DropColumn("dbo.Reservations", "OutFlightID");
            DropColumn("dbo.Reservations", "InFlightID");
            DropColumn("dbo.Reservations", "Inbound_FlightID");
            DropColumn("dbo.Reservations", "Outbound_FlightID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "Outbound_FlightID", c => c.Int());
            AddColumn("dbo.Reservations", "Inbound_FlightID", c => c.Int());
            AddColumn("dbo.Reservations", "InFlightID", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "OutFlightID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Flights", "Reservation_ReservationID", "dbo.Reservations");
            DropIndex("dbo.Flights", new[] { "Reservation_ReservationID" });
            DropColumn("dbo.Flights", "Reservation_ReservationID");
            CreateIndex("dbo.Reservations", "Outbound_FlightID");
            CreateIndex("dbo.Reservations", "Inbound_FlightID");
            AddForeignKey("dbo.Reservations", "Outbound_FlightID", "dbo.Flights", "FlightID");
            AddForeignKey("dbo.Reservations", "Inbound_FlightID", "dbo.Flights", "FlightID");
        }
    }
}
