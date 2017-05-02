using System.Xml;
using umbraco.interfaces;
using Umbraco.Core.IO;
using Umbraco.Core;

namespace Fortress.Installer
{
    public class FortressDashboardAction : IPackageAction
    {
        private const string startupSectionXPth = "//section[@alias='FortressDashboardSection']";
        //  we assume that no tab with same caption attribute value exists, if it does then do nothing!
        private const string welcomeTabXPath = "//section[@alias='FortressDashboardSection']/tab[@caption='Umbraco2FA']";


        public string Alias()
        {
            return "fortressDashboard";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            string xmlTab = "<data name=\"DashboardWelcomeTab\" xml:space=\"preserve\"><value>&lt;tab caption = \"Umbraco2FA\"&gt;&lt;control showOnce = \"false\" addPanel = \"false\" panelCaption = \"\"&gt;/App_Plugins/Umbraco2FA/backoffice/edit.html&lt;/control&gt;&lt;/tab&gt;</value></data>";

            string dbConfig = SystemFiles.DashboardConfig;
            XmlDocument dashboardFile = XmlHelper.OpenAsXmlDocument(dbConfig);

            XmlNode existingWelcomeTab = dashboardFile.SelectSingleNode(welcomeTabXPath);

            if (existingWelcomeTab == null)
            {
                XmlNode section = dashboardFile.SelectSingleNode(startupSectionXPth);

                XmlDocumentFragment xfrag = dashboardFile.CreateDocumentFragment();
                xfrag.InnerXml = xmlTab;

                section.PrependChild(xfrag);

                dashboardFile.Save(IOHelper.MapPath(dbConfig));
            }

            return true;

        }


        public bool Undo(string packageName, XmlNode xmlData)
        {

            string dbConfig = SystemFiles.DashboardConfig;
            XmlDocument dashboardFile = XmlHelper.OpenAsXmlDocument(dbConfig);

            XmlNode section = dashboardFile.SelectSingleNode(welcomeTabXPath);

            if (section != null)
            {

                dashboardFile.SelectSingleNode(startupSectionXPth).RemoveChild(section);
                dashboardFile.Save(IOHelper.MapPath(dbConfig));
            }

            return true;
        }


        public XmlNode SampleXml()
        {
            var xml = "<Action runat=\"install\" undo=\"true\" alias=\"fortressDashboard\" />";
            XmlDocument x = new XmlDocument();
            x.LoadXml(xml);
            return x;
        }
    }
}