using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Launcher
{
    public class Config
    {
        [JsonProperty("app")]
        public AppConfig App { get; set; }
        [JsonProperty("packages")]
        public PackageConfig[] Packages { get; set; }
    }

    public class AppConfig
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
    public class PackageConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("main")]
        public string MainUrl { get; set; }
        [JsonProperty("fallback")]
        public string FallbackUrl { get; set; }
    }

    public static class ConfigLoader
    {
        public static async Task<Config> Load()
        {
            try
            {
                var uri = new Uri(Define.Url);
                switch (uri.Scheme)
                {
                    case "streaming":
                    {
                        var path = uri.AbsolutePath;
#if UNITY_STANDALONE || UNITY_EDITOR
                        var fullPath = $"{Application.streamingAssetsPath}{path}";
                        var json = await File.ReadAllTextAsync(fullPath);
#else
                        var response = await UnityWebRequest.Get(uri).SendWebRequest();
                        var json = response.downloadHandler.text;
#endif
                        return JsonConvert.DeserializeObject<Config>(json);
                    }
                    case "http":
                    case "https":
                    {
                        var count = 0;
                        while (true)
                        {
                            var response = await UnityWebRequest.Get(uri).SendWebRequest();
                            if (UnityWebRequest.Result.Success == response.result)
                            {
                                var json = response.downloadHandler.text;
                                return JsonConvert.DeserializeObject<Config>(json);
                            }

                            count++;
                            if (count < 3)
                            {
                                Debug.LogError($"[Launcher] GetConfig Error: {response.error}, retry... ({count})");
                                await UniTask.Delay(1000);
                            }
                            else
                            {
                                Debug.LogError($"[Launcher] GetConfig Error: {response.error}");
                                break;
                            }
                        }
                    }
                        break;
                    default:
                        Debug.LogError($"[Launcher] GetConfig Error: Unknown scheme: {uri.Scheme}");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }
    }
}