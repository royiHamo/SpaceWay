namespace SpaceWay.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Passengers", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Passengers", "Username", c => c.String(nullable: false));
            AlterColumn("dbo.Passengers", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Passengers", "Password", c => c.String());
            AlterColumn("dbo.Passengers", "Username", c => c.String());
            AlterColumn("dbo.Passengers", "Name", c => c.String());
        }
    }
}
