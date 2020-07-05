namespace Incasso.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dosier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Debtors", "DossierNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Debtors", "DossierNo");
        }
    }
}
