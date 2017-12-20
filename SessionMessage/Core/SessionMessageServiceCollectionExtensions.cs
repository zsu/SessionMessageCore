using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SessionMessages.Core
{
    public static class SessionMessageServiceCollectionExtensions
    {

        public static IServiceCollection AddSessionMessage(this IServiceCollection services)
        {
            IServiceCollection col=services;
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (services.FirstOrDefault(d => d.ImplementationType == typeof(CookieSessionMessageProvider)) == null)
                col = services.AddSingleton<ISessionMessageProvider, CookieSessionMessageProvider>();
            if (services.FirstOrDefault(d => d.ImplementationType == typeof(SessionStateSessionMessageProvider)) == null)
                col = services.AddSingleton<ISessionMessageProvider, SessionStateSessionMessageProvider>();
            if (services.FirstOrDefault(d => d.ServiceType == typeof(ISessionMessageFactory)) == null)
                col = services.AddSingleton<ISessionMessageFactory, SessionMessageFactory>();
            if (services.FirstOrDefault(d => d.ServiceType == typeof(ISessionMessageManager)) == null)
                col = services.AddSingleton<ISessionMessageManager, SessionMessageManager>();
            if (services.FirstOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor)) == null)
                col = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            if (services.FirstOrDefault(d => d.ServiceType == typeof(IActionContextAccessor)) == null)
                col = services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            return col;
        }
    }
}
