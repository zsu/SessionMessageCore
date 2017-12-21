using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace SessionMessage.Core
{


    public class SessionMessageFactory : ISessionMessageFactory
    {
        private readonly Type _type;
        private IServiceProvider _serviceProvider;
        public SessionMessageFactory(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var typeName = configuration["sessionMessage:sessionMessageFactoryTypeName"];
            if (string.IsNullOrWhiteSpace(typeName))
                typeName = "SessionMessage.Core.CookieSessionMessageProvider,SessionMessage";
            _type = Type.GetType(typeName, true, true);
            _serviceProvider = serviceProvider;
        }
        public SessionMessageFactory(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                typeName = "SessionMessage.Core.CookieSessionMessageProvider,SessionMessage";

            _type = Type.GetType(typeName, true, true);
        }

        //public SessionMessageFactory()
        //    : this(_configuration["sessionMessageFactoryTypeName"])
        //{
        //}

        public ISessionMessageProvider CreateInstance()
        {
            var providers = _serviceProvider.GetService<IEnumerable<ISessionMessageProvider>>();
            ISessionMessageProvider provider=null;
            if(providers!=null)
            {
                provider = providers.FirstOrDefault(o => o.GetType() == _type);
            }
            if (provider == null)
                throw new Exception(string.Format("Cannot find type {0}", _type.Name));
            return provider;//Activator.CreateInstance(_type) as ISessionMessageProvider;
        }
    }
}