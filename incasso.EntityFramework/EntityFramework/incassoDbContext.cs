using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Incasso.Authorization.Roles;
using Incasso.Authorization.Users;
using Incasso.MultiTenancy;
using incasso.Debtors;
using incasso.Invoices;
using incasso.InvoiceNotes;

namespace Incasso.EntityFramework
{
    public class IncassoDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...
        public IDbSet<Administrators.Administrator> Administrators { get; set; }
        public IDbSet<Debtor> Debtors { get; set; }
        public IDbSet<Upload.Upload> Uploads{ get; set; }
        public IDbSet<Invoice> invoices{ get; set; }
        public IDbSet<InvoiceNote> InvoiceNotes { get; set; }
        public IDbSet<StatusCatalog> StatusCatalog { get; set; }
        

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public IncassoDbContext()
            : base("IncassoDB")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in incassoDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of incassoDbContext since ABP automatically handles it.
         */
        //public IncassoDbContext(string nameOrConnectionString)
        //    : base(nameOrConnectionString)
        //{

        //}

        //This constructor is used in tests
        //public IncassoDbContext(DbConnection existingConnection)
        // : base(existingConnection, false)
        //{

        //}

        //public IncassoDbContext(DbConnection existingConnection, bool contextOwnsConnection)
        // : base(existingConnection, contextOwnsConnection)
        //{

        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
