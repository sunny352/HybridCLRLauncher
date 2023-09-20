using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

#if HOST_PLAY_MODE
using Launcher.Yoo;
#endif

namespace Launcher
{
    public class Package
    {
        public string Name { get; }
        public string MainUrl { get; set; }
        public string FallbackUrl { get; set; }

        public Package(string name, string mainUrl = "", string fallbackUrl = "")
        {
            Name = name;
            MainUrl = mainUrl;
            FallbackUrl = fallbackUrl;
        }

        private bool _isInit;

        public async Task Init()
        {
            if (_isInit)
            {
                return;
            }

            _isInit = true;

            var package = YooAssets.CreatePackage(Name);
#if HOST_PLAY_MODE
            var initializeParameters = new HostPlayModeParameters
            {
                BuildinQueryServices = new BuildinQueryServices(),
                DeliveryQueryServices = new DeliveryQueryServices(),
                RemoteServices = new RemoteServices(this)
            };
#else
            var initializeParameters = new EditorSimulateModeParameters
            {
                SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(Name),
            };
#endif
            var initializationOperation = package.InitializeAsync(initializeParameters);
            await initializationOperation;
            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"[{Name}] Package init succeed");
            }
            else
            {
                Debug.LogError($"[{Name}] Package init failed - {initializationOperation.Error}");
            }
        }

        public async Task<bool> CheckAndUpdate()
        {
            var package = YooAssets.GetPackage(Name);
            var versionOperation = package.UpdatePackageVersionAsync();
            await versionOperation;

            if (versionOperation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                Debug.Log($"[{Name}] Updated package Version succeed : {versionOperation.PackageVersion}");
            }
            else
            {
                //更新失败
                Debug.LogError($"[{Name}] Updated package Version failed - {versionOperation.Error}");
                return false;
            }

            var manifestOperation = package.UpdatePackageManifestAsync(versionOperation.PackageVersion, false);
            await manifestOperation;

            if (manifestOperation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                Debug.Log($"[{Name}] Updated package Manifest succeed");
            }
            else
            {
                //更新失败
                Debug.LogError($"[{Name}] Updated package Manifest failed - {manifestOperation.Error}");
                return false;
            }

            var downloader = package.CreateResourceDownloader(100, 10);

            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {
                return true;
            }

            //需要下载的文件总数和总大小
            var totalDownloadCount = downloader.TotalDownloadCount;
            var totalDownloadBytes = downloader.TotalDownloadBytes;
            Debug.Log($"[{Name}] TotalDownloadCount : {totalDownloadCount}, TotalDownloadBytes : {totalDownloadBytes}");

            //注册回调方法
            downloader.OnDownloadOverCallback = OnDownloadOverFunction;
            downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
            downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

            //开启下载
            downloader.BeginDownload();
            await downloader;

            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
            {
                //下载成功
                Debug.Log($"[{Name}] Download succeed");
            }
            else
            {
                //下载失败
                Debug.LogError($"[{Name}] Download failed - {downloader.Error}");
                return false;
            }

            manifestOperation.SavePackageVersion();
            return true;
        }

        private void OnDownloadOverFunction(bool isSucceed)
        {
            Debug.Log($"[{Name}] OnDownloadOverFunction : {isSucceed}");
        }

        private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int lastDownloadCount,
            long totalDownloadBytes, long lastDownloadBytes)
        {
            // Debug.Log($"[{Name}] OnDownloadProgressUpdateFunction : {totalDownloadCount}, {lastDownloadCount}, {totalDownloadBytes}, {lastDownloadBytes}");
        }

        private void OnDownloadErrorFunction(string fileName, string error)
        {
            Debug.LogError($"[{Name}] OnDownloadErrorFunction : {fileName}, {error}");
        }

        private void OnStartDownloadFileFunction(string fileName, long fileSize)
        {
            Debug.Log($"[{Name}] OnStartDownloadFileFunction : {fileName}, {fileSize}");
        }
    }
}