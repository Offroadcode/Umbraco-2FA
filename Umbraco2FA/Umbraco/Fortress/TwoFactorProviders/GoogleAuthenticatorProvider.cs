using Google.Authenticator;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Orc.Fortress.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.Identity;

namespace Orc.Fortress.TwoFactorProviders
{
    public class GoogleAuthenticatorProvider : DataProtectorTokenProvider<BackOfficeIdentityUser, int>, IUserTokenProvider<BackOfficeIdentityUser, int>
    {
        public GoogleAuthenticatorProvider(IDataProtector protector)
            : base(protector)
        {
        }
        //
        // Summary:
        //     Returns true if provider can be used for this user, i.e. could require a user
        //     to have an email
        //
        // Parameters:
        //   manager:
        //
        //   user:
        Task<bool> IUserTokenProvider<BackOfficeIdentityUser, int>.IsValidProviderForUserAsync(UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {

            var db = new FortressDatabase();
            if (!db.GetSettingsFromDatabase().GoogleAuthenticator_Enabled)
            {
                return Task.FromResult(false);
            }

            var details = db.GetUserDetails(user.Id);
            if (details == null || !details.IsValidated || details.Provider == "GoogleAuthenticator")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        /// <summary>
        /// Explicitly implement this interface method - which overrides the base class's implementation
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="token"></param>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> IUserTokenProvider<BackOfficeIdentityUser, int>.ValidateAsync(string purpose, string token, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var db = new FortressDatabase();
            var details = db.GetUserDetails(user.Id);
            
            bool isCorrectPIN = tfa.ValidateTwoFactorPIN(details.Configuration,token);
            
            if(details.IsValidated == false && isCorrectPIN)
            {
                details.IsValidated = true;
                db.Update(details);
            }
            return Task.FromResult(isCorrectPIN);   
        }
    }
}
