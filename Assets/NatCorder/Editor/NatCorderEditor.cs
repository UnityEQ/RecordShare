/* 
*   NatCorder
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCorderU.Utilities {

    using UnityEditor;
    using UnityEditor.Callbacks;
    using System;
    using System.IO;

    #if UNITY_IOS
    using UnityEditor.iOS.Xcode;
    #endif

    public static class NatCorderEditor {

        private const string
		MicrophoneUsageKey = @"NSMicrophoneUsageDescription",
		LibraryUsageKey = @"NSPhotoLibraryUsageDescription",
		MicrophoneUsageDescription = @"Allow microphone input for video recording.", // Change this as necessary
		LibraryUsageDescription = @"Allow access to your photo library to save videos", // Change this as necessary
        API = "NATCORDER",
        CBridge = "INATCORDER_C",
		VersionNumber = "NATCORDER_10";

        [InitializeOnLoadMethod]
		static void SetGlobalDefines () {
			// Define C bridge dependency // Needed to compile AOT (IL2CPP)
			Func<BuildTargetGroup, bool> cdependency = (grp) => {
				if (grp == BuildTargetGroup.iOS) return true;
				return false;
			};
			// Define the build targets
			BuildTargetGroup[] groups = {
				BuildTargetGroup.Android,
				BuildTargetGroup.iOS,
				BuildTargetGroup.Standalone,
				BuildTargetGroup.WebGL,
				BuildTargetGroup.WSA
			};
			// Iterate and set
			foreach (var target in groups) {
				string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                if (!current.Contains(API)) current += (current.Equals(string.Empty) ? "" : ";") + API;
				if (!current.Contains(VersionNumber)) current += ";" + VersionNumber;
				if (cdependency(target) && !current.Contains(CBridge)) current += ";" + CBridge;
				else if (!cdependency(target) && current.Contains(CBridge)) current = current.Replace(";" + CBridge, string.Empty);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, current);
			}
		}


        #if UNITY_IOS

		[PostProcessBuild]
		static void LinkFrameworks (BuildTarget buildTarget, string path) {
			//Check that we're on iOS
			if (buildTarget != BuildTarget.iOS) return;
			//Get the project path
			string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			//Contruct a project reference
			PBXProject proj = new PBXProject();
			//Read it in
			proj.ReadFromString(File.ReadAllText(projPath));
			//Get the target name
			string target = proj.TargetGuidByName("Unity-iPhone");
			//Add AssetsLibrary.framework
			proj.AddFrameworkToProject(target, "AssetsLibrary.framework", true);
			// Add CoreImage.framework
			proj.AddFrameworkToProject(target, "CoreImage.framework", true);
			//Write out
			File.WriteAllText(projPath, proj.WriteToString());
		}

		[PostProcessBuild]
		static void SetPermissions (BuildTarget buildTarget, string path) {
			//Check that we're on iOS
			if (buildTarget != BuildTarget.iOS) return;
			//Get the plist path
			string plistPath = path + "/Info.plist";
			//Create plist document reference
			PlistDocument plist = new PlistDocument();
			//Read it in
			plist.ReadFromString(File.ReadAllText(plistPath));
			//Get the root
			PlistElementDict rootDictionary = plist.root;
			// Set microphone usage description
			rootDictionary.SetString(MicrophoneUsageKey, MicrophoneUsageDescription);
			// Set photo library usage description
			rootDictionary.SetString(LibraryUsageKey, LibraryUsageDescription);
			//Write out
			File.WriteAllText(plistPath, plist.WriteToString());
		}
		#endif
    }
}
#pragma warning restore 0162, 0429