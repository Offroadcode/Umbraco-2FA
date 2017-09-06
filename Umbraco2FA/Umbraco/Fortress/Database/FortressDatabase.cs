using System;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Orc.Fortress.Database.Models;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using Umbraco.Web.Cache;
using System.Collections.Generic;
using Orc.Fortress;

namespace Orc.Fortress.Database
{
    public class FortressDatabase
    {
        public FortressUser2FASettings GetUserDetails(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var results =
                db.FirstOrDefault<FortressUser2FASettings>("SELECT * FROM fortressUser2FASettings WHERE UserId = @0", id);

            return results;
        }

        public void Update(FortressUser2FASettings details)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            db.Update(details);
        }

        public void Insert(object details)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            db.Insert(details);
        }

        public FortressSettings GetSettingsFromDatabase()
        {
                var db = ApplicationContext.Current.DatabaseContext.Database;

                var results = db.Fetch<FortressSettingEntry>("SELECT * FROM FortressSettings");
                var model = new FortressSettings(results);
            return model;
        }

        public void SaveSettings(FortressSettings settings)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var data = settings.GetRawData().Select(x=>x.Value);
            var currentDatabaseSettings = GetSettingsFromDatabase();
            var currentData = currentDatabaseSettings.GetRawData();
            foreach (var fortressSettingEntry in data)
            {
                if (currentData.ContainsKey(fortressSettingEntry.Key)){
                    db.Update(fortressSettingEntry);
                }else
                {
                    db.Insert(fortressSettingEntry);
                }
            }
            
        }
        
    }
}