namespace SpaceWay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flights", "OriginID", c => c.Int(nullable: false));
            AddColumn("dbo.Flights", "DestinationID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Flights", "DestinationID");
            DropColumn("dbo.Flights", "OriginID");
        }
    }
}
