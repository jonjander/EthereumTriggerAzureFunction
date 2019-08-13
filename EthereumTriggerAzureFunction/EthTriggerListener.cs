using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EthereumTriggerAzureFunction {
    internal class EthTriggerListener : IListener {

        private Task _listenerTask;
        private CancellationTokenSource _listenerStoppingTokenSource;
        private ITriggeredFunctionExecutor _executor { get; set; }
        private Web3 _web3 { get; set; }

        private Contract _contract;

        private Func<Contract, Task<(string, List<(FilterLog, string)>, int)>> _filterFunction;



        public EthTriggerListener(
            ITriggeredFunctionExecutor executor,
            Web3 web3,
            Contract contract,
            Func<Contract, Task<(string, List<(FilterLog, string)>, int)>> filterFunction
        ) {
            _executor = executor;
            _web3 = web3;
            _contract = contract;
            _filterFunction = filterFunction;
        }   

        public void Cancel() {
            StopAsync(CancellationToken.None).Wait();
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            _listenerStoppingTokenSource = new CancellationTokenSource();
            _listenerTask = ListenAsync(_listenerStoppingTokenSource.Token);

            return _listenerTask.IsCompleted ? _listenerTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            if(_listenerTask == null) {
                return;
            }

            try {
                _listenerStoppingTokenSource.Cancel();
            } finally {
                await Task.WhenAny(_listenerTask,
                    Task.Delay(Timeout.Infinite, cancellationToken));
            }

        }

        public void Dispose() { }

        private async Task ListenAsync(CancellationToken listenerStoppingToken) {

            List<string> last100TransactionHashs = new List<string>();
            while(true) {
                var Result = ("", new List<(FilterLog, string)>(), 0);
                while(Result.Item3 == 0) {
                    try {
                        Result = await _filterFunction(_contract);

                    } catch {

                    }
                }

                var hashes = Result.Item2
                    .Select(s => s.Item1.TransactionHash).ToList();
                var newHashesNotMatchingOld = hashes
                    .Where(a => !last100TransactionHashs.Any(f => f == a))
                    .ToList();

                var logsToSend = Result.Item2
                    .Where(s => newHashesNotMatchingOld.Any(a => a == s.Item1.TransactionHash ))
                    .ToList();

                last100TransactionHashs = last100TransactionHashs
                    .Where(a => hashes.Any(f => f == a))
                    .ToList();

                await Task.Delay(500);
                foreach(var item in logsToSend) {
                    await _executor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = new EventResult(item.Item2, item.Item1) }, CancellationToken.None);
                }

                last100TransactionHashs.AddRange(logsToSend
                    .Select(s => s.Item1.TransactionHash)
                    .ToList());
            }

        }
    }
}
