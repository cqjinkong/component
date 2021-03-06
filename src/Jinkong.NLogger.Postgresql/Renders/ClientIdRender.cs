﻿using System.Linq;
using System.Text;
using NLog;
using NLog.LayoutRenderers;
using Shashlik.Utils.Extensions;

namespace Jinkong.NLogger.Mysql.Renders
{
    [LayoutRenderer("aspnet-request-clientid")]
    public class ClientIdRender : NLog.Web.LayoutRenderers.AspNetLayoutRendererBase
    {
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HttpContextAccessor.HttpContext != null)
            {
                var clientId = HttpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(r => r.Type == "clientid")
                    ?.Value;
                if (!clientId.IsNullOrWhiteSpace())
                    builder.AppendLine(clientId.SubStringIfTooLong(100000));
            }
        }
    }
}