namespace MyAcademy_MVC_CodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginLogAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoginLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserName = c.String(),
                        LoginDate = c.DateTime(nullable: false),
                        LogoutDate = c.DateTime(),
                        IpAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LoginLogs");
        }
    }
}
