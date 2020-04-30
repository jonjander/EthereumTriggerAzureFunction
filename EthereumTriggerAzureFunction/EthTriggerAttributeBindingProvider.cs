using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTriggerAzureFunction {

    //Implementation of ITriggerBindingProvider will be called when functions are being discovered
    internal class EthTriggerAttributeBindingProvider : ITriggerBindingProvider {


        private readonly Task<ITriggerBinding> _nullTriggerBindingTask = Task.FromResult<ITriggerBinding>(null);
        private INameResolver _nameResolver;
        private ILoggerFactory _loggerFactory;
        private IConfiguration _configuration;

        public EthTriggerAttributeBindingProvider(
            IConfiguration configuration,
            INameResolver nameResolver, 
            ILoggerFactory loggerFactory
            ) {
            _nameResolver = nameResolver;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context) {
            ParameterInfo parameter = context.Parameter;

            EthTriggerAttribute triggerAttribute = parameter.GetCustomAttribute<EthTriggerAttribute>(inherit: false);
            if(triggerAttribute is null) {
                return _nullTriggerBindingTask;
            }

            var contractABI = _configuration.GetSection(triggerAttribute.ABI).Value;
            var contractAddress = _configuration.GetSection(triggerAttribute.Address).Value;
            var networkUrl = _configuration.GetSection(triggerAttribute.NetworkUrl).Value;

            Web3 web3 = new Nethereum.Web3.Web3(networkUrl);

            Contract contract = web3.Eth.GetContract(contractABI, contractAddress);

            var filterClass = (IEventFilter)Activator.CreateInstance(Type.GetType(triggerAttribute.TypeName));

            return Task.FromResult<ITriggerBinding>(
                new EthTriggerBinding(parameter, web3, contract, filterClass.Filter)
            );
        }

    }
}
