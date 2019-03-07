using Newtonsoft.Json;

namespace AntRunner.Wrapper.Js
{
    public class Settings
    {
        [JsonProperty("debug")]
        public bool Debug { get; set; }

        [JsonProperty("debug.port")]
        public int DebugPort { get; set; } = 9229;

        [JsonProperty("nodejs.port")]
        public int Port { get; set; }
    }
}
