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
                    nodes.Add(CreateTreeNode("GoogleAuthenticator", id, queryStrings, "Google Authenticator", "icon-iphone", false, MainRoute + "/TwoFactor/GoogleAuthenticator"));
                    break;
            }
            return nodes;
        }

      

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return null;
        }
    }
}
