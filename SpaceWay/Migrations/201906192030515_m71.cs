namespace SpaceWay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m71 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Flights", "Reservation_ReservationID", "dbo.Reservations");
            DropIndex("dbo.Flights", new[] { "Reservation_ReservationID" });
            AddColumn("dbo.Reservations", "OutboundID", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "InboundID", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "Inbound_FlightID", c => c.Int());
            AddColumn("dbo.Reservations", "Outbound_FlightID", c => c.Int());
            CreateIndex("dbo.Reservations", "Inbound_FlightID");
            CreateIndex("dbo.Reservations", "Outbound_FlightID");
            AddForeignKey("dbo.Reservations", "Inbound_FlightID", "dbo.Flights", "FlightID");
            AddForeignKey("dbo.Reservations", "Outbound_FlightID", "dbo.Flights", "FlightID");
            DropColumn("dbo.Flights", "Reservation_ReservationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Flights", "Reservation_ReservationID", c => c.Int());
            DropForeignKey("dbo.Reservations", "Outbound_FlightID", "dbo.Flights");
            DropForeignKey("dbo.Reservations", "Inbound_FlightID", "dbo.Flights");
            DropIndex("dbo.Reservations", new[] { "Outbound_FlightID" });
            DropIndex("dbo.Reservations", new[] { "Inbound_FlightID" });
            DropColumn("dbo.Reservations", "Outbound_FlightID");
            DropColumn("dbo.Reservations", "Inbound_FlightID");
            DropColumn("dbo.Reservations", "InboundID");
            DropColumn("dbo.Reservations", "OutboundID");
            CreateIndex("dbo.Flights", "Reservation_ReservationID");
            AddForeignKey("dbo.Flights", "Reservation_ReservationID", "dbo.Reservations", "ReservationID");
        }
    }
}
