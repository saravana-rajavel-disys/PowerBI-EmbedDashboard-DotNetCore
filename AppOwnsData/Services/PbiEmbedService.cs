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
        /// Get embed params for a report
        /// </summary>
        /// <returns>Wrapper object containing Embed token, Embed URL for single report</returns>
        public ReportEmbedConfig EmbedReport(Guid workspaceId)
        {
            PowerBIClient pbiClient = GetPowerBIClient();

            // Get a list of reports
            var reports = pbiClient.Reports.GetReportsInGroupAsync(workspaceId).Result;

            var reportName = "Horticulture Crop Coverage";

            // Get the report in the workspace
            var report = reports.Value.Where(x => x.Name == reportName).FirstOrDefault();

            if (report == null)
            {
                throw new NullReferenceException("Workspace has no reports");
            }

            // Generate Embed Token
            var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");

            var tokenResponse = pbiClient.Reports.GenerateTokenInGroupAsync(workspaceId, report.Id, generateTokenRequestParameters);

            if (tokenResponse == null)
            {
                throw new NullReferenceException("Failed to generate embed token");
            }

            // Generate Embed Configuration
            var reportEmbedConfig = new ReportEmbedConfig
            {
                EmbedToken = tokenResponse.Result,
                EmbedUrl = report.EmbedUrl,
                ReportId = report.Id
            };

            return reportEmbedConfig;
        }
    }
}
