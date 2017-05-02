using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using Orc.Fortress.Attributes;
using System.Linq;
using Orc.Fortress.Database.Models;
using Google.Authenticator;
using Orc.Fortress.Logic;

namespace Orc.Fortress.Controllers
{
    [FortressPluginController]
    public class TwoFactorSetupController : BaseFortressAuthController
    {
        private static readonly CryptoRandomGenerator _random = new CryptoRandomGenerator();
        private static readonly object syncLock = new object();

        public List<string> GetAvailableTwoFactorMethods()
        {
            return UserManager.TwoFactorProviders.Keys.ToList();
        }
        public HttpResponseMessage SetupGoogleAuth()
        {
            var databaseSettings = CustomDatabase.GetSettingsFromDatabase();

            if (!databaseSettings.GoogleAuthenticator_Enabled)
            {
                throw new Exception("Google Authenticator is disabled");
            }
            var user = SignInManager.GetVerifiedUserId();
            
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

            var secretKey = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

            var setupInfo = tfa.GenerateSetupCode(CustomDatabase.GetSettingsFromDatabase().GoogleAuthenticator_Name, user.ToString(), secretKey, 300, 300);

            var details = CustomDatabase.GetUserDetails(user);
            if (details != null && details.IsValidated)
            {
                throw new UnauthorizedAccessException("This account has already setup GoogleAuthenticator");
            }
            var isNew = details == null;
            details = new FortressUser2FASettings();
            details.UserId = user;
            details.Provider = "GoogleAuthenticator";
            details.Configuration = secretKey;
            details.IsValidated = false;
            if (isNew)
            {
                CustomDatabase.Insert(details);
            }
            else
            {
                CustomDatabase.Update(details);
            }

            string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            string manualEntrySetupCode = setupInfo.ManualEntryKey;

            var response = Request.CreateResponse(HttpStatusCode.OK, new { image = qrCodeImageUrl, manualEntryCode = manualEntrySetupCode });
            return response;
        }
        public HttpResponseMessage SetupSMS(string number)
        {
            var user = SignInManager.GetVerifiedUserId();
            var details = CustomDatabase.GetUserDetails(user);
            if (details != null && details.IsValidated)
            {
                throw new UnauthorizedAccessException("This account has already setup SMS");
            }
            var isNew = details == null;
            details = new FortressUser2FASettings();
            details.UserId = user;
            details.Provider = "SMS";
            details.Configuration = number;
            details.IsValidated = false;

            lock (syncLock)
            {
                var code = _random.Next(999999).ToString();
                details.CurrentCode = code;
                details.CurrentCodeGenerated = DateTime.UtcNow;
            }
        
            if (isNew)
            {
                CustomDatabase.Insert(details);
            }
            else
            {
                CustomDatabase.Update(details);
            }
            var settings = CustomDatabase.GetSettingsFromDatabase();
            var SmsProvider = FortressContext.GetCurrentSmsProvider();

            SmsProvider.SendSms(details.Configuration, string.Format(settings.SMS_MessageFormat, details.CurrentCode));

            var response = Request.CreateResponse(HttpStatusCode.OK, new {token ="123456"});
            return response;
        }
    }
}