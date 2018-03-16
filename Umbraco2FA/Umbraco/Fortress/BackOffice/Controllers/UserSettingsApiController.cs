using Google.Authenticator;
using Orc.Fortress.Attributes;
using Orc.Fortress.Database.Models;
using System;

namespace Orc.Fortress.BackOffice.Controllers
{
    [FortressPluginController]
    public class UserSettingsApiController : BaseFortressBackofficeController
    {
        public UserTwoFactorSettings GetMySettings()
        {
            var userDetails = CustomDatabase.GetUserDetails(Security.GetUserId());
            
            var viewModel = new UserTwoFactorSettings()
            {
                IsSetup = userDetails != null && userDetails.IsValidated
            };

            return viewModel;
        }

        public TwoFactorSetupOptions SetupAuthenticator()
        {
            var userId = Security.GetUserId();
            var details = CustomDatabase.GetUserDetails(userId);
            if (details != null && details.IsValidated)
            {
                throw new UnauthorizedAccessException("This account has already setup GoogleAuthenticator");
            }

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

            var secretKey = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

            var setupInfo = tfa.GenerateSetupCode(CustomDatabase.GetSettingsFromDatabase().GoogleAuthenticator_Name, userId.ToString(), secretKey, 300, 300);


            var isNew = details == null;
            details = new FortressUser2FASettings();
            details.UserId = userId;
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
            
            var response = new TwoFactorSetupOptions()
            {
                Image = qrCodeImageUrl,
                ManualEntryCode = manualEntrySetupCode
            };
            return response;
        }

        public TwoFactorValidation ValidateGoogleAuthSetup(string twoFactorCode)
        {
            var model = new TwoFactorValidation();
            var userId = Security.GetUserId();
            var details = CustomDatabase.GetUserDetails(userId);
            if (details != null && details.IsValidated)
            {
                throw new UnauthorizedAccessException("This account has already setup GoogleAuthenticator");
            }

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

            var isValid = tfa.ValidateTwoFactorPIN(details.Configuration, twoFactorCode);

            if (isValid)
            {
                details.IsValidated = true;
                CustomDatabase.Update(details);
                model.IsValid = true;
                model.Settings = GetMySettings();
            }
            else
            {
                model.IsValid = false;
            }
            return model;
        }
        public UserTwoFactorSettings RemoveTwoFactor()
        {
            var model = new UserTwoFactorSettings();
            var userId = Security.GetUserId();
            var details = CustomDatabase.GetUserDetails(userId);
            if (details == null || !details.IsValidated)
            {
                throw new UnauthorizedAccessException("This account hasnt got authenticator setup");
            }

            details.IsValidated = false;
            CustomDatabase.Update(details);
          
            return model;
        }
    }
    public class TwoFactorValidation
    {
        public bool IsValid { get; set; }
        public UserTwoFactorSettings Settings { get; set; }
    }
    public class UserTwoFactorSettings
    {
        public bool IsSetup { get; set; }
    }
    public class TwoFactorSetupOptions
    {
        public string Image { get; set; }
        public string ManualEntryCode { get; set; }
    }
}