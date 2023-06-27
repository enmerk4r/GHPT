using Newtonsoft.Json;

namespace GHPT.IO
{
    public class GPTConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        public GPTConfig() { }

        public GPTConfig(string token, string model)
        {
            this.Token = token;
            this.Model = model;
        }
    }
}
