using Newtonsoft.Json;

namespace AntRunner.Wrapper.Js
{
    public class Settings
    {
        [JsonProperty("debug")]
        public bool Debug { get; set; }

        [JsonProperty("nodejs.port")]
        public int Port { get; set; }
    }
}
