using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Orc.Fortress.Cache;
using Orc.Fortress.Database;
using Orc.Fortress.Database.Models;
using Umbraco.Core.Persistence;

namespace Orc.Fortress.BackOffice.Models
{
    public class AllFirewallSettingsViewModel
    {
        public FirewallViewModel BackOffice{ get; set; }   
        public FirewallViewModel FrontEnd { get; set; }
    }
    public class FirewallViewModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FirewallMode FirewallMode { get; set; }

        public static FirewallViewModel BackOffice()
        {
            var model = new FirewallViewModel();
            model.FirewallMode = SettingsCache.Instance.BackofficeFirewallMode;

      
            return model;
        }
        public static FirewallViewModel FrontEnd()
        {
            var model = new FirewallViewModel();
            model.FirewallMode = SettingsCache.Instance.FrontEndFirewallMode;
        
            return model;
        }

        public long EntriesTotalPages { get; set; }

        public long EntriesCurrentPage { get; set; }
    }

    public class FirewallFiltersModel : _BasePagedViewModel<FirewallFilterModel>
    {
       
    }
    public class FirewallFilterModel
    {
        public int ID { get; set; }
        public string IPAddress { get; set; }
    }
}