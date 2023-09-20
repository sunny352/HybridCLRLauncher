using UnityEditor;
using Utils.Editor.Yoo;
using YooAsset.Editor;

namespace Launcher.Editor
{
    public static class Build
    {
        private static readonly Packer Packer = new Packer(Define.LauncherPackageName);
        
        [MenuItem("Launcher/Build/Package")]
        public static void Package()
        {
            Packer.Run("0.0.1", ECopyBuildinFileOption.ClearAndCopyAll);
        }
    }
}