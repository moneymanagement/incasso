namespace Incasso.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StatusCatalogInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "Status", c => c.Int());
            CreateIndex("dbo.Invoices", "Status");
            AddForeignKey("dbo.Invoices", "Status", "dbo.StatusCatalogs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "Status", "dbo.StatusCatalogs");
            DropIndex("dbo.Invoices", new[] { "Status" });
            DropColumn("dbo.Invoices", "Status");
        }
    }
}
