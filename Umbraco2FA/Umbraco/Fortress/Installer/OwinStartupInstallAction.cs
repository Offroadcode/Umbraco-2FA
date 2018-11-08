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
            return true;
        }
    }
}