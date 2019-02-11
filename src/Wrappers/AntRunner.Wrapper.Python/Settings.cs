using Newtonsoft.Json;

namespace AntRunner.Wrapper.Python
{
    public class Settings
    {
        [JsonProperty("debug")]
        public bool Debug { get; set; }

        [JsonProperty("ptvsd.port")]
        public int Port { get; set; }
    }
}
