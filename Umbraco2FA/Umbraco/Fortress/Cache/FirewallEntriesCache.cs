using System;
using System.Collections.Generic;
using Orc.Fortress.Database;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Web.Cache;

namespace Orc.Fortress.Cache
{
    public class FirewallEntriesCache
    {
        internal static FirewallEntriesModel Instance
        {
            get { return ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem<FirewallEntriesModel>(CacheKeys.FirewallCache.Key, getFresh); }
        }

        private static FirewallEntriesModel getFresh()
        {
            var model = new FirewallEntriesModel();
            var db = new FortressDatabase();
            model.BackOfficeBlackList = db.GetAllFirewallEntries(FirewallMode.BlackList, FirewallArea.BackOffice);
            model.BackOfficeWhiteList = db.GetAllFirewallEntries(FirewallMode.WhiteList, FirewallArea.BackOffice);
            model.FrontEndBlackList = db.GetAllFirewallEntries(FirewallMode.BlackList, FirewallArea.FrontEnd);
            model.FrontEndWhiteList= db.GetAllFirewallEntries(FirewallMode.WhiteList, FirewallArea.FrontEnd);
            return model;
        }

        internal static void ClearCache()
        {
            DistributedCache.Instance.RefreshAll(new Guid(CacheKeys.FirewallCache.Key));
        }
    }

    public class FirewallEntriesModel
    {
        internal List<string> BackOfficeWhiteList { get; set; }
        internal List<string> BackOfficeBlackList { get; set; }
        internal List<string> FrontEndWhiteList { get; set; }
        internal List<string> FrontEndBlackList { get; set; }
        
    }
}