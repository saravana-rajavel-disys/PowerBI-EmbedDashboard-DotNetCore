using Microsoft.PowerBI.Api.Models;
using System;

namespace AppOwnsData.Models
{
    public class ReportEmbedConfig
    {
        public Guid ReportId { get; set; }

        public string EmbedUrl { get; set; }

        public EmbedToken EmbedToken { get; set; }
    }
}
