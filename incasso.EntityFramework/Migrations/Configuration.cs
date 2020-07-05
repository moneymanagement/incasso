using System.Data.Entity.Migrations;
using System.Linq;
using Abp.MultiTenancy;
using Abp.Zero.EntityFramework;
using EntityFramework.DynamicFilters;
using incasso.Invoices;
using Incasso.EntityFramework;
using Incasso.Migrations.SeedData;

namespace Incasso.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<IncassoDbContext>, IMultiTenantSeed
    {
        public AbpTenantBase Tenant { get; set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "incasso";
        }

        protected override void Seed(IncassoDbContext context)
        {
            context.DisableAllFilters();

            if (Tenant == null)
            {
                //Host seed
                new InitialHostDbBuilder(context).Create();

                //Default tenant seed (in host database).
                new DefaultTenantCreator(context).Create();
                new TenantRoleAndUserBuilder(context, 1).Create();
            }
            else
            {
                //You can add seed for tenant databases and use Tenant property...
            }

            for (int i =0; i <=20; i++)
            {
                var status = InvoiceStatusCatalog.ParseToString(i);
                var defaultTenant = context.StatusCatalog.Any(t => t.Catalog.Trim() ==status.Trim() );
                if (!defaultTenant)
                {
                    context.StatusCatalog.Add(new StatusCatalog{Catalog= status});
                }
            }

            context.SaveChanges();
        }
    }
}
