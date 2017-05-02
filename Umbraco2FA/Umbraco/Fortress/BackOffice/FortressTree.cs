using System;
using System.Net.Http.Formatting;
using Orc.Fortress.Attributes;
using umbraco.businesslogic;
using umbraco.interfaces;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;


namespace Orc.Fortress.BackOffice
{
    [FortressPluginController]
    [Umbraco.Web.Trees.Tree(FortressConstants.UmbracoApplication.ApplicationAlias, FortressConstants.UmbracoApplication.TreeAlias, FortressConstants.UmbracoApplication.TreeName, "icon-doc")]
    public class FortressSectionTreeController : TreeController
    {
        private const string MainRoute =
            FortressConstants.UmbracoApplication.ApplicationAlias + "/" + FortressConstants.UmbracoApplication.TreeAlias;

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();


            switch (id)
            {
                case "-1":

                    //nodes.Add(CreateTreeNode("logs", id, queryStrings, "Logs", "icon-bar-chart", false, MainRoute + "/logs/0"));
                    //nodes.Add(CreateTreeNode("lockedAccounts", id, queryStrings, "Locked Accounts", "icon-lock", false, MainRoute + "/lockedAccounts/0"));
                    
                    //nodes.Add(CreateTreeNode("settings", id, queryStrings, "Settings", "icon-settings-alt", true));
                    //break;
                //case "settings":
                    //nodes.Add(CreateTreeNode("firewall", id, queryStrings, "Firewall", "icon-multiple-windows", false, MainRoute + "/firewall/0"));
                    //nodes.Add(CreateTreeNode("TwoFactor", id, queryStrings, "Two Factor", "icon-conversation-alt", true, MainRoute + "/TwoFactor/0"));
                    //nodes.Add(CreateTreeNode("Lockout", id, queryStrings, "Lockout", "icon-settings", false));
                    //break;
                //case "TwoFactor":
                    nodes.Add(CreateTreeNode("GoogleAuthenticator", id, queryStrings, "Google Authenticator", "icon-iphone", false, MainRoute + "/TwoFactor/GoogleAuthenticator"));
                    nodes.Add(CreateTreeNode("SMS", id, queryStrings, "SMS", "icon-message", true, MainRoute + "/TwoFactor/SMS"));
                    break;
                case "SMS": 
                    AddSmsProviders(ref nodes, id, queryStrings);
                    break;
            }
            return nodes;
        }

        private void AddSmsProviders(ref TreeNodeCollection nodes, string id, FormDataCollection queryStrings)
        {
            FortressContext.Initialize();
            var providers = FortressContext.GetAllSmsProviders();
            foreach (var provider in providers)
            {
                nodes.Add(CreateTreeNode(provider.Classname, id, queryStrings, provider.Name, "icon-notepad", false, MainRoute + "/TwoFactor/SMS-"+provider.Classname));
            }
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return null;
        }
    }
}
