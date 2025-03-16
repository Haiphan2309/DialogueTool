#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GDC.Utils.EditorTools
{
    [ExecuteInEditMode]
    public static partial class Common
    {
        public static void SaveCurrentScenesToPrefs()
        {
            int sceneCount = EditorSceneManager.sceneCount;
            EditorPrefs.SetInt("SceneCount", sceneCount);
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = EditorSceneManager.GetSceneAt(i);
                OpenSceneMode sceneMode;
                if (scene.isLoaded)
                    sceneMode = OpenSceneMode.Additive;
                else
                    sceneMode = OpenSceneMode.AdditiveWithoutLoading;
                if (EditorSceneManager.GetActiveScene() == scene)
                    sceneMode = OpenSceneMode.Single;
                EditorPrefs.SetString($"Scene{i}Path", scene.path);
                EditorPrefs.SetInt($"Scene{i}Mode", (int)sceneMode);
            }
        }
   
        public static void RemovePrefs()
        {
            int sceneCount = EditorPrefs.GetInt("SceneCount");
            for (int i = 0; i < sceneCount; i++)
            {
                EditorPrefs.DeleteKey($"Scene{i}Path");
                EditorPrefs.DeleteKey($"Scene{i}Mode");
            }
        }

        public static void DrawUILine(int thickness = 1, int padding = 10)
        {
            DrawUILine(Color.gray, thickness, padding);
        }

        public static void DrawUILine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y += padding/2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static bool IsAsciiLetter(this char c)
        {
            return (c >= 97 && c <= 122) || (c >= 65 && c <= 90);
        }

        public static bool IsAsciiNumber(this char c)
        {
            return c >= 48 && c <= 57;
        }

        public static bool IsAsciiLetterOrNumber(this char c)
        {
            return c.IsAsciiLetter() || c.IsAsciiNumber();
        }

        public static string ToAbsolutePath(this string path)
        {
            // this check works on Windows only
            if (path.Contains(":")) return path;
            return Application.dataPath.Replace("Assets", path);
        }

        public static string ToRelativePath(this string path)
        {
            if (!path.Contains("Assets"))
            {
                return path;
            }
            var paths = path.Split("Assets").Skip(1).ToList();
            if (paths.Count == 1)
            {
                return "Assets" + paths[0];
            }
            else
            {
                return string.Join("Assets", paths);
            }
        }

        public static string GetFileName(this string path, bool withExtension = true)
        {
            if (!withExtension)
            {
                path = path.Split(".")[0];
            }
            if (path.Contains(Path.AltDirectorySeparatorChar))
            {
                return path.Split(Path.AltDirectorySeparatorChar)[^1];
            }
            return path.Split(Path.DirectorySeparatorChar)[^1];
        }

        public static DirectoryInfo CombineInfo(this DirectoryInfo current, string path)
        {
            return new DirectoryInfo(Path.Combine(current.FullName, path));
        }

        public static DirectoryInfo CombineInfo(this DirectoryInfo current, params string[] path)
        {
            return new DirectoryInfo(Path.Combine(current.FullName,  Path.Combine(path)));
        }

        public static FileInfo CombineInfoToFile(this DirectoryInfo current, string path)
        {
            return new FileInfo(Path.Combine(current.FullName, path));
        }

        public static string CombineToPath(this DirectoryInfo current, string path)
        {
            return Path.Combine(current.FullName, path);
        }

        public static void ClearContents(this DirectoryInfo current)
        {
            FileInfo[] fileNames = current.GetFiles();

            for (int i = 0; i < fileNames.Length; i++)
            {
                fileNames[i].Delete();
            }

            Debug.Log($"Cleared {current.FullName}");
        }

        public static string CopyUpdateFile(string sourceFilePath, string destinationFilePath)
        {
            FileInfo srcFileInfo = new FileInfo(sourceFilePath);
            FileInfo destFileInfo = new FileInfo(destinationFilePath);
            bool isDifferentFile = System.DateTime.Compare(srcFileInfo.LastWriteTimeUtc, destFileInfo.LastWriteTimeUtc) != 0;

            if (isDifferentFile)
            {
                File.Copy(sourceFilePath, destinationFilePath, true);
            }

            AssetDatabase.Refresh();

            return destFileInfo.FullName;
        }
        public static void Log(string logText, ref StringBuilder sb, bool appendNewLine = true)
        {
            if (appendNewLine)
            {
                if (!logText.EndsWith("\n"))
                {
                    logText += "\n";
                }
            }
            Debug.Log(logText);
            sb.Append(logText);
        }

        public static void LogWarning(string logText, ref StringBuilder sb, bool appendNewLine = true, bool richText = true)
        {
            if (appendNewLine)
            {
                if (!logText.EndsWith("\n"))
                {
                    logText += "\n";
                }
            }
            Debug.LogWarning(logText);
            if (richText)
            {
                sb.Append($"<color=yellow>{logText}</color>");
            }
            else
            {
                sb.Append(logText);
            }
        }

        public static void LogError(string logText, ref StringBuilder sb, bool appendNewLine = true, bool richText = true)
        {
            if (appendNewLine)
            {
                if (!logText.EndsWith("\n"))
                {
                    logText += "\n";
                }
            }
            Debug.LogError(logText);
            if (richText)
            {
                sb.Append($"<color=red>{logText}</color>");
            }
            else
            {
                sb.Append(logText);
            }
        }

        public static string SetImportTextureType(string assetPath, TextureImporterType type = TextureImporterType.Sprite)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            importer.textureType = type;
            AssetDatabase.WriteImportSettingsIfDirty(assetPath);
            AssetDatabase.Refresh();
            return assetPath;
        }

        public static string[] ExtractFbxMaterials(string assetPath, string destFolder, string prefix)
        {
            List<string> extractedMaterialPaths = new List<string>();
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            HashSet<string> hashSet = new HashSet<string>();
            foreach (var obj in assets)
            {
                if (obj.GetType() == typeof(Material))
                {
                    if (obj.name.Equals("No Name")) continue;
                    string destName = obj.name.Split('_').Last();
                    string path = Path.Combine(destFolder, $"{prefix}_{destName}.mat");

                    string newPath = AssetDatabase.GenerateUniqueAssetPath(path);
                    if (newPath != path)
                    {
                        AssetDatabase.DeleteAsset(path);
                        AssetDatabase.Refresh();
                    }
                    string value = AssetDatabase.ExtractAsset(obj, path);
                    if (string.IsNullOrEmpty(value))
                    {
                        hashSet.Add(assetPath);
                        extractedMaterialPaths.Add(path);
                    }
                }
            }
            foreach (string item2 in hashSet)
            {
                AssetDatabase.WriteImportSettingsIfDirty(item2);
                AssetDatabase.ImportAsset(item2, ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.Refresh();

            return extractedMaterialPaths.ToArray();
        }

        public static T CreateScriptableObjectIfNotExist<T>(string assetPath) where T : ScriptableObject
        {
            assetPath = assetPath.ToRelativePath();
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.Refresh();
            }
            return asset;
        }
    }
}
#endif