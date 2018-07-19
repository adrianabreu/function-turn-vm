using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMachineStarter.Infraestructure
{
    public class AzureContext
    {
        private readonly AzureCredentials _credentials;
        private readonly string _subscriptionId;

        public AzureContext(string clientId, string clientSecret, string tenantId, string subscriptionId)
        {
            _subscriptionId = subscriptionId;
            _credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecret, tenantId,
                AzureEnvironment.AzureGlobalCloud);
        }

        public IAzure Context => Azure.Configure()
            .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
            .Authenticate(_credentials)
            .WithSubscription(_subscriptionId);

    }
}
