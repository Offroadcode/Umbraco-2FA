using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orc.Fortress.Cache
{
    internal class CacheKeys
    {
        internal static class SettingsCache
        {
            internal static readonly Guid RefresherGuid = new Guid("8059D6A2-E285-478F-86F2-D2A811087483");
            internal static readonly string Key = "uProtect_Settings";
        }
        internal static class FirewallCache
        {
           
            internal static readonly Guid RefresherGuid = new Guid("482FFB9C-53B1-4D34-855D-0E72ADB209B4");
            internal static readonly string Key = "uProtect_Firewal_Filters";
               
        }
    }
}
