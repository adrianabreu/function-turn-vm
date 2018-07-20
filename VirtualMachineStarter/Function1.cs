using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using VirtualMachineStarter.Infraestructure;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Polly;

namespace VirtualMachineStarter
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, TraceWriter log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            var azureContext = new AzureContext(
                config["ClientId"], 
                config["ClientSecret"],
                config["TenantId"], config["SubscriptionId"]
            );

            var machineName = config["VirtualMachineName"];
            var machineResourceGroup = config["VirtualMachineResourceGroup"];

            var policy = Policy
                .Handle<Exception>()
                .RetryAsync(4);

            await policy.ExecuteAsync(async () =>
           {
               await azureContext.Context.VirtualMachines.StartAsync(machineResourceGroup, machineName);

           });

        }


    }
}
