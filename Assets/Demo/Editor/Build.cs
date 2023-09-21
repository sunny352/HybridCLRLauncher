using UnityEditor;
using Utils.Editor.Yoo;
using YooAsset.Editor;

namespace Demo.Editor
{
    public static class Build
    {
        private static readonly Packer Packer = new Packer(Define.PackageName);
        
        [MenuItem("功能/Demo/构建/资源包")]
        public static void Package()
        {
            Packer.Run("0.0.1", ECopyBuildinFileOption.ClearAndCopyAll);
        }
    }
}