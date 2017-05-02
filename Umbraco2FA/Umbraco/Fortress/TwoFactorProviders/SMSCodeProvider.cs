using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Orc.Fortress.Database;
using Orc.Fortress.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Models.Identity;

namespace Orc.Fortress.TwoFactorProviders
{
    public class SMSCodeProvider : DataProtectorTokenProvider<BackOfficeIdentityUser, int>, IUserTokenProvider<BackOfficeIdentityUser, int>
    {
        private static readonly CryptoRandomGenerator _random = new CryptoRandomGenerator();
        private static readonly object syncLock = new object();

        public SMSCodeProvider(IDataProtector protector)
            : base(protector)
        {
        }
        Task IUserTokenProvider<BackOfficeIdentityUser, int>.NotifyAsync(string token, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            var db = new FortressDatabase();
            var details = db.GetUserDetails(user.Id);

            var settings = db.GetSettingsFromDatabase();
            if (details == null || !details.IsValidated || details.Provider == "SMS")
            {
                details.CurrentCode = token;
                details.CurrentCodeGenerated = DateTime.UtcNow;
                db.Update(details);
            }
            var SmsProvider = FortressContext.GetCurrentSmsProvider();
            SmsProvider.SendSms(details.Configuration, string.Format(settings.SMS_MessageFormat, token));
            return Task.FromResult(true);
        }
        Task<string> IUserTokenProvider<BackOfficeIdentityUser, int>.GenerateAsync(string purpose, UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            lock (syncLock)
            {
                var code = _random.Next(999999).ToString();
                return Task.FromResult(code);
            }
        }
        Task<bool> IUserTokenProvider<BackOfficeIdentityUser, int>.IsValidProviderForUserAsync(UserManager<BackOfficeIdentityUser, int> manager, BackOfficeIdentityUser user)
        {
            var db = new FortressDatabase();
            if (!db.GetSettingsFromDatabase().SMS_Enabled)
            {
                return Task.FromResult(false);
            }

            var details = db.GetUserDetails(user.Id);
            if (details == null || details.Provider == "SMS")
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
            
            var db = new FortressDatabase();
            var details = db.GetUserDetails(user.Id);
            if (details != null && details.Provider == "SMS")
            {
                return Task.FromResult(details.CurrentCode == token);
            }

            return Task.FromResult(false);
        }
    }
}
