﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTriggerAzureFunction {
    [Event("SuccessfulAttempt")]
    public class MyEvent : IEventFilter {
        [Parameter("address", "from", 1, false)]
        public string Sender { get; set; }

        [Parameter("string", "_type", 2, false)]
        public string Type { get; set; }

        [Parameter("string", "_action", 3, false)]
        public string Action { get; set; }

        [Parameter("int", "_pin", 4, false)]
        public int Pin { get; set; }

        public Type ThisType => MethodBase.GetCurrentMethod().DeclaringType;

        /// <summary>
        /// Filter contract log to find event changes
        /// This method is called from the Trigger listner and must exist in all event classes
        /// </summary>
        /// <param name="_contract">target contract</param>
        /// <returns>Log, event och number of hits</returns>
        public async Task<(string, List<(FilterLog, string)>, int)> Filter(Contract _contract) {
            var dnAttribute = ThisType.GetCustomAttributes(typeof(EventAttribute), true).FirstOrDefault() as EventAttribute;
            var contractEvent = _contract.GetEvent(dnAttribute.Name);
            var filterAll = await contractEvent.CreateFilterAsync();
            await Task.Delay(500);
            var filterResult = await contractEvent.GetFilterChanges<MyEvent>(filterAll);
            var results = new List<(FilterLog, string)>();
            foreach(var item in filterResult) {
                results.Add((item.Log,JsonConvert.SerializeObject(item.Event)));
            }
            return (JsonConvert.SerializeObject(filterResult), results, filterResult.Count);
        }
    }
}
