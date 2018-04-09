<div align="center">
    <img src="https://img.shields.io/badge/version-2.1.0-green.svg" />
    <a href="https://our.umbraco.org/projects/backoffice-extensions/umbraco-2fa/"><img src="https://img.shields.io/badge/our-umbraco-orange.svg"></a>
    <h1>Umbraco 2FA</h1>
</div>

Umbraco 2FA is a two factor authentication solution for Umbraco 7.6+ by [Offroadcode](https://offroadcode.com). With it you can configure your Umbraco site to utilize Google Authenticator to provide two factor authentication when logging into the Umbraco backoffice.

## Installation & Setup

**Note!:** Umbraco 2FA only works for Umbraco 7.6 and greater. Do not install on earlier versions.

### Installing Package to Umbraco

You can install the Umbraco 2FA package to Umbraco via these following steps:

1. Download the [package zip file](https://github.com/Offroadcode/Umbraco-2FA/tree/master/pkg).
2. In your Umbraco back office, navigate to Developer > Packages and click the "Install Local" icon in the upper right of the page.
3. Either drag the zip file onto the provided frame or click on the "or click here to choose files" link and select the zip file using your file explorer.
4. Accept the terms of use and hit "Install Package". Hit "Finish" once prompted.

### Setting Up Authentication

The Umbraco 2FA tab will appear as a tab in the Content section of the backoffice after installation.

Each user may enable 2FA for their account by performing the following steps:

1. Go to the Umbraco 2FA tab in Content.
2. Click on the 'Setup 2FA' button.
3. Open your Google Authenticator app on your smartphone ([Google Play](https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2) for your Android phone or [iTunes](https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8) for your iPhone), or a similar authenticator app, and select to add a new account by either barcode scan or manual entry.
4. If using barcode scan, scan the QR code on your screen. If using manual entry, enter the long string beneath the QR code.
5. Your Google Authenticator will display a 6-digit code. Enter this ento the text field and click "Verify".
6. Congrats! You now have 2FA configured on your Umbraco account.

To remove 2FA on your account, you can return to the Umbraco 2FA tab and click "Remove Authenticator".

## Logging In

When a user's account is configured with 2FA, any time they log into their account they will be prompted with a request for their Two Factor authentication code after entering their username and password. This code will be displayed on the authenticator that they are using.

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
