using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlockyBlock.Tools
{
    public class AutoBuild : MonoBehaviour
    {
        public enum ScreenResolution {FULL_SCREEN, WINDOWED}
        
        [MenuItem("Auto Build/Windows 64 Cheat")]
        public static void AutoBuildWin64_Cheat_FullScreen()
        {
            AutoBuildWin64(true, ScreenResolution.FULL_SCREEN, 1920, 1080);
        }
        [MenuItem("Auto Build/Windows 64 Release")]
        public static void AutoBuildWin64_Release_FullScreen()
        {
            AutoBuildWin64(false, ScreenResolution.FULL_SCREEN, 1920, 1080);
        }
        [ExecuteInEditMode]
        static void AutoBuildWin64(bool isCheat, ScreenResolution resolution, int width, int height)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            Screen.SetResolution(width, height, GetScreenMode(resolution));
            List<string> sceneArrays = new List<string>();
            int count = EditorBuildSettings.scenes.Length;
            for (int i = 0; i < count; i++)
            {
                string path = EditorBuildSettings.scenes[i].path;
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    sceneArrays.Add(path);
                }
            }
            
            string outFolder = isCheat ? "./_build/win64/cheat" : "./_build/win64/release";
            string productName = PlayerSettings.productName + (isCheat ? "_Cheat" : "_Release");

            CreateFolder(outFolder);

            buildPlayerOptions.scenes = sceneArrays.ToArray();
            buildPlayerOptions.locationPathName = outFolder + "/" + productName + ".exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            

            buildPlayerOptions.options = GetBuildOptions(BuildTargetGroup.Standalone, isCheat);

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
        static FullScreenMode GetScreenMode(ScreenResolution resolution)
        {
            switch (resolution)
            {
                case ScreenResolution.FULL_SCREEN:
                    return FullScreenMode.FullScreenWindow;
                case ScreenResolution.WINDOWED:
                    return FullScreenMode.Windowed;
                default:
                    return FullScreenMode.FullScreenWindow;
            }
        }
        static void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }
        static void DeleteFileOrFolder(string path)
        {
            string tmp = Application.dataPath + "/../" + path;
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(tmp);
                if (dirInfo.Exists)
                    dirInfo.Delete(true);

                FileInfo fileInfo = new FileInfo(tmp);
                if (fileInfo.Exists)
                    fileInfo.Delete();
            }
            catch (Exception)
            {
            }
        }
        static BuildOptions GetBuildOptions(BuildTargetGroup target, bool isCheat, List<string> customScriptingDefine = null)
        {
            if (isCheat)
                AddScriptingDefine(target, "ENABLE_CHEAT");
            else
                RemoveScriptingDefine(target, "ENABLE_CHEAT");
            if (customScriptingDefine != null)
            {
                foreach (var key in customScriptingDefine)
                {
                    AddScriptingDefine(target, key);
                }
            }

            BuildOptions options = BuildOptions.None;
            options |= BuildOptions.ShowBuiltPlayer;
            return options;
        }
        static void AddScriptingDefine(BuildTargetGroup buildTargetGroup, string define)
        {
            string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (current.Contains(define))
                return;
            string result = current + ";" + define;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, result);
        }
        static void RemoveScriptingDefine(BuildTargetGroup buildTargetGroup, string define)
        {
            string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!current.Contains(define))
                return;
            string result = current.Replace(define, String.Empty);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, result);
        }
    }
}
