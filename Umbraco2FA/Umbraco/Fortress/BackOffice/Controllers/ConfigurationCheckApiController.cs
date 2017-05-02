using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Orc.Fortress.Attributes;
using Orc.Fortress.BackOffice.Models;
using Orc.Fortress.Cache;
using Orc.Fortress.Database;
using Umbraco.Web;
using Umbraco.Web.Cache;
using System.Configuration;
using System.Web.Configuration;

namespace Orc.Fortress.BackOffice.Controllers
{
    [FortressPluginController]
    public class ConfigurationCheckApiController : BaseFortressBackofficeController
    {
        public ConfigurationCheckerViewModel Check()
        {
            var model = new ConfigurationCheckerViewModel();
            var owinStartup = System.Web.Configuration.WebConfigurationManager.AppSettings["owin:appStartup"];

            if(owinStartup == "FortressOwinStartup")
            {
                model.OwinStartupIsSetup = true;
            }
            else
            {
                model.OwinStartupIsSetup = false;
                if(owinStartup == "UmbracoDefaultOwinStartup")
                {
                    model.OwinStartupIsFixable = true;
                }else
                {
                    model.OwinStartupIsFixable = false;
                }
            }

            //Confirm that the sms message template contains the {0} required for replacment
            var settings = CustomDatabase.GetSettingsFromDatabase();

            if (settings.SMS_MessageFormat.Contains("{0}"))
            {
                model.SmsMessageContainsReplacement = true;
            }else
            {
                model.SmsMessageContainsReplacement = false;
            }
            

            return model;
        }


        public bool FixOwinStartup()
        {
            var owinStartup = System.Web.Configuration.WebConfigurationManager.AppSettings["owin:appStartup"];

            if (owinStartup == "UmbracoDefaultOwinStartup")
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("/");
                config.AppSettings.Settings["owin:appStartup"].Value = "FortressOwinStartup";
                config.Save(ConfigurationSaveMode.Modified);
            }

            return true;
        }
    }
    public class ConfigurationCheckerViewModel
    {
        public bool OwinStartupIsSetup { get; set; }
        public bool OwinStartupIsFixable { get; set; }

        public bool SmsMessageContainsReplacement { get; set; }

    }

}