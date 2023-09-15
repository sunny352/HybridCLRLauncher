using Unity.Plastic.Newtonsoft.Json;

namespace Launcher
{
    public class Config
    {
        [JsonProperty("app")]
        public App App { get; set; }
        [JsonProperty("package")]
        public Package[] Packages { get; set; }
    }

    public class App
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
    public class Package
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("main")]
        public string MainUrl { get; set; }
        [JsonProperty("fallback")]
        public string FallbackUrl { get; set; }
    }
}