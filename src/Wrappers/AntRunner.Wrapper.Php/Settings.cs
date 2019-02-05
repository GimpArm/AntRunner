using Newtonsoft.Json;

namespace AntRunner.Wrapper.Php
{
    public class Settings
    {
        [JsonProperty("debug")]
        public bool Debug { get; set; }

        [JsonProperty("xdebug.idekey")]
        public string IdeKey { get; set; }
    }
}
