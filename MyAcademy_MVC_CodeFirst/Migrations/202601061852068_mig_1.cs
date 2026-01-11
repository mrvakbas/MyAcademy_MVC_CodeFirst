namespace MyAcademy_MVC_CodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Policies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyNumber = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        AppUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AppUserId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.AppUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Policies", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Policies", "AppUserId", "dbo.Users");
            DropIndex("dbo.Policies", new[] { "AppUserId" });
            DropIndex("dbo.Policies", new[] { "CategoryId" });
            DropTable("dbo.Policies");
        }
    }
}
