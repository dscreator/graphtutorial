using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GraphTutorial
{
    public class DeviceCodeAuthProvider : IAuthenticationProvider
    {
        private string[] _scopes;
        private IAccount _userAccount; //user name, identity provider and account id

        //Create a msal client as per the contract/requirements for a Public Client Application
        private IPublicClientApplication _msalClient;


        public DeviceCodeAuthProvider(string appId, string[] scopes)
        {
            _scopes = scopes;

            //   PublicClientApplicationBuilder creates a PublicClientApplicationBuilder from a clientID. See https://aka.ms/msal-net-application-configuration
            //
            // Parameters:
            //   clientId:
            //     Client ID (also known as App ID) of the application as registered in the application
            //     registration portal (https://aka.ms/msal-net-register-app)/.
            _msalClient = PublicClientApplicationBuilder //from msal
                .Create(appId)
                // Set the tenant ID to "organizations" to disable personal accounts
                // Azure OAuth does not support device code flow for personal accounts
                // See https://docs.microsoft.com/azure/active-directory/develop/v2-oauth2-device-code
                .WithTenantId("organizations")
                .Build();
        }

        public async Task<string> GetAccessToken()
        {
            // If there is no saved user account, the user must sign-in
            if (_userAccount == null)
            {
                try
                {
                // Invoke device code flow so user can sign-in with a browser on a any device with a browser
                //Summary:
                    //     Acquires a security token on a device without a Web browser, by letting the user
                    //     authenticate on another device. This is done in two steps:
                    //     • The method first acquires a device code from the authority and returns it to
                    //     the caller via the deviceCodeResultCallback. This callback takes care of interacting
                    //     with the user to direct them to authenticate (to a specific URL, with a code)
                    //     • The method then proceeds to poll for the security token which is granted upon
                    //     successful login by the user based on the device code information
                    //     See https://aka.ms/msal-device-code-flow.
                    //
                    // Parameters:
                    //   scopes:
                    //     Scopes requested to access a protected API
                    //
                    //   deviceCodeResultCallback:
                    //     Callback containing information to show the user about how to authenticate and
                    //     enter the device code.
                    //
                    // Returns:
                    //     A builder enabling you to add optional parameters before executing the token request
                    //
                    // Remarks:
                    //     You can also pass optional parameters by calling: Microsoft.Identity.Client.AbstractAcquireTokenParameterBuilder`1.WithExtraQueryParameters(System.Collections.Generic.Dictionary{System.String,System.String})
                    //     to pass additional query parameters to the STS, and one of the overrides of Microsoft.Identity.Client.AbstractAcquireTokenParameterBuilder`1.WithAuthority(System.String,System.Boolean)
                    //     in order to override the default authority set at the application construction.
                    //     Note that the overriding authority needs to be part of the known authorities
                    //     added to the application construction.

                    var result = await _msalClient.AcquireTokenWithDeviceCode(_scopes, callback => {
                        Console.WriteLine(callback.Message);
                        return Task.FromResult(0);
                    }).ExecuteAsync();

                    _userAccount = result.Account;
                    return result.AccessToken;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error getting access token: {exception.Message}");
                    return null;
                }
            }
            else
            {
                // If there is an account, call AcquireTokenSilent
                // By doing this, MSAL will refresh the token automatically if
                // it is expired. Otherwise it returns the cached token.

                    var result = await _msalClient
                        .AcquireTokenSilent(_scopes, _userAccount)
                        .ExecuteAsync();

                   return result.AccessToken;
            }
        }

        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            // Summary of AuthenticationHeaderValue in System.Net.Http:
            //     Initializes a new instance of the System.Net.Http.Headers.AuthenticationHeaderValue class.
            //
            // Parameters:
            //   scheme:
            //     The scheme to use for authorization.
            //
            //   parameter:
            //     The credentials containing the authentication information of the user agent for
            //     the resource being requested.
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetAccessToken());
        }
    }
}