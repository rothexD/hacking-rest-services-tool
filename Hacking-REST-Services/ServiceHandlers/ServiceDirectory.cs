using System;
using System.Collections.Generic;
using System.Linq;
using Hacking_REST_Services.WebClient;

namespace Hacking_REST_Services.ServiceHandlers
{
    public class ServiceDirectory
    {
        private readonly Dictionary<string, Action<string, ICustomHttpClient>> _services;

        public ServiceDirectory()
        {
            _services = new Dictionary<string, Action<string, ICustomHttpClient>>();
        }

        public void AddServiceCall(string name, Action<string, ICustomHttpClient> serviceCall)
        {
            _services[name] = serviceCall;
        }

        public void RunAllTests(string targetUrl, ICustomHttpClient client)
        {
            foreach ((string name, var service) in _services)
            {
                service.Invoke(targetUrl, client);
            }
        }

        public IEnumerable<string> GetServiceNames()
        {
            return _services.Select(entry => entry.Key);
        }

        public void RunTest(string name, string targetUrl, ICustomHttpClient client)
        {
            if (!_services.ContainsKey(name))
            {
                throw new ArgumentException("No service with this name was registered.");
            }
            _services[name].Invoke(targetUrl, client);
        }
    }
}