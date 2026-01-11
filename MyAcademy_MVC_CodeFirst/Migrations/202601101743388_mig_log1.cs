namespace MyAcademy_MVC_CodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig_log1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Logs", "User_Id", "dbo.Users");
            DropIndex("dbo.Logs", new[] { "User_Id" });
            DropTable("dbo.Logs");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.LogId);
            
            CreateIndex("dbo.Logs", "User_Id");
            AddForeignKey("dbo.Logs", "User_Id", "dbo.Users", "Id");
        }
    }
}
