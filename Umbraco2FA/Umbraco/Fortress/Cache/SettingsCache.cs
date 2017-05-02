using Orc.Fortress.Database;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Web.Cache;

namespace Orc.Fortress.Cache
{
    public class SettingsCache
    {
        internal static FortressSettings Instance
        {
            get { return ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem<FortressSettings>(CacheKeys.SettingsCache.Key, getFresh); }
        }

        private static FortressSettings getFresh()
        {
            var db = new FortressDatabase();
            return db.GetSettingsFromDatabase();
        }

        internal static void ClearCache()
        {
            DistributedCache.Instance.RefreshAll(CacheKeys.SettingsCache.RefresherGuid);
        }
    }
}