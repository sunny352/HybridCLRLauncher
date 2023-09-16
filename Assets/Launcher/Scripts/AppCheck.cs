using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Launcher
{
    public class AppCheck
    {
        private readonly string _version;
        private readonly string _url;
        public AppCheck(string version, string url)
        {
            _version = version;
            _url = url;
        }
        
        public async Task Check()
        {
            var localVersion = new Version(Application.version);
            var remoteVersion = new Version(_version);
            if (localVersion.Major < remoteVersion.Major)
            {
                //todo 弹出强制更新窗口
            }

            if (localVersion.Minor < remoteVersion.Minor)
            {
                //todo 弹出强制更新窗口
            }
            
            if (localVersion.Build < remoteVersion.Build)
            {
                //todo 弹出可选更新窗口
            }
            
            if (localVersion.Revision < remoteVersion.Revision)
            {
                //todo 弹出可选更新窗口
            }
        }
    }
}