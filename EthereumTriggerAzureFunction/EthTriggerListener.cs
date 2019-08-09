using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Web3;
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

        public EthTriggerListener(
            ITriggeredFunctionExecutor executor,
            Web3 web3,
            Contract contract
        ) {
            _executor = executor;
            _web3 = web3;
            _contract = contract;
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
            string eventName = string.Empty;
            var dnAttribute = typeof(MyEvent).GetCustomAttributes(
                typeof(EventAttribute), true
            ).FirstOrDefault() as EventAttribute;
            if(dnAttribute != null) {
                eventName = dnAttribute.Name;
            } else {
                return;
            }
            List<string> last100TransactionHashs = new List<string>();
            while(true) {
                List<EventLog<MyEvent>> logis = new List<EventLog<MyEvent>>();
                while(logis.Count == 0) {
                    var Event = _contract.GetEvent(eventName);
                    var filterAll = await Event.CreateFilterAsync();
                    logis = await Event.GetFilterChanges<MyEvent>(filterAll);
                    await Task.Delay(500);
                }

                var hashes = logis.Select(s => s.Log.TransactionHash).ToList();
                var newHashesNotMatchingOld = hashes
                    .Where(a => !last100TransactionHashs.Any(f => f == a))
                    .ToList();

                var logsToSend = logis
                    .Where(s => newHashesNotMatchingOld.Any(a => a == s.Log.TransactionHash ))
                    .ToList();

                last100TransactionHashs = last100TransactionHashs
                    .Where(a => hashes.Any(f => f == a))
                    .ToList();

                await Task.Delay(500);
                foreach(var item in logsToSend) {
                    await _executor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = item }, CancellationToken.None);
                }

                last100TransactionHashs.AddRange(logsToSend
                    .Select(s => s.Log.TransactionHash)
                    .ToList());
            }

        }
    }
}
