using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Orc.Fortress.Database;
using Orc.Fortress.TwoFactorProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security.Identity;

namespace Orc.Fortress.UserManagement
{
    class FortressBackOfficeUserManager : BackOfficeUserManager, IUmbracoBackOfficeTwoFactorOptions
    {
        public FortressBackOfficeUserManager(IUserStore<BackOfficeIdentityUser, int> store)
        : base(store)
        {
        }

        /// <summary>
        /// Creates a BackOfficeUserManager instance with all default options and the default BackOfficeUserManager 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="userService"></param>
        /// <param name="externalLoginService"></param>
        /// <param name="membershipProvider"></param>
        /// <returns></returns>
        public static FortressBackOfficeUserManager Create(
            IdentityFactoryOptions<FortressBackOfficeUserManager> options,
            IUserService userService,
            IExternalLoginService externalLoginService,
            MembershipProviderBase membershipProvider)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (userService == null) throw new ArgumentNullException("userService");
            if (externalLoginService == null) throw new ArgumentNullException("externalLoginService");

            var manager = new FortressBackOfficeUserManager(new FortressBackOfficeUserStore(userService, externalLoginService, membershipProvider));

            manager.InitUserManager(manager, membershipProvider, options.DataProtectionProvider);


            //Here you can specify the 2FA providers that you want to implement, 
            //in this demo we are using the custom AcceptAnyCodeProvider - which literally accepts any code - do not actually use this!

            var dataProtectionProvider = options.DataProtectionProvider;
            manager.RegisterTwoFactorProvider("GoogleAuthenticator", new GoogleAuthenticatorProvider(dataProtectionProvider.Create("GoogleAuthenticator")));

            return manager;
        }
        /// <summary>
        /// Override to return true
        /// </summary>
        public override bool SupportsUserTwoFactor
        {
            get { return true;  }
        }

        /// <summary>
        /// Return the view for the 2FA screen
        /// </summary>
        /// <param name="owinContext"></param>
        /// <param name="umbracoContext"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetTwoFactorView(IOwinContext owinContext, UmbracoContext umbracoContext, string username)
        {
            var user = ApplicationContext.Current.Services.UserService.GetByUsername(username);
            var database = new FortressDatabase();

            var details = database.GetUserDetails(user.Id);
            var provider = TwoFactorProviders[details.Provider];
            //var providerDetails = provider as IuProtectTwoFactorProvider;
            if (provider!= null)
            {
                return "/App_Plugins/Umbraco2FA/backoffice/TwoFactor/TwoFactorLogin.html";
            }
            return "/App_Plugins/Umbraco2FA/backoffice/TwoFactor/GenericError.html";
            
        }
    }

}
