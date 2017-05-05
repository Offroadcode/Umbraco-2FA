# Umbraco 2FA &middot; ![version](https://img.shields.io/badge/version-1.0.0-green.svg)

Umbraco 2FA is a two factor authentication solution for Umbraco 7.6+ by [Offroadcode](https://offroadcode.com). With it you can configure your Umbraco site to utilize either Google Authenticator or an SMS provider to provide two factor authentication when logging into the Umbraco backoffice.

## Installation & Setup

**Note!:** Umbraco 2FA only works for Umbraco 7.6 and greater. Do not install on earlier versions.

### Installing Package to Umbraco

You can install the Umbraco 2FA package to Umbraco via these following steps:

1. Download the [package zip file](https://github.com/Offroadcode/Umbraco-2FA/tree/master/pkg).
2. In your Umbraco back office, navigate to Developer > Packages and click the "Install Local" icon in the upper right of the page.
3. Either drag the zip file onto the provided frame or click on the "or click here to choose files" link and select the zip file using your file explorer.
4. Accept the terms of use and hit "Install Package". Hit "Finish" once prompted.
5. Navigate to the Users, and select an user or user type that you wish to have access to the Umbraco 2FA settings. In the "Sections" part of the user profile, make sure that Umbraco2FA is checked, and save.

### Setting Up Authentication

By default, Umbraco 2FA will be enabled with Google Authenticator. By navigating to the Umbraco2FA section in the back office, you can edit your 2FA options.

#### Configuring Google Authenticator

Google Authenticator is a 2FA tool by Google that your users can download for for free onto their phones for added security when logging onto your site. (More details below in "How to Use".)

To set it up:

1. In the Umbraco 2FA section, click on "Google Authenticator".
2. You may alter the site name, and toggle this authenticator option on or off via the "Enabled" button. Click Save to complete.

#### Configuring An SMS Provider

Functionality for implementing an SMS provider requires a developer to write code for the sending of the SMS itself. We are are open to any pull requests for support for various suppliers.

If you are doing such, your SMS provider should implement Orc.Fortress.SMSProvider.BaseSMSProvider.

You will have to implement a single method, `void SendSms(string number, string message)`. The number is the phone number to send the message to, and the message is the complete text of the SMS message to send.

##### Settings

SMS providers will probably have some settings with them such as API keys, etc. On your implementation of the SMS provider, you can have public properties which will be auto populated from the settings within the umbraco backend (e.g.
[FromFortressSettings("This is the username for you account with the provider")]
public string UserName { get; set; }).

#### Disabling 2FA

You can turn off two-factor authentication for your site as follows:

1. In the Umbraco 2FA section, click on "Google Authenticator".
2. Uncheck "Enabled". Save.
3. Navigate to "SMS".
4. Uncheck "Enabled". Save.

## How To Use the Two Factor Authentication

### Initial Configuration

When a user tries to log into an Umbraco 2FA-protected installation for the first time, they will be prompted to choose the type of authenticator they wish to use, and then confirm with an authenticator-specific code before proceeding.

One option is to use Google Authenticator. It's a free 2FA tool from Google that you can download for free from [Google Play](https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2) for your Android phone or [iTunes](https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8) for your iPhone. Once you have it downloaded, you can follow the instructions on your screen.

### Logging In

When a user logs into an Umbraco installation protected with Umbraco 2FA, they will first enter their username and password like normal. After that, they'll be prompted to enter a 2FA code before they'll be granted access.

## Contributing Code to Umbraco 2FA

The following is a guide for setting up the Umbraco 2FA source code on your machine if you wish to help develop it.

### Install Dependencies
*Requires Node.js to be installed and in your system path*

    npm install -g grunt-cli && npm install -g grunt
    npm install

### Build

    grunt

Builds the project to /dist/. These files can be dropped into an Umbraco 7 site, or you can build directly to a site using:

    grunt --target="D:\inetpub\mysite"

But if you do build directly into a site, you'll need to add the following to its `/Config/Dashboard.config` file so the custom section's dashboard will show:

    <section alias="umbraco2FA">
        <areas>
            <area>umbraco2FA</area>
        </areas>
        <tab caption="Umbraco2FA">
            <control>/App_Plugins/Umbraco2FA/views/dashboard.html</control>
        </tab>  
    </section>

You can also watch for changes using:

    grunt watch
    grunt watch --target="D:\inetpub\mysite"

If you want to build the package file (into a pkg folder), use:

    grunt umbraco

However, there is currently a problem with loading local package files in Umbraco and we'll need 7.5.8+ (which addresses this bug).
