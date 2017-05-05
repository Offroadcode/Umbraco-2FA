angular.module('umbraco').controller('fortress.dashboard.controller', function($scope, $routeParams) {

    $scope.hackNames = function() {
        // Can't override some localized titles in dictionary. This is a workaround for now.
        window.setTimeout(function() {
            var header = document.querySelector('*[name="headerNameForm"] + .umb-panel-header-name');
            if (header) {
                header.innerHTML = "Umbraco 2FA";
            } 
            var tabs = document.querySelector('.umb-panel-header-content-wrapper .umb-nav-tabs');
            if (tabs) {
                tabs.className += " umb2fa-hide-tab";
            }
        }, 10);
    };

    $scope.save = function() {
    };

    $scope.hackNames();
}); 
 