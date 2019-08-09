using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumTriggerAzureFunction {

    [Extension("Eth")]
    internal class EthExtensionConfigProvider : IExtensionConfigProvider {
        private IConfiguration _configuration;
        private INameResolver _nameResolver;
        private ILoggerFactory _loggerFactory;

        public EthExtensionConfigProvider(
        IConfiguration configuration,
        INameResolver nameResolver,
        ILoggerFactory loggerFactory) 
            {
            _configuration = configuration;
            _nameResolver = nameResolver;
            _loggerFactory = loggerFactory;
        }

        public void Initialize(ExtensionConfigContext context) {

            var triggerAttributeBindingRule = context.AddBindingRule<EthTriggerAttribute>();
            triggerAttributeBindingRule.BindToTrigger<EventLog<MyEvent>>(
                new EthTriggerAttributeBindingProvider(_configuration, _nameResolver, _loggerFactory)
            );
            
        }
    }


}
