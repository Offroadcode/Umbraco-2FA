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

        public void AddNewSignInEvent(string userName, SignInStatus result, bool wasFromTwoFactorPage, string ipAddress)
        {
            int? userId = null;
            var profile = ApplicationContext.Current.Services.UserService.GetByUsername(userName);
            if (profile != null)
            {
                userId = profile.Id;
            }
            AddNewSignInEvent(userName, userId, result, wasFromTwoFactorPage, ipAddress);
        }

        public void AddNewSignInEvent(int id, SignInStatus result, bool wasFromTwoFactorPage, string ipAddress)
        {
            var userName = "";
            var profile = ApplicationContext.Current.Services.UserService.GetUserById(id);
            if (profile != null)
            {
                userName = profile.Username;
            }
            AddNewSignInEvent(userName, id, result, wasFromTwoFactorPage, ipAddress);
        }

        public void AddNewSignInEvent(string userName, int? userId, SignInStatus result, bool wasFromTwoFactorPage, string ipAddress)
        {

            var hostName = "";

            var host = Dns.GetHostEntry(ipAddress);
            if (host != null)
            {
                hostName = host.HostName;
            }
       
            Insert(new FortressLoginEvent
            {
                Date = DateTime.UtcNow,
                UserName = userName,
                Status = result.ToString(),
                UserId = userId,
                WasFromTwoFactorStage = wasFromTwoFactorPage,
                IpAddress = ipAddress,
                Hostname = hostName
            });
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

        /*public bool IsAccountLockedOut(string username)
        {
            var settings = GetSettings();

            var db = ApplicationContext.Current.DatabaseContext.Database;

            if (settings.EnableAccountLockout)
            {
                var lastSuccessful = GetLastSuccessfullLogin(username);
                var cutoff = DateTime.UtcNow.AddMinutes(settings.AccountLockoutDuration*-1);
                if (lastSuccessful.HasValue && cutoff < lastSuccessful)
                {
                    cutoff = lastSuccessful.Value;
                }

                var sql = @"SELECT count(*)
                      FROM [propertyPrefix + "LoginEvents]
                      Where Date > @0
                      and Status = 'Failure'
                      and UPPER(UserName) =@1";
                var concurrentFailures = db.FirstOrDefault<int>(sql, cutoff, username.ToUpper());

                if (concurrentFailures >= settings.AccountLockoutNumberOfAttempts)
                {
                    return true;
                }
            }
            return false;
        }*/

        /* private DateTime? GetLastSuccessfullLogin(string username)
         {
             var db = ApplicationContext.Current.DatabaseContext.Database;


             var sql = @"SELECT TOP 1
                       [Date]
                   FROM [FortressTestSite].[dbo].[FortressLoginEvents]
                   where UPPER(UserName) =@0
                   and Status='Success'
                   Order BY Date desc";
             var lastSuccessfulLogin = db.FirstOrDefault<DateTime?>(sql, username.ToUpper());
             return lastSuccessfulLogin;
         }*/

        public Page<FortressLoginEvent> GetLogEntries(long page, long itemsPerPage, string ipAddress,string userName)
        {
            
            var query = new Sql().Select("*").From(FortressConstants.TableNames.FortressLoginEvents);

            bool IsFirstWhereClause = true;
            if (!string.IsNullOrEmpty(ipAddress))
            {
                WhereOrAnd(query, ref IsFirstWhereClause);
                query.Append(" IpAddress LIKE @0%", ipAddress+"%");
            }

            if (!string.IsNullOrEmpty(userName))
            {
                WhereOrAnd(query, ref IsFirstWhereClause);
                query.Append(" UserName LIKE @0", userName+"%");
            }


            query.OrderBy("Date desc");

            var db = ApplicationContext.Current.DatabaseContext.Database;
            var pageDetails = db.Page<FortressLoginEvent>(page,itemsPerPage,query);

            return pageDetails;
        }

        private static void WhereOrAnd(Sql query, ref bool isFirstWhereClause)
        {
            if (isFirstWhereClause)
            {
                query.Append("WHERE");
                isFirstWhereClause = false;
            }
            else
            {
                query.Append("AND");
            }
        }

        public Page<FortressFirewallEntry> GetFirewallEntries(int page, FirewallMode firewallMode, FirewallArea area)
        {
            var query = new Sql().Select("*").From(FortressConstants.TableNames.FortressFirewallEntry);


            query.Append(" WHERE Area =@0", area.ToString());
            query.Append(" AND FirewallMode =@0", firewallMode.ToString());

            query.OrderBy("Id asc");
            
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var pageDetails = db.Page<FortressFirewallEntry>(page, 50, query);

            return pageDetails;
        }
        public List<String> GetAllFirewallEntries(FirewallMode firewallMode, FirewallArea area)
        {
            var query = new Sql().Select("IPAddress").From(FortressConstants.TableNames.FortressFirewallEntry);


            query.Append(" WHERE Area =@0", area.ToString());
            query.Append(" AND FirewallMode =@0", firewallMode.ToString());

            var db = ApplicationContext.Current.DatabaseContext.Database;
            var pageDetails = db.Fetch<string>(query);

            return pageDetails;
        }
    }
}