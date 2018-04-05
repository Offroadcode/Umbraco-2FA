using Orc.Fortress.Database;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Web.WebApi;

namespace Orc.Fortress.BackOffice.Controllers
{
    public class BaseFortressBackofficeController : UmbracoAuthorizedApiController
    {
        private FortressDatabase _customDatabase;

        private BackOfficeSignInManager _signInManager;

        private BackOfficeUserManager<BackOfficeIdentityUser> _userManager;

        protected new  BackOfficeUserManager<BackOfficeIdentityUser> UserManager
        {
            get { return _userManager ?? (_userManager = TryGetOwinContext().Result.GetBackOfficeUserManager()); }
        }

        protected BackOfficeSignInManager SignInManager
        {
            get { return _signInManager ?? (_signInManager = TryGetOwinContext().Result.GetBackOfficeSignInManager()); }
        }

        protected FortressDatabase CustomDatabase
        {
            get
            {
                return _customDatabase ?? (_customDatabase = new FortressDatabase());
            }
        }
    }
}