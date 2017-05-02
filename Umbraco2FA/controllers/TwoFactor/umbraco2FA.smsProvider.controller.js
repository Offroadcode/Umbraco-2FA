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
