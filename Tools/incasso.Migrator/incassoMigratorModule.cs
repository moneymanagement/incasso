using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Incasso.EntityFramework;

namespace Incasso.Migrator
{
    [DependsOn(typeof(incassoDataModule))]
    public class incassoMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<IncassoDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}