using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Orc.Fortress.Attributes;
using Orc.Fortress.BackOffice.Models;
using Umbraco.Web;

namespace Orc.Fortress.BackOffice.Controllers
{
    [FortressPluginController]
    public class LockedAccountsApiController : BaseFortressBackofficeController
    {
        public LockedAccountsViewModel GetData()
        {
            var model = new LockedAccountsViewModel();

            int totalRecords = 0;
            var users = ApplicationContext.Services.UserService.GetAll(0, 9999, out totalRecords);
            model.Entries = new List<LockedAccountViewModel>();
            foreach (var user in users)
            {
                //UserManager.IsLockedOut(user.Id);
                if (user.IsLockedOut)
                {
                    var userModel = new LockedAccountViewModel();
                    userModel.ID = user.Id;
                    userModel.Name = user.Name;
                    model.Entries.Add(userModel);
                }
            }
            
            return model;
        }

        public bool UnlockUser(int id)
        {
            var user = ApplicationContext.Services.UserService.GetUserById(id);
            user.IsLockedOut = false;
            ApplicationContext.Services.UserService.Save(user);
            return true;
        } 
    }
}