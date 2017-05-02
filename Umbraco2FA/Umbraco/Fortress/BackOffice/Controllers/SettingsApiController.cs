using Orc.Fortress.Attributes;
using Orc.Fortress.BackOffice.Models;
using Orc.Fortress.Cache;
using Orc.Fortress.Database;
using Orc.Fortress.Models;
using Orc.Fortress.SMSProvider;
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
            var settings = SettingsCache.Instance;
            var viewModel = new GoogleAuthenticatorSettingsModel()
            {
                Enabled = settings.GoogleAuthenticator_Enabled,
                Name = settings.GoogleAuthenticator_Name
            };
            return viewModel;
        }
        public bool SaveGoogleAuthenicatorSettings(GoogleAuthenticatorSettingsModel model)
        {
            var settings = CustomDatabase.GetSettingsFromDatabase();

            settings.GoogleAuthenticator_Enabled = model.Enabled;
            settings.GoogleAuthenticator_Name = model.Name;

            CustomDatabase.SaveSettings(settings);

            SettingsCache.ClearCache();
            return true;
        }

        public SMSSettingsModel GetSMSSettings()
        {
            var settings = SettingsCache.Instance;
            var viewModel = new SMSSettingsModel()
            {
                MessageFormat = settings.SMS_MessageFormat,
                Enabled = settings.SMS_Enabled,
                CurrentSMSProvider = settings.SMS_CurrentSMSProvider
            };

            viewModel.SMSProviders = FortressContext.GetAllSmsProviders();



            return viewModel;
        }
        public bool SaveSMSSettings(SMSSettingsModel model)
        {
            var settings = CustomDatabase.GetSettingsFromDatabase();

            settings.SMS_CurrentSMSProvider = model.CurrentSMSProvider;
            settings.SMS_Enabled = model.Enabled;
            settings.SMS_MessageFormat = model.MessageFormat;

            CustomDatabase.SaveSettings(settings);

            SettingsCache.ClearCache();
            return true;
        }


        public SMSProviderSettingsModel GetSMSProviderSettings(string ProviderName)
        {
            var type = Type.GetType(ProviderName);
            var smsAttr = type.GetCustomAttribute<SmsProviderAttribute>();
            var allProviders = FortressContext.GetAllSmsProviders();
            var thisProvider = allProviders.FirstOrDefault(x => x.Classname == ProviderName);
            var settings = SettingsCache.Instance;
            var viewModel = new SMSProviderSettingsModel()
            {
                Name = smsAttr.Name,
                ClassName = thisProvider.Classname
            };
            viewModel.Settings = settings.GetPropertiesOnType(type);

            return viewModel;
        }
        public bool SaveSMSProviderSettings(SMSProviderSettingsModel model)
        {
            var settings = CustomDatabase.GetSettingsFromDatabase();

            settings.SaveSmsProviderSettings(model);
                     
            CustomDatabase.SaveSettings(settings);

            SettingsCache.ClearCache();
            return true;
        }



    }
    public class GoogleAuthenticatorSettingsModel
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
    public class SMSSettingsModel
    {
        public string MessageFormat { get; set; }
        public bool Enabled { get; set; }
        public string CurrentSMSProvider { get; set; }
        public List<SmsProviderViewModel> SMSProviders { get; set; }
    }

    public class SMSProviderSettingsModel
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public List<SMSProviderSettingModel> Settings { get; set; }
    }
    public class SMSProviderSettingModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}