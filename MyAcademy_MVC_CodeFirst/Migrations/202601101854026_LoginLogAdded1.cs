namespace MyAcademy_MVC_CodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginLogAdded1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoginLogs", "LogDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.LoginLogs", "Message", c => c.String());
            DropColumn("dbo.LoginLogs", "UserId");
            DropColumn("dbo.LoginLogs", "UserName");
            DropColumn("dbo.LoginLogs", "LoginDate");
            DropColumn("dbo.LoginLogs", "LogoutDate");
            DropColumn("dbo.LoginLogs", "IpAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoginLogs", "IpAddress", c => c.String());
            AddColumn("dbo.LoginLogs", "LogoutDate", c => c.DateTime());
            AddColumn("dbo.LoginLogs", "LoginDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.LoginLogs", "UserName", c => c.String());
            AddColumn("dbo.LoginLogs", "UserId", c => c.String());
            DropColumn("dbo.LoginLogs", "Message");
            DropColumn("dbo.LoginLogs", "LogDate");
        }
    }
}
