using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UTK.Tool.Common;

namespace UTK.Tool.RecentFileViewer
{
    public class RecentFileViewer : EditorWindow
    {
        static readonly string CONFIGDIRECTORYPATH = "Assets/UTK/Config";
        static readonly string CONFIGFILEPATH = "Assets/UTK/Config/RecentFileViewerConfig.asset";

        static RecentFileViewer recentFileViewer;
        static bool isRegisterEvent = false;

        [SerializeField]
        RecentFileViewerConfig config;

        enum ViewerTabType : int
        {
            Scene,
            Prefab,
            Scripts,
            Materials,
            Textures,
            Option
        }
        readonly string[] tabs = new string[] { "Scene", "Prefabs", "Scripts", "Materials", "Textures", "Options" };
        int currentTabIndex;

        Vector2 sceneScrollPos;
        Vector2 prefabScrollPos;

        public void AddSceneToQueueList(Scene scene)
        {
            //Check if already registered in list.
            if (config.RecentSceneList.Count(s => s.Id == AssetDatabase.AssetPathToGUID(scene.path)) != 0)
            {
                //Sort list.
                var top = config.RecentSceneList.First(s => s.Id == AssetDatabase.AssetPathToGUID(scene.path));
                SortList(config.RecentSceneList, top);
                return;
            }

            var d = new RecentOpenFileData(scene.name, scene.path);
            SortList(config.RecentSceneList, d);

            if (config.RecentSceneList.Count > config.QueueLimit)
            {
                config.RecentSceneList.RemoveAt(config.RecentSceneList.Count - 1);
            }

            //Record that there has been a change.
            EditorUtility.SetDirty(config);
        }

        public void AddAssetToQueueList(Object assetobject)
        {
            string path = string.Empty;
            path = AssetDatabase.GetAssetPath(assetobject);

            if (path == string.Empty)
            {
                //Returns the corresponding asset object of source, or null if it can't be found.
                var projectprefab = PrefabUtility.GetCorrespondingObjectFromSource(assetobject);
                path = AssetDatabase.GetAssetPath(projectprefab);
            }

            List<RecentOpenFileData> activelist = null;
            switch (ConfigUtility.GetAssetType(path))
            {
                case ConfigUtility.AssetType.Material: activelist = config.RecentMaterialList; break;
                case ConfigUtility.AssetType.Prefab: activelist = config.RecentPrefabList; break;
                case ConfigUtility.AssetType.Script: activelist = config.RecentScriptList; break;
                case ConfigUtility.AssetType.Texture: activelist = config.RecentTextureList; break;
            }

            if (activelist == null) return;

            //Check if already registered in queue.
            if (activelist.Count(s => s.Id == AssetDatabase.AssetPathToGUID(path)) != 0)
            {
                //Sort list.
                var top = activelist.First(s => s.Id == AssetDatabase.AssetPathToGUID(path));
                SortList(activelist, top);
                return;
            }

            var d = new RecentOpenFileData(assetobject.name, path);
            SortList(activelist, d);

            if (config.RecentPrefabList.Count > config.QueueLimit)
            {
                activelist.RemoveAt(activelist.Count - 1);
            }

            //Record that there has been a change.
            EditorUtility.SetDirty(config);
        }

        [MenuItem("UTK/Utility/RecentFileViewer", false, 12)]
        static void Open()
        {
            if (recentFileViewer == null)
            {
                recentFileViewer = CreateInstance<RecentFileViewer>();
            }
            recentFileViewer.config = ConfigUtility.GetOrCreateToolConfig<RecentFileViewerConfig>(CONFIGDIRECTORYPATH, CONFIGFILEPATH);
            recentFileViewer.RegisterEvent();

            recentFileViewer.titleContent.text = "RecentFileViewer";
            recentFileViewer.Show();

        }

        #region Internal

        void OnEnable()
        {
            if (recentFileViewer == null)
            {
                recentFileViewer = this;
                config = ConfigUtility.GetOrCreateToolConfig<RecentFileViewerConfig>(CONFIGDIRECTORYPATH, CONFIGFILEPATH);
                RegisterEvent();
            }
        }

        void OnDestroy()
        {
            RemoveEvent();
        }

