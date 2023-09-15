using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Launcher
{
    public class Launcher : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log($"[Launcher] Start");
            RunUpdate();
        }

        private async void RunUpdate()
        {
            var config = await GetConfig();
        }

        private async Task<Config> GetConfig()
        {
            try
            {
                var uri = new Uri(Define.Url);
                switch (uri.Scheme)
                {
                    case "streaming":
                    {
                        var path = uri.AbsolutePath;
                        var response = await UnityWebRequest.Get($"{Application.streamingAssetsPath}{path}")
                            .SendWebRequest();
                        var json = response.downloadHandler.text;
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