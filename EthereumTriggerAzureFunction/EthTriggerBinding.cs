using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTriggerAzureFunction {

    internal class EthTriggerBinding : ITriggerBinding {

        private bool _includeTypes { get; set; }
        private Web3 _web3 { get; set; }
        private readonly Task<ITriggerData> _emptyBindingDataTask =
            Task.FromResult<ITriggerData>(new TriggerData(null, new Dictionary<string, object>()));
        private Contract _contract;

        public Type TriggerValueType => typeof(EventResult);

        public IReadOnlyDictionary<string, Type> BindingDataContract { get; } =
            new Dictionary<string, Type>();

        public Func<Contract, Task<(string, List<(FilterLog, string)>, int)>> _filterFunction { get; set; }

        public EthTriggerBinding(
            ParameterInfo parameter,
            Web3 web3,
            Contract contract,
            Func<Contract, Task<(string, List<(FilterLog, string)>, int)>> filterFunction
            ) {
            _web3 = web3;
            _contract = contract;
            _filterFunction = filterFunction;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context) {
            return _emptyBindingDataTask;
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context) {


        return Task.FromResult<IListener>(
            new EthTriggerListener(context.Executor, _web3, _contract, _filterFunction)
        );
        }

        public ParameterDescriptor ToParameterDescriptor() {
            return new ParameterDescriptor();
        }
    }
}
