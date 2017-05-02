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