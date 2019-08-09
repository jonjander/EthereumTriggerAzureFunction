using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;

namespace EthereumTriggerAzureFunction {
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class EthTriggerAttribute : Attribute {

        [AppSetting]
        public string ABI { get; set; }
        [AppSetting]
        public string Address { get; set; }
        [AppSetting]
        public string NetworkUrl { get; set; }

        public EthTriggerAttribute(string aBI, string address, string networkUrl) {
            ABI = aBI;
            Address = address;
            NetworkUrl = networkUrl;
        }

    }
}
