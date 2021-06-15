// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

namespace AppOwnsData.Controllers
{
    using AppOwnsData.Models;
    using AppOwnsData.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System;

    public class EmbedInfoController : Controller
    {
        private readonly PbiEmbedService _pbiEmbedService;
        private readonly IOptions<PowerBI> _powerBI;

        public EmbedInfoController(PbiEmbedService pbiEmbedService, IOptions<PowerBI> powerBI)
        {
            _pbiEmbedService = pbiEmbedService;
            _powerBI = powerBI;
        }

        /// <summary>
        /// Returns Embed token, Embed URL, and Embed token expiry to the client
        /// </summary>
        /// <returns>JSON containing parameters for embedding</returns>
        [HttpGet]
        public ActionResult EmbedDashboard()
        {
            DashboardEmbedConfig dashboard = _pbiEmbedService.EmbedDashboard(new Guid(_powerBI.Value.WorkspaceId));
            return View(dashboard);
        }
    }
}
