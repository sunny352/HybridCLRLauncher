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
        private void Start()
        {
            Debug.Log($"[Launcher] Start");
            RunUpdate();
        }

        private async void RunUpdate()
        {
            var config = await ConfigLoader.Load();
            //初始化资源包
            await InitPackages(config.Packages);
            await ShowLogo();
            //检查更新
            await CheckAndUpdate(config.App.Version, config.App.Url);
        }

        private List<Package> _packages;
        private async Task InitPackages(IEnumerable<PackageConfig> packageConfigs)
        {
            YooAssets.Initialize();
            _packages = new List<Package>();
            var isExistLauncher = false;
            foreach (var packageConfig in packageConfigs)
            {
                isExistLauncher = isExistLauncher || packageConfig.Name == "launcher";
                var package = new Package(packageConfig.Name, packageConfig.MainUrl, packageConfig.FallbackUrl);
                _packages.Add(package);
            }
            if (!isExistLauncher)
            {
                var launcherPackage = new Package("launcher", "", "");
                _packages.Add(launcherPackage);
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
            var launcherPackage = YooAssets.GetPackage("launcher");
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