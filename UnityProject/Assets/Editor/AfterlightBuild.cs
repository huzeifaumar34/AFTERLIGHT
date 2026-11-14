using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Afterlight.Editor
{
    [InitializeOnLoad]
    public static class AfterlightBuild
    {
        private const string ScenePath = "Assets/AFTERLIGHT/Scenes/HarborDistrict.unity";

        static AfterlightBuild()
        {
            EditorApplication.delayCall += EnsureProjectScene;
        }

        [MenuItem("AFTERLIGHT/Create or repair Harbor District scene")]
        public static void EnsureProjectScene()
        {
            Directory.CreateDirectory("Assets/AFTERLIGHT/Scenes");

            if (File.Exists(ScenePath))
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }
            else
            {
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var bootstrap = new GameObject("AFTERLIGHT / Runtime world");
                bootstrap.AddComponent<AfterlightBootstrap>();
                EditorSceneManager.SaveScene(scene, ScenePath);
            }

            AssetDatabase.ImportAsset(ScenePath, ImportAssetOptions.ForceUpdate);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            AssetDatabase.SaveAssets();
        }

        [MenuItem("AFTERLIGHT/Build Android APK")]
        public static void BuildAndroid()
        {
            EnsureProjectScene();

            PlayerSettings.productName = "AFTERLIGHT: NAMBARA";
            PlayerSettings.companyName = "AFTERLIGHT Studio";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.afterlight.nambara");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Minimal);
            PlayerSettings.Android.useCustomKeystore = false;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
            EditorUserBuildSettings.buildAppBundle = false;

            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string outputDirectory = Path.Combine(projectRoot, "Builds", "Android");
            Directory.CreateDirectory(outputDirectory);
            string outputPath = Path.Combine(outputDirectory, "AFTERLIGHT.apk");

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.CompressWithLz4
            };

            Debug.Log("AFTERLIGHT Android build started: " + outputPath);
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception("AFTERLIGHT Android build failed: " +
                    report.summary.result + " / " + report.summary.totalErrors + " errors.");
            }

            Debug.Log("AFTERLIGHT APK created. Size: " + report.summary.totalSize + " bytes");
        }
    }
}
