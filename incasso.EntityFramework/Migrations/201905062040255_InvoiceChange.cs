namespace Incasso.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Administrators", "AdminId", c => c.String());
            AddColumn("dbo.Invoices", "Paractitioner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "Paractitioner");
            DropColumn("dbo.Administrators", "AdminId");
        }
    }
}
