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

        /// <param name="aBI">Appsetting path to contract ABI</param>
        /// <param name="address">Appsetting path to contract address</param>
        /// <param name="networkUrl">Appsetting path to contract network endpoint uri</param>
        /// <param name="typeName">Event return type full name, used as a web3 filter</param>
        public EthTriggerAttribute(string aBI, string address, string networkUrl, string typeName) {
            ABI = aBI;
            Address = address;
            NetworkUrl = networkUrl;
            TypeName = typeName;
        }

    }
}
