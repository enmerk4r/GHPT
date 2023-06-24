using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHPT.IO
{
    public class ResponsePayload
    {
        [JsonProperty("id")]
        string Id { get; set; }

        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }

        public ResponsePayload() { }
    }
}
