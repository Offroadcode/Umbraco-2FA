using System.Xml;
using umbraco.interfaces;
using Umbraco.Core.IO;
using Umbraco.Core;
using System.Configuration;
using System.Web.Configuration;
using Umbraco.Core.Logging;

namespace Fortress.Installer
{
    public class OwinStartupInstallAction : IPackageAction
    {
        public string Alias()
        {
            return "OwinStartupInstallAction";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            LogHelper.Debug<OwinStartupInstallAction>("Execute");

            Configuration config = WebConfigurationManager.OpenWebConfiguration("/");
            string oldValue = config.AppSettings.Settings["owin:appStartup"].Value;
            if (oldValue == "UmbracoDefaultOwinStartup")
            {
                LogHelper.Debug<OwinStartupInstallAction>("Setting to FortressOwinStartup");
                config.AppSettings.Settings["owin:appStartup"].Value = "FortressOwinStartup";
                config.Save(ConfigurationSaveMode.Modified);
            }
            return true;
        }

        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"OwinStartupInstallAction\"></Action>";
            XmlDocument x = new XmlDocument();
            x.LoadXml(sample);
            return x;
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            LogHelper.Debug<OwinStartupInstallAction>("Undo");

            Configuration config = WebConfigurationManager.OpenWebConfiguration("/");
            config.AppSettings.Settings["owin:appStartup"].Value = "UmbracoDefaultOwinStartup";
            config.Save(ConfigurationSaveMode.Modified);
            return true;
        }
    }
}