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
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            var azureContext = new AzureContext(
                config["Credendials:ClientId"], 
                config["Credentials:ClientSecret"],
                config["TenantId"], config["Credentials:SubscriptionId"]
            );

            var machineName = config["VirtualMachine:Name"];
            var machineResourceGroup = config["VirtualMachine:ResourceGroup"];

            var policy = Policy.
                Handle<Exception>().
                Retry(4);

            await policy.ExecuteAsync(async () =>
           {
               await azureContext.Context.VirtualMachines.StartAsync(machineResourceGroup, machineName);

           });

        }


    }
}
