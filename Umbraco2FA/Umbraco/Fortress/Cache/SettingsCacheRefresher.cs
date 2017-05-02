using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orc.Fortress.Database;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Orc.Fortress.Cache
{
    public class SettingsCacheRefresher : TypedCacheRefresherBase<SettingsCacheRefresher, FortressSettings>, ICacheRefresher
    {
        protected override SettingsCacheRefresher Instance
        {
            get { return this; }
        }
   
        public override Guid UniqueIdentifier
        {
            get { return CacheKeys.SettingsCache.RefresherGuid; }
        }

        public override string Name
        {
            get { return "Fortress Settings Cache"; }
        }

        public override void RefreshAll()
        {
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(CacheKeys.SettingsCache.Key);
            base.RefreshAll();
        }
    }


}
