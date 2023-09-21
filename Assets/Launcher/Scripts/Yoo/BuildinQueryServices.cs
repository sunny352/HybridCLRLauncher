using YooAsset;

namespace Launcher.Yoo
{
    public class BuildinQueryServices : IBuildinQueryServices
    {
        public bool QueryStreamingAssets(string packageName, string fileName)
        {
            return packageName == Define.LauncherPackageName;
        }
    }
}