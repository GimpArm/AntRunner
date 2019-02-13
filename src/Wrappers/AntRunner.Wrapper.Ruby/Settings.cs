using Newtonsoft.Json;

namespace AntRunner.Wrapper.Ruby
{
    public class Settings
    {
        [JsonProperty("debug")]
        public bool Debug { get; set; }

        [JsonProperty("rdebug-ide.port")]
        public int Port { get; set; }
    }
}
