using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SessionMessage.Mvc
{
    public static class SessionMessageApplicationBuilderExtension
    {
        public static IApplicationBuilder UseSessionMessage(this IApplicationBuilder app) => UseSessionMessage(app, null);

        public static IApplicationBuilder UseSessionMessage(this IApplicationBuilder app, Action<Options> setupOptions)
        {
            var options = new Options();
            setupOptions?.Invoke(options);
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = $"/{options.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(typeof(SessionMessageApplicationBuilderExtension).GetTypeInfo().Assembly, "SessionMessage.Mvc")
            });

            return app;
        }

        public class Options
        {
            internal Options() { }

            public string RoutePrefix { get; set; } = "sessionmessage";
        }
    }
}
