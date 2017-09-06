using Orc.Fortress.Attributes;

using Orc.Fortress.Cache;
using Orc.Fortress.Database;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orc.Fortress.BackOffice.Controllers
{
    [FortressPluginController]
    public class SettingsApiController : BaseFortressBackofficeController
    {
        public GoogleAuthenticatorSettingsModel GetGoogleAuthenticatorSettings()
        {
            CheckAuth();
            var settings = SettingsCache.Instance;
            var viewModel = new GoogleAuthenticatorSettingsModel()
            {
                Enabled = settings.GoogleAuthenticator_Enabled,
                Name = settings.GoogleAuthenticator_Name,
                Users = new List<UserListTwoFactorSettings>()
            };
            int totalUsers;
            var allUsers = ApplicationContext.Services.UserService.GetAll(0, 5000, out totalUsers);

            foreach(var user in allUsers)
            {
                var userDetails = CustomDatabase.GetUserDetails(user.Id);
                viewModel.Users.Add(new UserListTwoFactorSettings()
                {
                    Name = user.Name,
                    HasAccountEnabled = userDetails != null && userDetails.IsValidated,
                    Id = user.Id
                });
            }

            return viewModel;
        }
        public GoogleAuthenticatorSettingsModel RemoveGoogleAuthenticatorForUser(int id)
        {
            CheckAuth();
            var model = new UserTwoFactorSettings();
            
            var details = CustomDatabase.GetUserDetails(id);
            if (details == null || !details.IsValidated)
            {
                throw new UnauthorizedAccessException("This account hasnt got authenticator setup");
            }

            details.IsValidated = false;
            CustomDatabase.Update(details);


            return GetGoogleAuthenticatorSettings();
        }

        public bool SaveGoogleAuthenicatorSettings(GoogleAuthenticatorSettingsModel model)
        {
            CheckAuth();
            var settings = CustomDatabase.GetSettingsFromDatabase();

            settings.GoogleAuthenticator_Enabled = model.Enabled;
            settings.GoogleAuthenticator_Name = model.Name;

            CustomDatabase.SaveSettings(settings);

            SettingsCache.ClearCache();
            return true;
        }

        private void CheckAuth()
        {
            if (!Security.CurrentUser.AllowedSections.Contains(FortressConstants.UmbracoApplication.ApplicationAlias))
            {
                throw new Exception("You do not have access to this section");
            }
        }
    }
    public class GoogleAuthenticatorSettingsModel
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<UserListTwoFactorSettings> Users { get; internal set; }
    }
    public class UserListTwoFactorSettings
    {
        public string Name { get; set; }
        public bool HasAccountEnabled { get; set; }
        public int Id { get; internal set; }
    }
}