namespace Incasso.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserAdministrators", newName: "AdministratorUsers");
            DropPrimaryKey("dbo.AdministratorUsers");
            AddPrimaryKey("dbo.AdministratorUsers", new[] { "Administrator_Id", "User_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.AdministratorUsers");
            AddPrimaryKey("dbo.AdministratorUsers", new[] { "User_Id", "Administrator_Id" });
            RenameTable(name: "dbo.AdministratorUsers", newName: "UserAdministrators");
        }
    }
}
