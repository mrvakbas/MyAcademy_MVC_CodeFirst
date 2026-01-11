namespace MyAcademy_MVC_CodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig_log : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Action = c.String(),
                        Details = c.String(),
                        LogDate = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.Users", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "User_Id", "dbo.Users");
            DropIndex("dbo.Logs", new[] { "User_Id" });
            DropColumn("dbo.Users", "UserId");
            DropTable("dbo.Logs");
        }
    }
}
