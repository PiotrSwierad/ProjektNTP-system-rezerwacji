namespace ProjektNTP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "Plates", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "Plates");
        }
    }
}
