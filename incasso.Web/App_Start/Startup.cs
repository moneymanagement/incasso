﻿using System;
using System.Configuration;
using Abp.Owin;
using Incasso.Web;
using Incasso.Api.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Hangfire.SqlServer;
using Hangfire;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using Abp.BackgroundJobs;
using Abp.Hangfire;

[assembly: OwinStartup(typeof(Startup))]

namespace Incasso.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseAbp();

            app.UseOAuthBearerAuthentication(AccountController.OAuthBearerOptions);
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard();
            app.UseCookieAuthentication(GetCookieAuthenticationOptions());

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.MapSignalR();

            //ENABLE TO USE HANGFIRE dashboard(Requires enabling Hangfire in incassoWebModule)
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AbpHangfireAuthorizationFilter() } //You can remove this line to disable authorization
            });
        }
        private static CookieAuthenticationOptions GetCookieAuthenticationOptions()
        {
            var options = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                // by setting following values, the auth cookie will expire after the configured amount of time (default 14 days) when user set the (IsPermanent == true) on the login
                //ExpireTimeSpan = TimeSpan.FromDays(4),
                CookieName = ".Application.Session",
            };
            var provider = (CookieAuthenticationProvider)options.Provider;
            provider.OnResponseSignIn = (context) =>
            {
                context.Properties.IsPersistent = true;
                context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24);
            };
            return options;
        }

        private IEnumerable<IDisposable> GetHangfireServers()
        {
            Hangfire.GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("IncassoDB", new SqlServerStorageOptions()
                 );

            yield return new BackgroundJobServer();
        }

    }
}
