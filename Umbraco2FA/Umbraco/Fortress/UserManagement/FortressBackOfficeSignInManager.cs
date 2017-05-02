using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Orc.Fortress.Database;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;

namespace Orc.Fortress.UserManagement
{
    public class FortressBackOfficeSignInManager : BackOfficeSignInManager
    {
        public FortressBackOfficeSignInManager(UserManager<BackOfficeIdentityUser, int> userManager,
            IAuthenticationManager authenticationManager, ILogger logger, IOwinRequest request)
            : base(userManager, authenticationManager, logger, request)
        {
            Request = request;
        }

        public IOwinRequest Request { get; set; }

        public new static FortressBackOfficeSignInManager Create(IdentityFactoryOptions<BackOfficeSignInManager> options,
            IOwinContext context, ILogger logger)
        {
            return new FortressBackOfficeSignInManager(
                context.GetBackOfficeUserManager(),
                context.Authentication,
                logger,
                context.Request);

        }

        /// <summary>
        ///     Sign in the user in using the user name and password
        /// </summary>
        /// <param name="userName" />
        /// <param name="password" />
        /// <param name="isPersistent" />
        /// <param name="shouldLockout" />
        /// <returns />
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent,
            bool shouldLockout)
        {
            var db = new FortressDatabase();


            var result = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            if (result == SignInStatus.LockedOut)
            {
                db.AddNewSignInEvent(userName, SignInStatus.LockedOut, false, Request.RemoteIpAddress);
                HttpContext.Current.Items.Add(FortressConstants.LockoutItemKey, true);
                return SignInStatus.RequiresVerification;
            }
            db.AddNewSignInEvent(userName, result, false, Request.RemoteIpAddress);

            return result;
        }

        public override async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent,
            bool rememberBrowser)
        {
            var result = await base.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);

            var db = new FortressDatabase();

            db.AddNewSignInEvent(this.GetVerifiedUserId(), result, true, Request.RemoteIpAddress);

            return result;
        }
    }
}
