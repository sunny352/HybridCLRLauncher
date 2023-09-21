﻿using UnityEditor;
using Utils.Editor.Yoo;
using YooAsset.Editor;

namespace Launcher.Editor
{
    public static class Build
    {
        private static readonly Packer Packer = new Packer(Define.LauncherPackageName);
        
        [MenuItem("功能/Launcher/构建/资源包")]
        public static void Package()
        {
            Packer.Run("0.0.1", ECopyBuildinFileOption.ClearAndCopyAll);
        }
    }
}