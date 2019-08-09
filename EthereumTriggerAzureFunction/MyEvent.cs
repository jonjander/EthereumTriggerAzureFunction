using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumTriggerAzureFunction {
    [Event("SuccessfulAttempt")]
    public class MyEvent {
        [Parameter("address", "from", 1, false)]
        public string Sender { get; set; }

        [Parameter("uint", "amount", 2, false)]
        public Int64 Result { get; set; }

        [Parameter("string", "_type", 3, false)]
        public string _type { get; set; }

        [Parameter("string", "_action", 4, false)]
        public string _action { get; set; }

        [Parameter("uint", "_pin", 5, false)]
        public Int64 _pin { get; set; }

    }
}
