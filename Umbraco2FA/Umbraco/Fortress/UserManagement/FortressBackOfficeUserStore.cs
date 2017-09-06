using Orc.Fortress.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Core.Services;

namespace Orc.Fortress.UserManagement
{
    /// <summary>
    /// Subclass the default BackOfficeUserManager and extend it to support 2FA
    /// </summary>
    public class FortressBackOfficeUserStore : BackOfficeUserStore
    {
        public FortressBackOfficeUserStore(IUserService userService, IExternalLoginService externalLoginService, MembershipProviderBase usersMembershipProvider)
            : base(userService, externalLoginService, usersMembershipProvider)
        {
        }

        /// <summary>
        /// Override to support setting whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"/><param name="enabled"/>
        /// <returns/>
        /// <remarks>
        /// This Demo does not persist any data, so this method doesn't really have any effect, if you wish 
        /// to have 2FA setup per user, you'll need to persist that somewhere and to do that you'd need to override
        /// the IUserStore.UpdateAsync method by explicitly implementing that interface's method, calling the base
        /// class logic and then updating your 2FA storage for the user. Similarly you'd have to do that for 
        /// IUserStore.DeleteAsync and IUserStore.CreateAsync. 
        /// 
        /// This method is NOT designed to persist data! It's just meant to assign it, just like this
        /// </remarks>
        public override Task SetTwoFactorEnabledAsync(BackOfficeIdentityUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Returns whether two factor authentication is enabled for the user
        /// </summary>
        /// <param name="user"/>
        /// <returns/>
        /// <remarks>
        /// This Demo does not persist any data, so this method for this Demo always returns true.
        /// If you want to have 2FA configured per user, you will need to store that information somewhere.
        /// See the notes above in the SetTwoFactorEnabledAsync method.
        /// </remarks>
        public override Task<bool> GetTwoFactorEnabledAsync(BackOfficeIdentityUser user)
        {
            var db = new FortressDatabase();
            var details = db.GetUserDetails(user.Id);
            if (details != null && details.IsValidated)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);

            //If you persisted this data somewhere then you could either look it up now, or you could
            //explicitly implement all IUserStore "Find*" methods, call their base implementation and then lookup
            //your persisted value and assign to the TwoFactorEnabled property of the resulting BackOfficeIdentityUser user.
            //return Task.FromResult(user.TwoFactorEnabled);
        }
    }
}