        void OnGUI()
        {
            currentTabIndex = GUILayout.Toolbar(currentTabIndex, tabs);

            if ((int)ViewerTabType.Scene == currentTabIndex)
            {

                sceneScrollPos = EditorGUILayout.BeginScrollView(sceneScrollPos, GUI.skin.box);
                {
                    DrawQueueList(config.RecentSceneList);
                }

                EditorGUILayout.EndScrollView();
            }
            else if ((int)ViewerTabType.Prefab == currentTabIndex)
            {

                prefabScrollPos = EditorGUILayout.BeginScrollView(prefabScrollPos, GUI.skin.box);
                {
                    DrawQueueList(config.RecentPrefabList);
                }

                EditorGUILayout.EndScrollView();
            }
            else if ((int)ViewerTabType.Scripts == currentTabIndex)
            {

                prefabScrollPos = EditorGUILayout.BeginScrollView(prefabScrollPos, GUI.skin.box);
                {
                    DrawQueueList(config.RecentScriptList);
                }

                EditorGUILayout.EndScrollView();
            }
            else if ((int)ViewerTabType.Materials == currentTabIndex)
            {

                prefabScrollPos = EditorGUILayout.BeginScrollView(prefabScrollPos, GUI.skin.box);
                {
                    DrawQueueList(config.RecentMaterialList);
                }

                EditorGUILayout.EndScrollView();
            }
            else if ((int)ViewerTabType.Textures == currentTabIndex)
            {

                prefabScrollPos = EditorGUILayout.BeginScrollView(prefabScrollPos, GUI.skin.box);
                {
                    DrawQueueList(config.RecentTextureList);
                }

                EditorGUILayout.EndScrollView();
            }
            else if ((int)ViewerTabType.Option == currentTabIndex)
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Clear all queue", GUILayout.Width(150)))
                {
                    config.RecentSceneList.Clear();
                    config.RecentPrefabList.Clear();
                    config.RecentScriptList.Clear();
                    config.RecentMaterialList.Clear();
                    config.RecentTextureList.Clear();
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Clear scene queue", GUILayout.Width(150)))
                {
                    config.RecentSceneList.Clear();
                }

                if (GUILayout.Button("Clear prefab queue", GUILayout.Width(150)))
                {
                    config.RecentPrefabList.Clear();
                }

                if (GUILayout.Button("Clear script queue", GUILayout.Width(150)))
                {
                    config.RecentScriptList.Clear();
                }

                if (GUILayout.Button("Clear material queue", GUILayout.Width(150)))
                {
                    config.RecentMaterialList.Clear();
                }

                if (GUILayout.Button("Clear texture queue", GUILayout.Width(150)))
                {
                    config.RecentTextureList.Clear();
                }

                EditorGUILayout.Space();

                config.QueueLimit = EditorGUILayout.IntField("Queue limit", config.QueueLimit, GUILayout.Width(250));
            }
        }
   
        void RegisterEvent()
        {
            if (isRegisterEvent) return;
            isRegisterEvent = true;
            PrefabUtility.prefabInstanceUpdated += OnPrefabUpdated;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        void RemoveEvent()
        {
            PrefabUtility.prefabInstanceUpdated -= OnPrefabUpdated;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            isRegisterEvent = false;
        }

        void DrawQueueList(List<RecentOpenFileData> datas)
        {
            foreach (var p in datas)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField(p.Name);

                    if (GUILayout.Button("Open", GUILayout.Width(100)))
                    {
                        switch (ConfigUtility.GetAssetType(p.Path))
                        {
                            case ConfigUtility.AssetType.Scene:
                                EditorSceneManager.OpenScene(p.Path);
                                break;

                            case ConfigUtility.AssetType.Prefab:
                                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(p.Path));
                                break;

                            case ConfigUtility.AssetType.Material:
                                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Material>(p.Path));
                                break;

                            case ConfigUtility.AssetType.Texture:
                                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Texture>(p.Path));
                                break;

                            case ConfigUtility.AssetType.Audio:
                                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<AudioClip>(p.Path));
                                break;
                            case ConfigUtility.AssetType.Script:
                                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(p.Path));
                                break;
                        }

                        break;
                    }
                    if (GUILayout.Button("Select", GUILayout.Width(100)))
                    {
                        var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(p.Path);
                        if (obj) EditorGUIUtility.PingObject(obj);
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void SortList(List<RecentOpenFileData> datas, RecentOpenFileData top)
        {
            var temp = datas.ToArray();
            datas.Clear();
            datas.Add(top);
            foreach (var s in temp)
            {
                if (s != top)
                {
                    datas.Add(s);
                }
            }
        }

        static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            recentFileViewer.AddSceneToQueueList(scene);
        }

        static void OnPrefabUpdated(GameObject instance)
        {
            recentFileViewer.AddAssetToQueueList(instance);
        }

        [OnOpenAsset(0)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            recentFileViewer.AddAssetToQueueList(obj);

            //Since we want to call the original open processing, the result of this function must return false
            return false;
        }

        #endregion
    }

}
