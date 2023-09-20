using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace Launcher
{
    public class Launcher : MonoBehaviour
    {
        private async void Start()
        {
            Debug.Log($"[Launcher] Start");
            await Init();
            RunUpdate();
        }

        private async Task Init()
        {
            YooAssets.Initialize();
            var launcherPackage = new Package(Define.LauncherPackageName);
            await launcherPackage.Init();
            _packages.Add(launcherPackage);
        }
        private async void RunUpdate()
        {
            await ShowLogo();
            var config = await ConfigLoader.Load();
            //初始化资源包
            await InitPackages(config.Packages);
            //检查更新
            await CheckAndUpdate(config.App.Version, config.App.Url);
        }

        private readonly List<Package> _packages = new List<Package>();
        private async Task InitPackages(IEnumerable<PackageConfig> packageConfigs)
        {
            foreach (var packageConfig in packageConfigs)
            {
                var package = _packages.Find(item => item.Name == packageConfig.Name);
                if (null != package)
                {
                    package.MainUrl = packageConfig.MainUrl;
                    package.FallbackUrl = packageConfig.FallbackUrl;
                }
                else
                {
                    package = new Package(packageConfig.Name, packageConfig.MainUrl, packageConfig.FallbackUrl);
                    _packages.Add(package);
                }
            }
            await Task.WhenAll(_packages.Select(package => package.Init()));
        }
        
        private async Task CheckAndUpdate(string version, string appUrl)
        {
            Debug.Log($"[Launcher] CheckAndUpdate");
            var launcherPackage = YooAssets.GetPackage("launcher");
            var uiUpdateHandle = launcherPackage.LoadAssetSync<GameObject>("Assets/Launcher/Prefabs/UIUpdate.prefab");
            var uiUpdatePrefab = uiUpdateHandle.GetAssetObject<GameObject>();
            var uiUpdateObj = Instantiate(uiUpdatePrefab);
            
            //检查App更新
            var appCheck = new AppCheck(version, appUrl);
            await appCheck.Check();
            
            foreach (var package in _packages)
            {
                while (true)
                {
                    var success = await package.CheckAndUpdate();
                    if (success)
                    {
                        break;
                    }
                    await UniTask.Delay(1000);
                }
            }
            Destroy(uiUpdateObj);
            uiUpdateHandle.Release();
            Debug.Log($"[Launcher] CheckAndUpdate Finished");
        }

        private async Task ShowLogo()
        {
            Debug.Log($"[Launcher] ShowLogo Start");
            var launcherPackage = YooAssets.GetPackage(Define.LauncherPackageName);
            var uiLogoHandle = launcherPackage.LoadAssetSync("Assets/Launcher/Prefabs/UILogo.prefab");
            var uiLogoPrefab = uiLogoHandle.GetAssetObject<GameObject>();
            var uiLogoObj = Instantiate(uiLogoPrefab);
            var uiLogo = uiLogoObj.GetComponent<UI.UILogo>();
            await uiLogo.Wait();
            Destroy(uiLogoObj);
            uiLogoHandle.Release();
            Debug.Log($"[Launcher] ShowLogo Finished");
        }
    }
}