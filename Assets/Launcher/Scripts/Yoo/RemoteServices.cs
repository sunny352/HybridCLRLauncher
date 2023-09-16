using YooAsset;

namespace Launcher.Yoo
{
    public class RemoteServices : IRemoteServices
    {
        private readonly string _mainUrl;
        private readonly string _fallbackUrl;
        public RemoteServices(string mainUrl, string fallbackUrl)
        {
            _mainUrl = mainUrl;
            _fallbackUrl = fallbackUrl;
        }
        public string GetRemoteMainURL(string fileName)
        {
            return $"{_mainUrl}/{fileName}";            
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackUrl}/{fileName}";
        }
    }
}