using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.Zero.Configuration;
using Abp.Modules;
using Abp.Web.Mvc;
using Abp.Web.SignalR;
using Castle.MicroKernel.Registration;
using Hangfire;
using Incasso.Api;
using Microsoft.Owin.Security;
using Abp.Domain.Repositories;
using Incasso.MultiTenancy;
using Incasso.Administrator;
using Abp.Configuration.Startup;
using Newtonsoft.Json.Serialization;
using Abp.Extensions;
using Incasso.Upload;

namespace Incasso.Web
{
    [DependsOn(
        typeof(incassoDataModule),
        typeof(incassoApplicationModule),
        typeof(incassoWebApiModule),
        typeof(AbpWebSignalRModule),
      typeof(AbpHangfireModule),  // - ENABLE TO USE HANGFIRE INSTEAD OF DEFAULT JOB MANAGER
        typeof(AbpWebMvcModule))]
    public class incassoWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Enable database based localization
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();
            //Configuration.Modules.Zero().AntiForgery.IsEnabled = false;
            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<incassoNavigationProvider>();
            Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;
            //Configure Hangfire - ENABLE TO USE HANGFIRE INSTEAD OF DEFAULT JOB MANAGER
            Configuration.BackgroundJobs.UseHangfire(configuration =>
            {
                configuration.GlobalConfiguration.UseSqlServerStorage("IncassoDB");
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;
            IocManager.IocContainer.Register(
                Component
                    .For<IAuthenticationManager>()
                    .UsingFactoryMethod(() => HttpContext.Current.GetOwinContext().Authentication)
                    .LifestyleTransient()
            );
            //IocManager.Register<IRepository<IDebtorsAppService, ObjectId>, MongoDbRepositoryBase<TreatmentPlannerCatalog, ObjectId>>(DependencyLifeStyle.Transient);
            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder.For<IUploadDataAppService>("app/UploadData").Build();
            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder.For<IDebtorsAppService>("app/Debtors").Build();
            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder.For<IInvoicesAppService>("app/Invoices").Build();
            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder.For<IAdministrationsAppService>("app/Administrations").Build();
            Configuration.Modules.AbpWebApi().HttpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public override void PostInitialize()
        {
            Configuration.Modules.AbpWebApi().HttpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new PascalCasePropertyNamesContractResolver();
            Configuration.Modules.AbpWebApi().HttpConfiguration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                   = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            Configuration.Modules.AbpWebApi().HttpConfiguration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling
                 = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            base.PostInitialize();
        }
    }

    public class PascalCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public new static readonly PascalCasePropertyNamesContractResolver Instance = new PascalCasePropertyNamesContractResolver();

        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToPascalCase();
        }
    }
}
