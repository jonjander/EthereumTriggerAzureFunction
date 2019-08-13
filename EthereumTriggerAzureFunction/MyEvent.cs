using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTriggerAzureFunction {
    [Event("SuccessfulAttempt")]
    public class MyEventz : IEventFilter {
        [Parameter("address", "from", 1, false)]
        public string Sender { get; set; }

        [Parameter("string", "_type", 2, false)]
        public string Type { get; set; }

        [Parameter("string", "_action", 3, false)]
        public string Action { get; set; }

        [Parameter("int", "_pin", 4, false)]
        public int Pin { get; set; }

        public async Task<(string, List<(FilterLog, string)>, int)> Filter(Contract _contract) {
            var Event = _contract.GetEvent("SuccessfulAttempt");
            var filterAll = await Event.CreateFilterAsync();
            await Task.Delay(500);
            var logis = await Event.GetFilterChanges<MyEventz>(filterAll);
            
            var results = new List<(FilterLog, string)>();
            foreach(var item in logis) {
                results.Add((
                    item.Log,
                    JsonConvert.SerializeObject(item.Event)
                    ));
            }
            return (JsonConvert.SerializeObject(logis), results, logis.Count);
        }

    }

    public interface IEventFilter {
        Task<(string, List<(FilterLog, string)>, int)> Filter(Contract _contract);
    }


    public class EventResult {
        public EventResult(string _event, FilterLog _log) {
            EventString = _event;
            LogString = JsonConvert.SerializeObject(_log);
        }

        public EventResult() {

        }

        public (T _Event, FilterLog _Log) GetEvent<T>(){
            return (
                JsonConvert.DeserializeObject<T>(EventString),
                JsonConvert.DeserializeObject<FilterLog>(LogString)) ;
        }

        public string EventString { get; set; }
        public string LogString { get; set; }
    }
}
