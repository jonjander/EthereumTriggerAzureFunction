using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumTriggerAzureFunction {
    public class EventResult {
        public EventResult(string _event, FilterLog _log) {
            EventString = _event;
            LogString = JsonConvert.SerializeObject(_log);
        }

        public EventResult() {

        }

        public (T _Event, FilterLog _Log) GetEvent<T>() {
            return (
                JsonConvert.DeserializeObject<T>(EventString),
                JsonConvert.DeserializeObject<FilterLog>(LogString));
        }

        public string EventString { get; set; }
        public string LogString { get; set; }
    }
}
