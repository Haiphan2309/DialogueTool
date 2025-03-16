using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace GDC.Utils.EditorTools
{
    public class EditorScene
    {
        public string name;
        public string path;

        public EditorScene(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }

    [ExecuteInEditMode]
    public class EditorPlayFromScene : EditorWindow
    {
        [SerializeField] int targetScene = 0;
        static List<EditorScene> scenes = new List<EditorScene>();
        private string waitScene = null;
        private Vector2 scrollPos;
        private Vector2 sceneScrollPos;
        private bool isPlayBtnClicked;
        private bool openPreviousScene;
        private string searchScene;

        [MenuItem("Tools/Play From Scene")]
        public static void Run()
        {
            var window = EditorWindow.GetWindow<EditorPlayFromScene>();
            window.minSize = new Vector2(850, 450);
            window.titleContent = new GUIContent("Play From Scene");
        }

        void OnEnable()
        {
            FilterScenes();
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        List<EditorScene> GetScenes()
        {
            scenes.Clear();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                scenes.Add(new EditorScene(AsSpacedCamelCase(Path.GetFileNameWithoutExtension(scene.path)), scene.path));
            }
            return scenes.OrderBy(x => x.name).ToList();
        }

        void OnDisable()
        {
            scenes.Clear();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        }

        void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Space(5);
            OpenSingleScene();
            GUILayout.Space(5);
            if (GUILayout.Button("Reset all", GUILayout.Height(30)))
            {
                Common.RemovePrefs();
                EditorSceneManager.OpenScene(scenes[0].path, OpenSceneMode.Single);
            }
            EditorGUILayout.EndScrollView();
        }

        private void OpenSingleScene()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
                GUILayout.Label("Search: ");
                EditorGUI.BeginChangeCheck();
                    searchScene = EditorGUILayout.TextField(searchScene);
                if (EditorGUI.EndChangeCheck())
                {
                    FilterScenes();
                }
                openPreviousScene = GUILayout.Toggle(openPreviousScene, "Open Previous Scenes on exit");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                    sceneScrollPos = GUILayout.BeginScrollView(sceneScrollPos);
                    targetScene = GUILayout.SelectionGrid(targetScene, scenes.Select(x => x.name).ToArray(), 2);
                    GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                    if (GUILayout.Button($"Play", GUILayout.MaxHeight(60)))
                    {
                        Common.SaveCurrentScenesToPrefs();
                        waitScene = scenes[targetScene].path;
                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                        isPlayBtnClicked = true;
                        EditorSceneManager.OpenScene(waitScene);
                        EditorApplication.isPlaying = true;
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Play Main", GUILayout.MaxHeight(60)))
                    {
                        Common.SaveCurrentScenesToPrefs();
                        waitScene = GetScenes().Where(x => x.name == "Main").First().path;
                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                        isPlayBtnClicked = true;
                        EditorSceneManager.OpenScene(waitScene, OpenSceneMode.Single);
                        EditorApplication.isPlaying = true;
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Play Home", GUILayout.MaxHeight(60)))
                    {
                        Common.SaveCurrentScenesToPrefs();
                        waitScene = GetScenes().Where(x => x.name == "Home").First().path;
                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                        isPlayBtnClicked = true;
                        EditorSceneManager.OpenScene(waitScene, OpenSceneMode.Single);
                        EditorApplication.isPlaying = true;
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Open", GUILayout.MaxHeight(60)))
                    {
                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                        EditorSceneManager.OpenScene(scenes[targetScene].path);
                        Common.RemovePrefs();
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Open Additive", GUILayout.MaxHeight(60)))
                    {
                        EditorSceneManager.OpenScene(scenes[targetScene].path, OpenSceneMode.Additive);
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Select", GUILayout.MaxHeight(60)))
                    {
                        var scene = AssetDatabase.LoadAssetAtPath(scenes[targetScene].path, typeof(object));
                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(scene);
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Open Build Settings", GUILayout.MaxHeight(60)))
                    {
                        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Reload Scenes", GUILayout.MaxHeight(60)))
                    {
                        GetScenes();
                    }
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void FilterScenes()
        {
            if (searchScene != null)
            {
                searchScene = searchScene.ToLower();
            }
            if (string.IsNullOrEmpty(searchScene))
            {
                scenes = GetScenes();
            }
            else
            {
                scenes = GetScenes().Where(x => Validate(x.path)).ToList();
            }
        }

        private bool Validate(string path)
        {
            var x = path.ToLower().Split('/').Last();
            return x.Contains(searchScene);
        }

        private static void OnPlayModeStateChange(PlayModeStateChange mode)
        {
            if (mode == PlayModeStateChange.EnteredEditMode)
            {
                var window = EditorWindow.GetWindow<EditorPlayFromScene>();
                if (!window.openPreviousScene) return;
                bool shouldOpenWaitScene = false;
                bool shouldNotLoadWaitScene = false;
                int sceneCount = EditorPrefs.GetInt("SceneCount");
                for (int i = 0; i < sceneCount; i++)
                {
                    string scenePath = EditorPrefs.GetString($"Scene{i}Path");
                    OpenSceneMode sceneMode = (OpenSceneMode)EditorPrefs.GetInt($"Scene{i}Mode");
                    if (scenePath == window.waitScene)
                    {
                        shouldOpenWaitScene = true;
                        if (sceneMode == OpenSceneMode.AdditiveWithoutLoading)
                            shouldNotLoadWaitScene = true;
                    }
                    if (sceneMode != OpenSceneMode.Single)
                    {
                        try {
                            EditorSceneManager.OpenScene(scenePath, sceneMode);
                        }
                        catch {}
                    }
                    else
                    {
                        try {
                            if (!string.IsNullOrEmpty(scenePath))
                            {
                                Scene sceneOpened = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                                EditorSceneManager.SetActiveScene(sceneOpened);
                            }
                        }
                        catch {}
                    }
                }
                if (window.isPlayBtnClicked)
                {
                    window.isPlayBtnClicked = false;
                    if (!shouldOpenWaitScene)
                    {
                        EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByPath(window.waitScene), true);
                    }
                    else {
                        if (shouldNotLoadWaitScene)
                        {
                            EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByPath(window.waitScene), false);
                        }
                    }
                }
                Common.RemovePrefs();
            }
        }
        public string AsSpacedCamelCase(string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length * 2);
            if (text.Length == 0) return "";
            sb.Append(char.ToUpper(text[0]));
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    sb.Append(' ');
                sb.Append(text[i]);
            }
            return sb.ToString();
        }
    }
}

#endif