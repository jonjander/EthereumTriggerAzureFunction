using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EthereumTriggerAzureFunction {
    public interface IEventFilter {
        System.Type ThisType { get; }

        Task<(string, List<(FilterLog, string)>, int)> Filter(Contract _contract);
    }
}
