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
