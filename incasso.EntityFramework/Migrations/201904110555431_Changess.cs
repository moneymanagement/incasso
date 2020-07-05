namespace Incasso.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changess : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AdministratorUsers", newName: "UserAdministrators");
            DropPrimaryKey("dbo.UserAdministrators");
            AddPrimaryKey("dbo.UserAdministrators", new[] { "User_Id", "Administrator_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserAdministrators");
            AddPrimaryKey("dbo.UserAdministrators", new[] { "Administrator_Id", "User_Id" });
            RenameTable(name: "dbo.UserAdministrators", newName: "AdministratorUsers");
        }
    }
}
