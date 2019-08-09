using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(EthereumTriggerAzureFunction.Startup))]
namespace EthereumTriggerAzureFunction {
    public class Startup : IWebJobsStartup {

        public void Configure(IWebJobsBuilder builder) {
            builder.AddExtension<EthExtensionConfigProvider>();
        }
    }
}
