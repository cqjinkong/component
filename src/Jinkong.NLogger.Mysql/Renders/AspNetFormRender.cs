﻿using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.LayoutRenderers;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

namespace Jinkong.NLogger.Mysql.Renders
{
    [LayoutRenderer("aspnet-request-form-custom")]
    public class AspNetFormRender : NLog.Web.LayoutRenderers.AspNetLayoutRendererBase
    {
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HttpContextAccessor.HttpContext != null)
            {
                var body = HttpContextAccessor?.HttpContext?.RequestServices?.GetService<AspNetForm>()?.Form;
                if (!body.IsNullOrWhiteSpace())
                    builder.AppendLine(body);
            }
        }
    }

    [Scoped]
    public class AspNetForm
    {
        public AspNetForm(IHttpContextAccessor httpContextAccessor)
        {

            form = new Lazy<string>(() =>
            {
                try
                {
                    var context = httpContextAccessor.HttpContext;
                    if (context == null)
                        return null;

                    if (context.Request.ContentLength == 0)
                        return null;

                    StringBuilder sb = new StringBuilder();
                    foreach (var item in context.Request.Form.Keys)
                    {
                        var value = context.Request.Form[item];
                        sb.Append(item);
                        sb.Append("=");
                        sb.Append(value.ToString());
                        sb.Append(",");
                    }

                    return sb.ToString().SubStringIfTooLong(100000);

                }
                catch
                {
                    return null;
                }
            });

        }

        private Lazy<string> form { get; }

        public string Form => form.Value;
    }
}
;