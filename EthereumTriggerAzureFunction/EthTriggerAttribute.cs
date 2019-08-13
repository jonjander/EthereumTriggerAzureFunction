using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EthereumTriggerAzureFunction {
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class EthTriggerAttribute : Attribute {

        [AppSetting]
        public string ABI { get; set; }
        [AppSetting]
        public string Address { get; set; }
        [AppSetting]
        public string NetworkUrl { get; set; }
        public string TypeName { get; set; }

        public EthTriggerAttribute(string aBI, string address, string networkUrl, string typeName) {
            ABI = aBI;
            Address = address;
            NetworkUrl = networkUrl;
            TypeName = typeName;
        }

    }
}
