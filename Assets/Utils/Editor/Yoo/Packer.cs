using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace Utils.Editor.Yoo
{
    public class Packer
    {
        private readonly string _outputRoot;
        private readonly string _packageName;

        public Packer(string packageName, string outputRoot = "YooPackages")
        {
            _outputRoot = outputRoot;
            _packageName = packageName;
        }

        public string Run(string version, ECopyBuildinFileOption option = ECopyBuildinFileOption.None)
        {
            var builder = new AssetBundleBuilder();
            var result = builder.Run(new BuildParameters
            {
                SBPParameters = new BuildParameters.SBPBuildParameters
                {
                    WriteLinkXML = true,
                },
                StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot(),
                BuildOutputRoot = _outputRoot,
                BuildTarget = EditorUserBuildSettings.activeBuildTarget,
                BuildPipeline = EBuildPipeline.ScriptableBuildPipeline,
                BuildMode = EBuildMode.IncrementalBuild,
                PackageName = _packageName,
                PackageVersion = version,
                VerifyBuildingResult = true,
                SharedPackRule = new ZeroRedundancySharedPackRule(),
                CompressOption = ECompressOption.LZ4,
                OutputNameStyle = EOutputNameStyle.HashName,
                CopyBuildinFileOption = option,
            });

            if (result.Success)
            {
                Debug.Log($"[{_packageName}] Build Success");
                Debug.Log($"[{_packageName}] {result.OutputPackageDirectory}");
                var reportDir = $"{result.OutputPackageDirectory}_report";
                if (Directory.Exists(reportDir))
                {
                    Directory.Delete(reportDir, true);
                }

                Directory.CreateDirectory(reportDir);
                var files = Directory.GetFiles(result.OutputPackageDirectory, "*.json", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    File.Move(file, Path.Combine(reportDir, Path.GetFileName(file)));
                }
                File.Move($"{result.OutputPackageDirectory}/link.xml", $"{reportDir}/link.xml");
                return result.OutputPackageDirectory;
            }
            else
            {
                Debug.LogError($"[{_packageName}] Build Failed - {result.ErrorInfo}");
                return string.Empty;
            }
        }
    }
}