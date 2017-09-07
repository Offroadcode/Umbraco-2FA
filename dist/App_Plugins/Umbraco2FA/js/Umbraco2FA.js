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
//adds the resource to umbraco.resources module:
angular.module('umbraco.resources').factory('FortressTwoFactorResource', 
    function($q, $http,umbRequestHelper) {
        
        return {
            SendOTP: function () {
                return umbRequestHelper.resourcePromise(
                   $http.post('/Umbraco/Umbraco2FA/TwoFactorAuth/Generate'),
                   'Failed to requestSms');
            },
            
            Validate: function (code, token) {
                return umbRequestHelper.resourcePromise(
                   $http.post('/Umbraco/Umbraco2FA/TwoFactorAuth/Validate?code='+code+'&token='+token),
                   'Failed to Validate');
            },

            
            GetAvailableMethods: function () {
                return umbRequestHelper.resourcePromise(
                   $http.get('/Umbraco/Umbraco2FA/TwoFactorSetup/GetAvailableTwoFactorMethods'),
                   'Failed to GetAvailableMethods');
            },
             
            SetupSms: function (phoneNumber) {
                return umbRequestHelper.resourcePromise(
                   $http.post('/Umbraco/Umbraco2FA/TwoFactorSetup/SetupSMS?number='+phoneNumber),
                   'Failed to SetupSms');
            },

            GetGoogleAuthenticatorDetails:function(){
                return umbRequestHelper.resourcePromise(
                    $http.post('/Umbraco/Umbraco2FA/TwoFactorSetup/SetupGoogleAuth'),
                    'Failed to SetupGoogleAuth');
            }
        };
    }
); 
angular.module('umbraco').controller('fortress.edit.controller', function ($scope, $routeParams) {
    $scope.save = function() {
 
    };
}); 
angular.module('umbraco').controller('fortress.dashboard.controller', function($scope, $routeParams,$http) {

    $scope.currentPage = "home"; // home, setupWizard
    
       $scope.data = null;
       $scope.isLoading = true;
       $scope.settingsLoaded = false;

       $scope.setupWizard = {googleAuthenticatorImage:"",googleAuthenticatorManual:"",lastDidFail: false};
       $scope.loadMySettings = function() {
                              $scope.isLoading = true;
            return $http.get('/umbraco/backoffice/Umbraco2FA/UserSettingsApi/GetMySettings').then(function(response) {
                $scope.data = response.data;
                   $scope.settingsLoaded = true;
                   $scope.isLoading = false;
            });
        };
        $scope.setupAccount= function(){
            $scope.isLoading = true
            return $http.post('/umbraco/backoffice/Umbraco2FA/UserSettingsApi/SetupAuthenticator').then(function(response) {
                $scope.setupWizard.googleAuthenticatorImage = response.data.Image;
                $scope.setupWizard.googleAuthenticatorManual = response.data.ManualEntryCode;
                $scope.currentPage = "setupWizard";
                $scope.isLoading = false
            });
        };
        $scope.removeAuthenticator= function(){
            $scope.isLoading = true
            return $http.post('/umbraco/backoffice/Umbraco2FA/UserSettingsApi/RemoveTwoFactor').then(function(response) {
                $scope.data = response.data;
                $scope.currentPage = "home";
                $scope.isLoading = false
            });
        };
        $scope.verifyGoogleAuthenticator= function(otp){
            $scope.isLoading = true
            return $http.post('/umbraco/backoffice/Umbraco2FA/UserSettingsApi/ValidateGoogleAuthSetup?twoFactorCode='+otp).then(function(response) {
                console.log("verifiy repsonse@:", response);
                if(response.data.IsValid === true){
                    console.log("1");
                    $scope.data = response.data.Settings;
                    $scope.currentPage = "home";
                    $scope.isLoading = false;
                }else{
                    console.log("2");                    $scope.isLoading = false;

                       $scope.currentPage = "setupWizard";
                        $scope.setupWizard.lastDidFail = true;
                }
                
            });
        };
        $scope.loadMySettings();
}); 
 
angular.module("umbraco").controller("fortress.twoFactorLogin.controller",
    function ($scope, $cookies, localizationService, userService, externalLoginInfo, resetPasswordCodeInfo, $timeout, authResource, dialogService) {

        $scope.code = "";
        $scope.provider = "";
        $scope.providers = [];
        $scope.step = "loading"; 
        $scope.didFail = false;
        $scope.errorMsg="";
        authResource.get2FAProviders()
            .then(function (data) {
                var provider = data[0];
                $scope.provider = provider;
                authResource.send2FACode(provider)
                .then(function () {
                    $scope.step = "code";
                });
            }
        );


        $scope.validate = function (provider, code) {
            $scope.code = code;
            authResource.verify2FACode(provider, code)
                .then(function (data) {
                    userService.setAuthenticationSuccessful(data);
                    $scope.submit(true);
                }, 
                function(reason) {
                    console.log("didFail", reason);
                    $scope.didFail = true;
                    $scope.errorMsg = reason.errorMsg;
                }
            );
        };
    });
angular.module("umbraco").controller("fortress.twofactor.controller",
    function ($scope, $cookies, $http, userService, notificationsService, FortressBackOfficeResource) {

        $scope.isSaving = false;
        $scope.settingsLoaded = false;
        $scope.data = {
            smsProviders: {}
        };
        $scope.tabs = [];

        
        $scope.getTab = function() {
            $scope.tabs = [{ id: "GoogleAuthenticator", label: "Google Authenticator" }];
        };

        $scope.loadData = function() {
            $scope.settingsLoaded = false;
         
            $scope.loadGoogleAuthenticatorSettings().then(function() {
                    $scope.settingsLoaded = true;
            });
         
        };

        $scope.loadGoogleAuthenticatorSettings = function() {
            return $http.get('/umbraco/backoffice/Umbraco2FA/SettingsApi/GetGoogleAuthenticatorSettings').then(function(response) {
                $scope.data.authenticatorSettings = response.data;
            });
        };

        $scope.save = function() {
            $scope.isSaving = true;
            
            $scope.saveGoogleAuthenticatorSettings().then(function() {
                    notificationsService.success("Two Factor Settings Saved", "Your Two factor settings have been saved!");
                    $scope.isSaving = false;
                    $scope.twoFactorForm.$dirty = false;
            });
                   
        };

        $scope.saveGoogleAuthenticatorSettings = function() {
            return $http.post('/umbraco/backoffice/Umbraco2FA/SettingsApi/SaveGoogleAuthenicatorSettings', $scope.data.authenticatorSettings).then(function(response) {
            });
        };

        $scope.removeAuthenticatorFromUser=function(id){
            $scope.isSaving = true;
            return $http.post('/umbraco/backoffice/Umbraco2FA/SettingsApi/RemoveGoogleAuthenticatorForUser?id='+id).then(function(response) {
                $scope.isSaving = false;
                   $scope.data.authenticatorSettings = response.data;
            });
        };
        $scope.getTab();
        $scope.loadData();
    });
