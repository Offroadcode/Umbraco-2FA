using Orc.Fortress;
using Umbraco.Web.Mvc;

namespace Orc.Fortress.Attributes
{
    public class FortressPluginControllerAttribute : PluginControllerAttribute
    {
        public FortressPluginControllerAttribute() : base(FortressConstants.UmbracoApplication.ApplicationAlias)
        {
        }
    }
}