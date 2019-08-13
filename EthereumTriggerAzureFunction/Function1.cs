using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Newtonsoft.Json;

namespace EthereumTriggerAzureFunction {
    public static class Function1
    {

        [FunctionName(nameof(EthTrigger))]
        public static async Task EthTrigger(
            [EthTrigger("TestContract:ABI", "TestContract:Address", "RopstenNetworkEndpoint", "EthereumTriggerAzureFunction.MyEventz")]EventResult ethEvent,
            ILogger log
            )
        {
            var dc = ethEvent.GetEvent<MyEventz>();
            log.LogInformation(dc._Log.BlockNumber + ": " + dc._Event.Pin);
            await Task.Delay(1);
        }
    }

    
}
