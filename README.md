# Umbraco 2FA

A two factor authentication package for Umbraco by [Offroadcode](https://offroadcode.com).

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