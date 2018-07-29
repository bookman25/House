using System;
using Newtonsoft.Json;

namespace HouseService.Api.Models
{
    public class ODataObject
    {
        [JsonProperty("odata.type")]
        public string RuntimeType { get; set; }

        public string Id { get; set; }

        public ODataObject()
        {
            RuntimeType = GetType().FullName;
        }

        public ODataObject(string id)
        {
            Id = id;
            RuntimeType = GetType().FullName;
        }
    }
}
