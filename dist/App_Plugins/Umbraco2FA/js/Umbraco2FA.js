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
                   $http.get('/Umbraco/Fortress/TwoFactorSetup/GetAvailableTwoFactorMethods'),
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
angular.module("umbraco").controller("fortress.setup.controller",
    function ($scope, $cookies, localizationService, userService, externalLoginInfo, resetPasswordCodeInfo, $timeout, authResource,FortressTwoFactorResource,notificationsService ) {
        $scope.view="loading"; 
        $scope.availableProviders = [];
        $scope.smsValidationToken = "";
        $scope.googleAuthenticatorImage;
        $scope.googleAuthenticatorManual;
         FortressTwoFactorResource.GetAvailableMethods().then(function(data){
            $scope.view = "providerSelection";
            $scope.availableProviders = data;
        });

        $scope.selectProvider = function(provider) {
            if(provider == "GoogleAuthenticator"){
                FortressTwoFactorResource.GetGoogleAuthenticatorDetails().then(function(data){
                    $scope.googleAuthenticatorImage = data.image;
                    $scope.googleAuthenticatorManual = data.manualEntryCode;
                    $scope.view = "setup-"+provider;
                });
            }else{
                $scope.view = "setup-"+provider;
            }
        }
        
        $scope.configureSMS = function(phoneNumber){
            FortressTwoFactorResource.SetupSms(phoneNumber).then(function(data){
                $scope.smsValidationToken = data.token;
                $scope.view = "verify-SMSProvider";
            });
        }

        $scope.verifySMS = function(otp){
            authResource.verify2FACode("SMS", otp).then(function(data){
                notificationsService.success("Sms Verification Saved", "Your Two factor authentication settings have been saved!");
                userService.setAuthenticationSuccessful(data);
                $scope.submit(true);
            });
        }

        $scope.configureGoogleAuthenticator= function(){
            FortressTwoFactorResource.SetupGoogleAuthenticator().then(function(data){
                $scope.smsValidationToken = data.token;
                $scope.view = "verify-GoogleAuthenticator";
            });       
        }

        $scope.verifyGoogleAuthenticator= function(otp){
            authResource.verify2FACode("GoogleAuthenticator", otp).then(function(data){
                notificationsService.success("Google Verification Saved", "Your Two factor authentication settings have been saved!");
                userService.setAuthenticationSuccessful(data);
                $scope.submit(true);
            });       
        }
    });

angular.module("umbraco").controller("fortress.smsProvider.controller",
    function ($scope, $cookies, localizationService, userService, externalLoginInfo, resetPasswordCodeInfo, $timeout, authResource,FortressTwoFactorResource,eventsService) {
       
        $scope.view = "loading";
        uProtectTwoFactorResource.SendOTP().then(function(data){
            $scope.token = data.token;
            $scope.lastFourDigits = data.lastFourDigits;
            $scope.view = "submit2FA";
        });

        $scope.submitValidation = function(code){
            uProtectTwoFactorResource.Validate(code,$scope.token).then(function(data){
                userService.setAuthenticationSuccessful(data);
                $scope.submit(true);
            });
        }
    });

angular.module("umbraco").controller("fortress.twoFactorLogin.controller",
    function ($scope, $cookies, localizationService, userService, externalLoginInfo, resetPasswordCodeInfo, $timeout, authResource, dialogService) {

        $scope.code = "";
        $scope.provider = "";
        $scope.providers = [];
        $scope.step = "loading"; 
        

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
                });

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

        /**
         * @method $scope.currentPage
         * @returns {string}
         * @description Determines what sub-page the user is on based on the location hash.
         */
        $scope.currentPage = function() {
            var page = "main";
            if (window.location.hash.indexOf("GoogleAuthenticator") > -1) {
                page = "authenticator";
            } else if (window.location.hash.indexOf("SMS-") > -1) {
                page = "smsProvider";
            } else if (window.location.hash.indexOf("SMS") > -1) {
                page = "sms";
            } 
            return page;
        };

        $scope.getCurrentProvider = function() {
            if ($scope.currentPage() !== "smsProvider") {
                return "";
            }
            return window.location.hash.split("SMS-")[1].split("/")[0];
        };

        $scope.getTab = function() {
            switch ($scope.currentPage()) {
                case "authenticator":
                    $scope.tabs = [{ id: "GoogleAuthenticator", label: "Google Authenticator" }];
                    break;
                case "sms":
                    $scope.tabs = [{ id: "SMS", label: "SMS" }];
                    break;
                case "smsProvider":
                    $scope.tabs = [{ id: "Provider", label: $scope.getCurrentProvider() }];
                    break;
            }
        };

        $scope.loadData = function() {
            $scope.settingsLoaded = false;
            switch($scope.currentPage()) {
                case "smsProvider":
                    $scope.loadProviderSettings().then(function() {
                        $scope.settingsLoaded = true;                       
                    });
                    break;
                default:
                    $scope.loadGoogleAuthenticatorSettings().then(function() {
                        $scope.loadSMSSettings().then(function(){
                            $scope.settingsLoaded = true;
                        });
                    });
                    break;
            };
        };

        $scope.loadGoogleAuthenticatorSettings = function() {
            return $http.get('/umbraco/backoffice/Umbraco2FA/SettingsApi/GetGoogleAuthenticatorSettings').then(function(response) {
                $scope.data.authenticatorSettings = response.data;
            });
        };

        $scope.loadSMSSettings = function() {
            return $http.get('/umbraco/backoffice/Umbraco2FA/SettingsApi/GetSMSSettings').then(function(response) {
                $scope.data.smsSettings = response.data;
            });
        };

        $scope.loadProviderSettings = function() {
            var providerName = $scope.getCurrentProvider();
            return $http.get('/umbraco/backoffice/Umbraco2FA/SettingsApi/GetSMSProviderSettings?ProviderName=' + providerName).then(function(response) {
                $scope.data.smsProviders[providerName] = response.data;
            });
        };
        
        $scope.save = function() {
            $scope.isSaving = true;
            switch($scope.currentPage()) {
                case "smsProvider":
                    $scope.saveProviderSettings().then(function() {
                        notificationsService.success("Two Factor Settings Saved", "Your Two factor settings have been saved!");
                        $scope.isSaving = false;
                        $scope.twoFactorForm.$dirty = false;
                    });
                    break;
                default:
                    $scope.saveGoogleAuthenticatorSettings().then(function() {
                        $scope.saveSMSSettings().then(function() {
                            notificationsService.success("Two Factor Settings Saved", "Your Two factor settings have been saved!");
                            $scope.isSaving = false;
                            $scope.twoFactorForm.$dirty = false;
                        });
                    });
                    break;
            };
        };

        $scope.saveGoogleAuthenticatorSettings = function() {
            return $http.post('/umbraco/backoffice/Umbraco2FA/SettingsApi/SaveGoogleAuthenicatorSettings', $scope.data.authenticatorSettings).then(function(response) {
            });
        };

        $scope.saveProviderSettings = function() {
            var provider = $scope.data.smsProviders[$scope.getCurrentProvider()];
            return $http.post('/umbraco/backoffice/Umbraco2FA/SettingsApi/SaveSMSProviderSettings', provider).then(function(response) {
            });
        };

        $scope.saveSMSSettings = function() {
            return $http.post('/umbraco/backoffice/Umbraco2FA/SettingsApi/SaveSMSSettings', $scope.data.smsSettings).then(function(response) {
            });
        };
 
        $scope.getTab();
        $scope.loadData();
    });
