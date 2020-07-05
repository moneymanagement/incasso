namespace Incasso.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class StatusCatalogMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StatusCatalogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Catalog = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleterUserId = c.Long(),
                        DeletionTime = c.DateTime(),
                        LastModificationTime = c.DateTime(),
                        LastModifierUserId = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    },
                annotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_StatusCatalog_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IsDeleted);
            
            DropColumn("dbo.Invoices", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "Status", c => c.Int(nullable: false));
            DropIndex("dbo.StatusCatalogs", new[] { "IsDeleted" });
            DropTable("dbo.StatusCatalogs",
                removedAnnotations: new Dictionary<string, object>
                {
                    { "DynamicFilter_StatusCatalog_SoftDelete", "EntityFramework.DynamicFilters.DynamicFilterDefinition" },
                });
        }
    }
}
