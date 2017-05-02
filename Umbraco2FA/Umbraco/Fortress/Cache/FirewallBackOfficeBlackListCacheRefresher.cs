using System;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Orc.Fortress.Cache
{
    public class FirewallEntriesCacheRefresher : TypedCacheRefresherBase<FirewallEntriesCacheRefresher, FirewallEntriesModel>, ICacheRefresher
    {
        protected override FirewallEntriesCacheRefresher Instance
        {
            get { return this; }
        }

        public override Guid UniqueIdentifier
        {
            get { return CacheKeys.FirewallCache.RefresherGuid; }
        }

        public override string Name
        {
            get { return "Fortress FirewallEntries Cache"; }
        }

        public override void RefreshAll()
        {
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(CacheKeys.FirewallCache.Key);
            base.RefreshAll();
        }
    }
}