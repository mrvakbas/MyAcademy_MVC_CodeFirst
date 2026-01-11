namespace MyAcademy_MVC_CodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig_city : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Policies", "City", c => c.String());
            AddColumn("dbo.Users", "City", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "City");
            DropColumn("dbo.Policies", "City");
        }
    }
}
