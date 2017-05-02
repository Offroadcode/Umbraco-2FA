using Orc.Fortress.Database;
using Orc.Fortress.Models;
using Orc.Fortress.SMSProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace Orc.Fortress
{
    internal class FortressContext
    {
        public static void Initialize()
        {
            var baseSmsType = typeof(BaseSMSProvider);
            SmsProviders = new List<SmsProviderViewModel>();
            var types = TypeFinder.FindClassesOfType<BaseSMSProvider>().ToList();

            foreach (var type in types)
            {
                if (type.IsSubclassOf(baseSmsType))
                {
                    var smsAttr = type.GetCustomAttribute<SmsProviderAttribute>();
                    if (smsAttr != null)
                    {
                        var model = new SmsProviderViewModel();
                        model.Classname = type.FullName;
                        model.Name = smsAttr.Name;
                        model.Description = smsAttr.Description;
                        SmsProviders.Add(model);
                    }
                }
            }

        }
        private static List<SmsProviderViewModel> SmsProviders = new List<SmsProviderViewModel>();
        public static BaseSMSProvider GetCurrentSmsProvider()
        {
            var providerType = Cache.SettingsCache.Instance.GetSmsProviderType();
            var SmsService = Activator.CreateInstance(providerType);

            BaseSMSProvider baseSmsService = (BaseSMSProvider)SmsService;
            Cache.SettingsCache.Instance.PopulateSmsPropertiesOnObject(baseSmsService);

            return baseSmsService;

        }
        public static List<SmsProviderViewModel> GetAllSmsProviders()
        {
            return SmsProviders;
        }
    }
}
