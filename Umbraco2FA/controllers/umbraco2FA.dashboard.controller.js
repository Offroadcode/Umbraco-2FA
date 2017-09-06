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
 