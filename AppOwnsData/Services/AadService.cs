// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace AppOwnsData.Services
{
    using AppOwnsData.Models;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;
    using System;
    using System.Linq;
    using System.Security;

    public class AadService
    {
        private readonly IOptions<AzureAd> _azureAd;

        public AadService(IOptions<AzureAd> azureAd)
        {
            _azureAd = azureAd;
        }

        /// <summary>
        /// Generates and returns Access token
        /// </summary>
        /// <returns>AAD token</returns>
        public string GetAccessToken()
        {
            AuthenticationResult authenticationResult = null;
            if (_azureAd.Value.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
            {
                // Create a public client to authorize the app with the AAD app
                IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(_azureAd.Value.ClientId).WithAuthority(_azureAd.Value.AuthorityUri).Build();

                SecureString password = new SecureString();
                foreach (var key in _azureAd.Value.PbiPassword)
                {
                    password.AppendChar(key);
                }
                authenticationResult = clientApp.AcquireTokenByUsernamePassword(_azureAd.Value.Scope, _azureAd.Value.PbiUsername, password).ExecuteAsync().Result;
            }

            // Service Principal auth is the recommended by Microsoft to achieve App Owns Data Power BI embedding
            else if (_azureAd.Value.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
            {
                // For app only authentication, we need the specific tenant id in the authority url
                var tenantSpecificUrl = _azureAd.Value.AuthorityUri.Replace("organizations", _azureAd.Value.TenantId);

                // Create a confidential client to authorize the app with the AAD app
                IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                                                .Create(_azureAd.Value.ClientId)
                                                                                .WithClientSecret(_azureAd.Value.ClientSecret)
                                                                                .WithAuthority(tenantSpecificUrl)
                                                                                .Build();
                // Make a client call if Access token is not available in cache
                authenticationResult = clientApp.AcquireTokenForClient(_azureAd.Value.Scope).ExecuteAsync().Result;
            }

            return authenticationResult.AccessToken;
        }
    }
}
