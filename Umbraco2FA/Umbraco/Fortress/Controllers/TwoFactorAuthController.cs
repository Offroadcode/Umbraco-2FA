using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Orc.Fortress.Attributes;
using umbraco;
using Umbraco.Web;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.WebApi.Filters;

namespace Orc.uProtect.Controllers
{
    [FortressPluginController]
    public class TwoFactorAuthController : BaseFortressAuthController
    {
        [SetAngularAntiForgeryTokens]
        public HttpResponseMessage Generate()
        {
            var user = SignInManager.GetVerifiedUserId();
            var details = CustomDatabase.GetUserDetails(user);
            var token = UserManager.GenerateTwoFactorToken(user, details.Provider);

            object responseDetails;
            if (details.Provider == "SMSProvider")
            {
                responseDetails =
                    new
                    {
                        token,
                        lastFourDigits = details.Configuration.Substring(details.Configuration.Length - 4)
                    };
            }
            else
            {
                responseDetails = new {token};
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, responseDetails);
            return response;
        }

        [HttpPost]
        [SetAngularAntiForgeryTokens]
        public HttpResponseMessage Validate(string code, string token)
        {
            var userId = SignInManager.GetVerifiedUserId();
            var details = CustomDatabase.GetUserDetails(userId);

            var result = SignInManager.TwoFactorSignIn(details.Provider, code + ":" + token, true, true);

            switch (result)
            {
                case SignInStatus.Success:
                    if (!details.IsValidated)
                    {
                        details.IsValidated = true;
                        CustomDatabase.Update(details);
                    }
                    //get the user
                    var user = ApplicationContext.Services.UserService.GetUserById(SignInManager.GetVerifiedUserId());
                    var userDetail = Mapper.Map<UserDetail>(user);

                    //update the userDetail and set their remaining seconds
                    userDetail.SecondsUntilTimeout = TimeSpan.FromMinutes(GlobalSettings.TimeOutInMinutes).TotalSeconds;

                    //create a response with the userDetail object
                    var response = Request.CreateResponse(HttpStatusCode.OK, userDetail);

                    //ensure the user is set for the current request
                    Request.SetPrincipalForRequest(user);

                    return response;
                    break;
                case SignInStatus.LockedOut:
                case SignInStatus.Failure:
                default:
                    //return BadRequest (400), we don't want to return a 401 because that get's intercepted 
                    // by our angular helper because it thinks that we need to re-perform the request once we are
                    // authorized and we don't want to return a 403 because angular will show a warning msg indicating 
                    // that the user doesn't have access to perform this function, we just want to return a normal invalid msg.            
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                    break;
            }
        }
    }

    public class AvailableTwoFactorMethodsViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}