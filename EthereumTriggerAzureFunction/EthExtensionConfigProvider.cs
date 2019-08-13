using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            var ethTriggerAttributeBindingProvider = new EthTriggerAttributeBindingProvider(_configuration, _nameResolver, _loggerFactory);
            triggerAttributeBindingRule.BindToTrigger<EventResult>(
                ethTriggerAttributeBindingProvider
            );
            
        }
    }


}
