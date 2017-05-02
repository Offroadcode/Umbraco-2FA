using System.Net.Http.Formatting;
using Orc.Fortress.Attributes;
using umbraco.businesslogic;
using umbraco.interfaces;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Orc.Fortress.BackOffice
{
    [Application(FortressConstants.UmbracoApplication.ApplicationAlias, FortressConstants.UmbracoApplication.ApplicationName, "icon-firewall", 15)]
    public class FortressSectionApplication : IApplication
    {
    }

   
}