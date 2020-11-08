using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

/// <summary>
/// A sample build post processor for iOS.
/// Source: https://www.jianshu.com/p/6bed4fc32aeb
/// </summary>
public class BuildPostprocessor : ScriptableObject
{

	internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
			Directory.Delete(dstPath);
		if (File.Exists(dstPath))
			File.Delete(dstPath);

		Directory.CreateDirectory(dstPath);

		foreach (var file in Directory.GetFiles(srcPath))
			File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

		foreach (var dir in Directory.GetDirectories(srcPath))
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
	}

	#if UNITY_IOS

	internal static void AddLibToProject(PBXProject inst, string targetGuid, string lib)
	{
		string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
		inst.AddFileToBuild(targetGuid, fileGuid);
	}

	#endif

	[PostProcessBuild(999)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
#if UNITY_IOS
		if (target == BuildTarget.iOS) {

			//Debug.Log("Starting build post processor...");

			// Read PBX project.
			string pbxPath = PBXProject.GetPBXProjectPath(path);
			PBXProject project = new PBXProject();
			project.ReadFromFile(pbxPath);

#if UNITY_2019_3_OR_NEWER
			string targetGuid = project.GetUnityMainTargetGuid();
			//var manager = new ProjectCapabilityManager(path, "Entitlements.entitlements", targetGuid: targetGuid);
#else
            string targetName = PBXProject.GetUnityTargetName();
            string targetGuid = project.TargetGuidByName(targetName);
			//var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", targetName);
#endif

			// Set a custom link flag
			//project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");

			// Disable Bitcode
			//Debug.Log("Disabling bitcode...");
			//project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

			// Add user packages to project
			//CopyAndReplaceDirectory("NativeAssets/TestLib.bundle", Path.Combine(path, "Frameworks/TestLib.bundle"));
			//project.AddFileToBuild(targetGuid, project.AddFile("Frameworks/TestLib.bundle", "Frameworks/TestLib.bundle", PBXSourceTree.Source));

			// Add system frameworks
			//project.AddFrameworkToProject(targetGuid, "AssetsLibrary.framework", false);
			//project.AddFrameworkToProject(targetGuid, "ReplayKit.framework", true);

			// Add custom libraies
			//project.AddFrameworkToProject(targetGuid, "SweatyChair/Kaiser/Plugins/iOS/IOS_IntegratedSDK.framework", false);
			//project.AddFrameworkToProject(targetGuid, "SweatyChair/Kaiser/Plugins/iOS/IOS_KNewSDK.framework", false);

			// Add system libraries
			//AddLibToProject(project, targetGuid, "libxml2.tbd");

			// Add our framework directory to the framework include path
			//project.SetBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
			//project.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");

			// Apply to project
			//project.WriteToFile(path);

			// Add capabilities
			//manager.AddSignInWithApple();
			//manager.AddGameCenter();
			//manager.AddInAppPurchase();
			//manager.AddKeychainSharing(new string[] { "$(AppIdentifierPrefix)com.my.appid" });
			//manager.AddPushNotifications(false);
			//manager.WriteToFile();

			// Modify Info.plist

			//var plistPath = Path.Combine(builtProjectPath, "Info.plist");
			//var plist = new PlistDocument();
			//plist.ReadFromFile(plistPath);

			// Insert URL Scheme to Info.plist
			//plist.root.SetString("NSCalendarsUsageDescription", "Clendars is not required, please contact developer if iOS requiring it.");
			//plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
			//plist.WriteToFile(plistPath);

			// Insert URL Scheme to Info.plist
			//var array = plist.root.CreateArray("CFBundleURLTypes");
			// Insert dict
			//var urlDict = array.AddDict();
			//urlDict.SetString("CFBundleTypeRole", "Editor");
			// Insert array
			//var urlInnerArray = urlDict.CreateArray("CFBundleURLSchemes");
			//urlInnerArray.AddString("blablabla");
			// Apply to plist
			//plist.WriteToFile(plistPath);

			// Insert codes
			// Read UnityAppController.mm
			//string unityAppControllerPath = pathToBuiltProject + "/Classes/UnityAppController.mm";
			//XClass UnityAppController = new XClass(unityAppControllerPath);

			// Insert a single line after a specific code
			//UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"", "#import <UMSocialCore/UMSocialCore.h>");

			// Insert multiple lines after a specific code
			//string newCode = "\n" +
			//	"    [[UMSocialManager defaultManager] openLog:YES];\n" +
			//	"    [UMSocialGlobal shareInstance].type = @\"u3d\";\n" +
			//	"    [[UMSocialManager defaultManager] setUmSocialAppkey:@\"" + "\"];\n" +
			//	"    [[UMSocialManager defaultManager] setPlaform:UMSocialPlatformType_WechatSession appKey:@\"" + "\" appSecret:@\"" + "\" redirectURL:@\"http://mobile.umeng.com/social\"];\n" +
			//	"    \n"
			//	;
			//UnityAppController.WriteBelow("// if you wont use keyboard you may comment it out at save some memory", newCode);

		}
#endif

	}

}