using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace SweatyChair
{

	/// <summary>
	/// A building tool that quickly switchs different builds.
	/// </summary>
	public static class BuildTool
	{

		private static readonly string PATH_PROJECT = Application.dataPath.Replace("/Assets", "");
		private static readonly string PATH_BUILDS = Path.Combine(PATH_PROJECT, "Builds");

		private static readonly string FILENAME_VERSION_CODE = PlayerSettings.bundleVersion.Replace(".", "-");

		private static BuildToolSettings _setting = Resources.Load<BuildToolSettings>("BuildToolSettings");

		private static DateTime _mannualDateTime;
		private static DateTime _dateTime => _mannualDateTime == default ? DateTime.Now : _mannualDateTime;

		[MenuItem("Sweaty Chair/Build Tool/Change To iOS Build")]
		private static void ChangeToIOSSettings()
		{
			if (_setting.FindBuildInfo("iOS", out BuildToolBuildInfo buildInfo))
				ChangeToBuild(buildInfo);
		}

		[MenuItem("Sweaty Chair/Build Tool/Change To iOS Chinese Build")]
		private static void ChangeToIOSChineseSettings()
		{
			if (_setting.FindBuildInfo("iOS Chinese", out BuildToolBuildInfo buildInfo))
				ChangeToBuild(buildInfo);
		}

		[MenuItem("Sweaty Chair/Build Tool/Change To Android Global Build")]
		private static void ChangeToAndroidGlobalBuild()
		{
			if (_setting.FindBuildInfo("Android Global", out BuildToolBuildInfo buildInfo))
				ChangeToBuild(buildInfo);
		}

		[MenuItem("Sweaty Chair/Build Tool/Change To Android Alternative Store Build")]
		private static void ChangeToAndroidAlternativeStoreBuild()
		{
			if (_setting.FindBuildInfo("Android Alternative Store", out BuildToolBuildInfo buildInfo))
				ChangeToBuild(buildInfo);
		}

		[MenuItem("Sweaty Chair/Build Tool/Change To Android Chinese Build")]
		private static void ChangeToAndroidChineseBuild()
		{
			if (_setting.FindBuildInfo("Android Chinese", out BuildToolBuildInfo buildInfo))
				ChangeToBuild(buildInfo);
		}

		public static void ChangeToBuild(BuildToolBuildInfo buildInfo)
		{
			if (_setting.setProductName)
				PlayerSettings.SetApplicationIdentifier(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), buildInfo.productName);
			if (_setting.setBundleId)
				PlayerSettings.SetApplicationIdentifier(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), buildInfo.bundleId);
			if (_setting.setDefineSymbols)
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), buildInfo.defineSymbols);
		}

		[MenuItem("Sweaty Chair/Build Tool/Export Android Project")]
		private static void ExportAndroidProject()
		{
#if UNITY_ANDROID
			Build(true);
#endif
		}

		[MenuItem("Sweaty Chair/Build Tool/Build Android APK")]
		private static void BuildAndroidAPK()
		{
#if UNITY_ANDROID
			Build(false);
#endif
		}

		[MenuItem("Sweaty Chair/Build Tool/Build Now #&b")]
		public static void Build()
		{
#if UNITY_IOS
			Build(true);
#else
			Build(false);
#endif
		}

		[MenuItem("Sweaty Chair/Build Tool/Build Now Version+1")]
		private static void BuildVersionPlus1()
		{
			_mannualDateTime = DateTime.Now.AddHours(24);
			Build();
		}

		[MenuItem("Sweaty Chair/Build Tool/Reset Manual Version")]
		private static void RestManualVersion()
		{
			_mannualDateTime = default;
		}

		private static void Build(bool exportProject)
		{
			// Destination path
			string targetPath = PATH_BUILDS + "/" + _dateTime.ToString("yyMMdd") + "_" + FILENAME_VERSION_CODE;
			if (!exportProject) {
				if (EditorUserBuildSettings.buildAppBundle)
					targetPath += ".aab";
				else
					targetPath += ".apk";
			}

			// Backup if build already exists
			if (Directory.Exists(targetPath) || File.Exists(targetPath)) {
				string targetBackupPath = "";
				if (exportProject) {
					targetBackupPath = targetPath + "_backup";
					if (Directory.Exists(targetBackupPath))
						FileUtil.DeleteFileOrDirectory(targetBackupPath);
					FileUtil.ReplaceDirectory(targetPath, targetBackupPath);
				} else {
					targetBackupPath = targetPath + ".bak";
					if (File.Exists(targetBackupPath))
						FileUtil.DeleteFileOrDirectory(targetBackupPath);
					FileUtil.ReplaceFile(targetPath, targetBackupPath);
				}
				FileUtil.DeleteFileOrDirectory(targetPath); // Just make sure
			}
			Debug.Log("targetPath=" + targetPath);
			if (exportProject)
				Directory.CreateDirectory(targetPath);

#if UNITY_IOS
			PlayerSettings.iOS.buildNumber = _dateTime.ToString("yyMMdd");
#elif UNITY_ANDROID
			PlayerSettings.Android.bundleVersionCode = int.Parse(_dateTime.ToString("yyMMdd"));
			PlayerSettings.Android.keystorePass = _setting.keystorePassword;
			PlayerSettings.Android.keyaliasPass = _setting.keyAliasPassword;
#endif

			// Build
			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
				scenes = GetBuildScenes(), // Use current scenes
				locationPathName = targetPath,
				target = EditorUserBuildSettings.activeBuildTarget, // Use current target
				options = exportProject ? BuildOptions.AcceptExternalModificationsToPlayer : BuildOptions.None
			};

			var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

			if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded) {
				Debug.Log("Build successfully at: " + targetPath);
				EditorUtility.RevealInFinder(PATH_BUILDS);
			} else {
				Debug.Log("Build failed");
			}
		}

		private static string[] GetBuildScenes()
		{
			List<string> buildScenes = new List<string>();
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
				EditorBuildSettingsScene e = EditorBuildSettings.scenes[i];
				if (e != null && e.enabled)
					buildScenes.Add(e.path);
			}
			return buildScenes.ToArray();
		}

	}

}