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
