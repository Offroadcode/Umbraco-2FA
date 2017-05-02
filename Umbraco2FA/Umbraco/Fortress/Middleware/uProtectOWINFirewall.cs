using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Orc.Fortress.Cache;
using Orc.Fortress.Database;
using Umbraco.Core;

namespace Orc.Fortress
{
    public class FortressOWINFirewall : OwinMiddleware
    {
        public FortressOWINFirewall(
            OwinMiddleware next,
            Microsoft.Owin.Logging.ILogger logger)
            : base(next)
        {

        }
        private FortressDatabase _customDatabase;

        protected FortressDatabase CustomDatabase
        {

            get
            {
                return _customDatabase ?? (_customDatabase = new FortressDatabase());
            }
        }
        public override async Task Invoke(IOwinContext context)
        {
            var settings = SettingsCache.Instance;
            if (IsBackendRequest(context))
            {
                switch (settings.BackofficeFirewallMode)
                {
                    case FirewallMode.WhiteList:
                        if (!IsIPAddressMatchedInList(context.Request.RemoteIpAddress, FirewallEntriesCache.Instance.BackOfficeWhiteList))
                        {
                            await context.Response.WriteAsync("Access Denied as IP address is not in WhiteList");
                            context.Response.StatusCode = 403;
                            return;
                        }

                        break;
                    case FirewallMode.BlackList:
                        if (IsIPAddressMatchedInList(context.Request.RemoteIpAddress, FirewallEntriesCache.Instance.BackOfficeBlackList))
                        {
                            await context.Response.WriteAsync("Access Denied as IP address is in BlackList");
                            context.Response.StatusCode = 403;
                            return;
                        }
                        break;
                }
            }
            else
            {
                switch (settings.FrontEndFirewallMode)
                {
                    case FirewallMode.WhiteList:
                        if (!IsIPAddressMatchedInList(context.Request.RemoteIpAddress, FirewallEntriesCache.Instance.FrontEndWhiteList))
                        {
                            await context.Response.WriteAsync("Access Denied as IP address is not in WhiteList");
                            context.Response.StatusCode = 403;
                            return;
                        }

                        break;
                    case FirewallMode.BlackList:
                        if (IsIPAddressMatchedInList(context.Request.RemoteIpAddress, FirewallEntriesCache.Instance.FrontEndBlackList))
                        {
                            await context.Response.WriteAsync("Access Denied as IP address is in BlackList");
                            context.Response.StatusCode = 403;
                            return;
                        }
                        break;
                }

            }

            //allowAccess
            if (Next != null)
            {
                await Next.Invoke(context);
            }
        }

        private bool IsBackendRequest(IOwinContext context)
        { 
            
            var requestPath = context.Request.Path.Value;

            return 
                requestPath.StartsWith("/App_Plugins/", StringComparison.InvariantCultureIgnoreCase) 
                || requestPath.StartsWith("/umbraco/", StringComparison.InvariantCultureIgnoreCase) 
                || requestPath.InvariantEquals("/umbraco");
        }

        private static bool IsIPAddressMatchedInList(string remoteIpAddress, List<string> backOfficeWhiteList)
        {
            
            return backOfficeWhiteList.Contains(remoteIpAddress);
        }
    }
}
