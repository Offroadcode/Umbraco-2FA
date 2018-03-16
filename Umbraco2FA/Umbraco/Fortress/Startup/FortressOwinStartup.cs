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
using Umbraco.Core.Configuration;

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

            var applicationContext = ApplicationContext.Current;
            LogHelper.Info(typeof(FortressOwinStartup), "Fortress: Startup");
            //Here's where we assign a custom UserManager called MyBackOfficeUserManager
            app.ConfigureUserManagerForUmbracoBackOffice<FortressBackOfficeUserManager, BackOfficeIdentityUser>(
                applicationContext,
                (options, context) =>
                {
                    var membershipProvider = Umbraco.Core.Security.MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();
                   
                //Create the custom MyBackOfficeUserManager
                var userManager = FortressBackOfficeUserManager.Create(options,
                        applicationContext.Services.UserService,
                        applicationContext.Services.EntityService,
                        applicationContext.Services.ExternalLoginService,
                        membershipProvider,
                         UmbracoConfig.For.UmbracoSettings().Content);
                    return userManager;
                });
            
            
        }


        //You don't need to override this unless you plan on implementing custom middleware which you might
        protected override void ConfigureMiddleware(IAppBuilder app)
        {
            LogHelper.Info(typeof (FortressOwinStartup), "OFFROADCODE: ConfigureMiddleware");

            app.UseTwoFactorSignInCookie(global::Umbraco.Core.Constants.Security.BackOfficeTwoFactorAuthenticationType, TimeSpan.FromMinutes(5));
            base.ConfigureMiddleware(app);
        }
    }
    
}