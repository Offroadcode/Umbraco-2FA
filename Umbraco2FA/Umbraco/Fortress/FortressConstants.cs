using System;

namespace Orc.Fortress
{
    internal class FortressConstants
    {
        internal static readonly Guid Licence = new Guid("72dfbf06-622e-48c7-80dc-56dc64b6a0a6");
        internal static readonly string LockoutItemKey = "ORC_AccountIsLockedOut";

        internal static string OTPSessionKey = "ORC_OTP";

        internal static class TableNames
        {
            internal const string FortressUser2FASettings = "FortressUser2FASettings";
            internal const string FortressLoginEvents = "FortressLoginEvents";
            internal const string FortressSettings = "FortressSettings";
            internal const string FortressFirewallEntry = "FortressFirewallEntry";
        }

        internal static class UmbracoApplication
        {
            
            internal const string ApplicationAlias = "umbraco2FA";
            internal const string ApplicationName = "Umbraco2FA";
            internal const string TreeAlias = "fortressTree";
            internal const string TreeName = "Umbraco2FA";
        }
    }
}