using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;

namespace EthereumTriggerAzureFunction {
    public static class Function1
    {

        [FunctionName(nameof(EthTrigger))]
        public static async Task EthTrigger(
            [EthTrigger("TestContract:ABI", "TestContract:Address", "RopstenNetworkEndpoint")]EventLog<MyEvent> dc,
            ILogger log
            )
        {
            log.LogInformation(dc.Log.BlockNumber + ": " + dc.Event._action);
            await Task.Delay(1);
        }
    }

    
}
