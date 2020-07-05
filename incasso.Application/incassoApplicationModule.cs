using System;
using System.Globalization;
using System.Reflection;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Modules;
using AutoMapper;
using incasso.Helper;
using Incasso.Authorization.Roles;
using Incasso.Authorization.Users;
using Incasso.Roles.Dto;
using Incasso.Users.Dto;

namespace Incasso
{
    [DependsOn(typeof(incassoCoreModule), typeof(AbpAutoMapperModule))]
    public class incassoApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            // TODO: Is there somewhere else to store these, with the dto classes
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg .CreateMap<DateTime, string>()
                    .ConvertUsing(dt => dt.ToString(DateHelper.IncassoDateFormat));

                cfg .CreateMap<DateTime?, string>()
                    .ConvertUsing(dt => dt.HasValue ? dt.Value.ToString(DateHelper.IncassoDateFormat) : string.Empty);

                cfg.CreateMap<string, DateTime>().ConvertUsing<StringToDateTimeConverter>();
                 cfg .CreateMap<string, DateTime?>().ConvertUsing<StringToDateTimeNullConverter>();

                // Role and permission
                cfg.CreateMap<Permission, string>().ConvertUsing(r => r.Name);
                cfg.CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

                cfg.CreateMap<CreateRoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
                cfg.CreateMap<RoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
                
                cfg.CreateMap<UserDto, User>();
                cfg.CreateMap<UserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());

                cfg.CreateMap<CreateUserDto, User>();
                cfg.CreateMap<CreateUserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());
            });
        }
    }


    public class StringToDateTimeNullConverter : ITypeConverter<string, DateTime?>
    {
        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            string objDateTime = source;
            DateTime dateTime;

            if (string.IsNullOrWhiteSpace(objDateTime) )
            {
                return null;
            }
            try
            {

              DateTime.TryParseExact(objDateTime, DateHelper.GetFormates().ToArray(), System.Globalization.CultureInfo.InvariantCulture,DateTimeStyles.AssumeLocal,out DateTime date);
                return date;
            }
            catch (Exception e)
            {
               return null;
            }
             
            return null;
        }
    }
    public class StringToDateTimeConverter : ITypeConverter<string, DateTime>
    { 
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            string objDateTime = source;

            if (string.IsNullOrWhiteSpace(objDateTime))
            {
                return default(DateTime);
            }
             
            try
            {
                DateTime.TryParseExact(objDateTime, DateHelper.GetFormates().ToArray(), System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime date);
                return date;
            }
            catch (Exception e)
            {
            }

            return default(DateTime);
        }
    }
}
