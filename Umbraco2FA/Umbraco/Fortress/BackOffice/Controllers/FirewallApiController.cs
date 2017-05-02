using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Orc.Fortress.Attributes;
using Orc.Fortress.BackOffice.Models;
using Orc.Fortress.Cache;
using Orc.Fortress.Database;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Orc.Fortress.BackOffice.Controllers
{
    [FortressPluginController]
    public class FirewallApiController : BaseFortressBackofficeController
    {
        public AllFirewallSettingsViewModel GetSettings()
        {
            AllFirewallSettingsViewModel model = new AllFirewallSettingsViewModel();


            model.BackOffice = FirewallViewModel.BackOffice();

            model.FrontEnd = FirewallViewModel.FrontEnd();

            return model;
        }
        public FirewallFiltersModel GetEntries(int page, FirewallArea area)
        {
            var mode = FirewallMode.Disabled;

            var settings = SettingsCache.Instance;

            if (area == FirewallArea.BackOffice)
            {
                mode = settings.BackofficeFirewallMode;
            }
            else if (area == FirewallArea.FrontEnd)
            {
                mode = settings.FrontEndFirewallMode;
            }

            FirewallFiltersModel model = new FirewallFiltersModel();
            model.Entries = new List<FirewallFilterModel>();
            var entries = CustomDatabase.GetFirewallEntries(page, mode, area);

            foreach (var entry in entries.Items)
            {
                model.Entries.Add(new FirewallFilterModel() { ID = entry.Id, IPAddress = entry.IPAddress });
            }
            model.CurrentPage = entries.CurrentPage;
            model.TotalEntries = entries.TotalItems;
            model.TotalPages = entries.TotalPages;
            return model;
        }
        public AllFirewallSettingsViewModel SaveSettings(AllFirewallSettingsViewModel model)
        {

            var settings = CustomDatabase.GetSettingsFromDatabase();

            settings.BackofficeFirewallMode = model.BackOffice.FirewallMode;
            settings.FrontEndFirewallMode = model.FrontEnd.FirewallMode;

            CustomDatabase.SaveSettings(settings);

             SettingsCache.ClearCache();
            return model;
        }

    }


}