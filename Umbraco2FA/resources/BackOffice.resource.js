//adds the resource to umbraco.resources module:
angular.module('umbraco.resources').factory('FortressBackOfficeResource', 
    function($q, $http,umbRequestHelper) {
        
        return {
            GetLogEntries: function (page, ipAddressFilter, userName) {
                var url = '/umbraco/backoffice/Umbraco2FA/LogsApi/GetData?page='+page;
                if(ipAddressFilter && ipAddressFilter !="") {
                    url = url+"&ipAddressFilter="+ipAddressFilter;
                }
                if(userName && userName !="") {
                    url = url+"&userName="+userName;
                }
                return umbRequestHelper.resourcePromise(
                   $http.get(url),
                   'Failed to Get Logs');
            }, 

            GetLockedAccounts:function() {
                var url = '/umbraco/backoffice/Umbraco2FA/LockedAccountsApi/GetData';
                return umbRequestHelper.resourcePromise(
                   $http.get(url),
                   'Failed to Get Locked Accounts');
            },

            UnlockAccount:function(id) {
                var url = '/umbraco/backoffice/Umbraco2FA/LockedAccountsApi/UnlockUser?id='+id;
                return umbRequestHelper.resourcePromise(
                   $http.post(url),
                   'Failed to unlock Account');
            },

            GetFirewallSettings:function() {
                var url = '/umbraco/backoffice/Umbraco2FA/FirewallApi/GetSettings';
                return umbRequestHelper.resourcePromise(
                   $http.get(url),
                   'Failed to Get Locked Accounts');
            },

            GetEntries:function(page,area) {
                var url = '/umbraco/backoffice/Umbraco2FA/FirewallApi/GetEntries?page='+page+'&area='+area;
                return umbRequestHelper.resourcePromise(
                   $http.get(url),
                   'Failed to Get Locked Accounts');
            },

            SaveFirewallData:function(data) {
                var url = '/umbraco/backoffice/Umbraco2FA/FirewallApi/SaveSettings';
                return umbRequestHelper.resourcePromise(
                   $http.post(url,data),
                   'Failed to unlock Account');
            },
        };
    }
); 