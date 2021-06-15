// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace AppOwnsData.Services
{
    using AppOwnsData.Models;
    using Microsoft.PowerBI.Api;
    using Microsoft.PowerBI.Api.Models;
    using Microsoft.Rest;
    using System;
    using System.Linq;

    public class PbiEmbedService
    {
        private readonly AadService aadService;
        private readonly string urlPowerBiServiceApiRoot  = "https://api.powerbi.com";

        public PbiEmbedService(AadService aadService)
        {
            this.aadService = aadService;
        }

        /// <summary>
        /// Get Power BI client
        /// </summary>
        /// <returns>Power BI client object</returns>
        public PowerBIClient GetPowerBIClient()
        {
            var tokenCredentials = new TokenCredentials(aadService.GetAccessToken(), "Bearer");
            return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot ), tokenCredentials);
        }

        /// <summary>
        /// Get embed params for a dashboard
        /// </summary>
        /// <returns>Wrapper object containing Embed token, Embed URL for single dashboard</returns>
        public DashboardEmbedConfig EmbedDashboard(Guid workspaceId)
        {
            PowerBIClient pbiClient = GetPowerBIClient();

            // Get a list of dashboards.
            var dashboards = pbiClient.Dashboards.GetDashboardsInGroupAsync(workspaceId).Result;

            // Get the first report in the workspace.
            var dashboard = dashboards.Value.FirstOrDefault();

            if (dashboard == null)
            {
                throw new NullReferenceException("Workspace has no dashboards");
            }

            // Generate Embed Token.
            var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
            var tokenResponse = pbiClient.Dashboards.GenerateTokenInGroupAsync(workspaceId, dashboard.Id, generateTokenRequestParameters);

            if (tokenResponse == null)
            {
                throw new NullReferenceException("Failed to generate embed token");
            }

            // Generate Embed Configuration.
            var dashboardEmbedConfig = new DashboardEmbedConfig
            {
                EmbedToken = tokenResponse.Result,
                EmbedUrl = dashboard.EmbedUrl,
                DashboardId = dashboard.Id
            };

            return dashboardEmbedConfig;
        }
    }
}
