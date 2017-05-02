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
