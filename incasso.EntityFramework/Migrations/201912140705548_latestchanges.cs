namespace Incasso.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latestchanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UploadAdministrators", "Admin_Id", "dbo.Administrators");
            DropForeignKey("dbo.UploadAdministrators", "Upload_Id", "dbo.Uploads");
            DropIndex("dbo.UploadAdministrators", new[] { "Admin_Id" });
            DropTable("dbo.UploadAdministrators");
            CreateTable(
                "dbo.UploadAdministrators",
                c => new
                    {
                        Upload_Id = c.Int(nullable: false),
                        Administrator_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Upload_Id, t.Administrator_Id })
                .ForeignKey("dbo.Uploads", t => t.Upload_Id, cascadeDelete: true)
                .ForeignKey("dbo.Administrators", t => t.Administrator_Id, cascadeDelete: true)
                .Index(t => t.Administrator_Id);
            
           
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UploadAdministrators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Upload_Id = c.Int(nullable: false),
                        Admin_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.UploadAdministrators", "Administrator_Id", "dbo.Administrators");
            DropForeignKey("dbo.UploadAdministrators", "Upload_Id", "dbo.Uploads");
            DropIndex("dbo.UploadAdministrators", new[] { "Administrator_Id" });
            DropTable("dbo.UploadAdministrators");
            CreateIndex("dbo.UploadAdministrators", "Admin_Id");
            AddForeignKey("dbo.UploadAdministrators", "Upload_Id", "dbo.Uploads", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UploadAdministrators", "Admin_Id", "dbo.Administrators", "Id", cascadeDelete: true);
        }
    }
}
