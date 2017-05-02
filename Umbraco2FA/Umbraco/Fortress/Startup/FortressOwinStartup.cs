using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Orc.Fortress;

using Owin;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Web;
using Umbraco.Web.Security.Identity;
using ILogger = Umbraco.Core.Logging.ILogger;
using Orc.Fortress.Startup;
using Orc.Fortress.UserManagement;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup("FortressOwinStartup", typeof (FortressOwinStartup))]

namespace Orc.Fortress.Startup
{
    public class FortressOwinStartup : UmbracoDefaultOwinStartup
    {
        /// <summary>
        ///     Configures services to be created in the OWIN context (CreatePerOwinContext)
        /// </summary>
        /// <param name="app"></param>
        protected override void ConfigureServices(IAppBuilder app)
        {
            app.SetUmbracoLoggerFactory();
            LogHelper.Info(typeof (FortressOwinStartup), "Fortress: Startup");

            app.SetUmbracoLoggerFactory();

            var applicationContext = ApplicationContext.Current;

            //Here's where we assign a custom UserManager called MyBackOfficeUserManager
            app.ConfigureUserManagerForUmbracoBackOffice<FortressBackOfficeUserManager, BackOfficeIdentityUser>(
                applicationContext,
                (options, context) =>
                {
                    var membershipProvider = Umbraco.Core.Security.MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();

                //Create the custom MyBackOfficeUserManager
                var userManager = FortressBackOfficeUserManager.Create(options,
                        applicationContext.Services.UserService,
                        applicationContext.Services.ExternalLoginService,
                        membershipProvider);
                    return userManager;
                });

            app.CreatePerOwinContext<BackOfficeSignInManager>(
          (options, context) =>
              FortressBackOfficeSignInManager.Create(options, context,
                  app.CreateLogger(typeof(BackOfficeSignInManager).FullName)));
        }


        //You don't need to override this unless you plan on implementing custom middleware which you might
        protected override void ConfigureMiddleware(IAppBuilder app)
        {
            LogHelper.Info(typeof (FortressOwinStartup), "OFFROADCODE: ConfigureMiddleware");

            //you can use the defaults - which executes what is listed below, however if you require
            // a specialized order, or something else you can register the defaults yourself as per below.

            app.UseTwoFactorSignInCookie(global::Umbraco.Core.Constants.Security.BackOfficeTwoFactorAuthenticationType, TimeSpan.FromMinutes(5));
            app.Use<FortressOWINFirewall>(app.CreateLogger<FortressOWINFirewall>());


            base.ConfigureMiddleware(app);
            //Ensure owin is configured for Umbraco back office authentication. If you have any front-end OWIN
            // cookie configuration, this must be declared after it.

            ////DEFAULT:
            //app
            //    .UseUmbracoBackOfficeCookieAuthentication(ApplicationContext, PipelineStage.Authenticate)
            //    .UseUmbracoBackOfficeExternalCookieAuthentication(ApplicationContext, PipelineStage.Authenticate)
            //    .UseUmbracoPreviewAuthentication(ApplicationContext, PipelineStage.Authorize)
            //    .FinalizeMiddlewareConfiguration();
        }
    }
    
}