using YooAsset;

namespace Launcher.Yoo
{
    public class RemoteServices : IRemoteServices
    {
        private readonly Package _package;
        public RemoteServices(Package package)
        {
            _package = package;
        }
        public string GetRemoteMainURL(string fileName)
        {
            return $"{_package.MainUrl}/{fileName}";            
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            return $"{_package.FallbackUrl}/{fileName}";
        }
    }
}