using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UTK.RecentFileViewer
{
    public class RecentFileViewer : EditorWindow
    {
        static RecentFileViewer recentFileViewer;
        static bool isRegisterEvent = false;

        [SerializeField]
        RecentFileViewerConfig config;

        enum ViewerAssetType : int
        {
            Scene,
            Prefab
        }
        readonly string[] tabs = new string[] { "Scene", "Prefabs"};
        int currentTabIndex;

        Vector2 sceneScrollPos;
        Vector2 prefabScrollPos;

        Queue<RecentOpenFileData> recentSceneQueue = null;
        Queue<RecentOpenFileData> recentPrefabQueue = null;

        public void AddSceneQueue(Scene scene)
        {
            //Check if already registered in queue.
            if (recentSceneQueue.Count(s=>s.Id == AssetDatabase.AssetPathToGUID(scene.path)) != 0)
            {
                return;
            }

            var d = new RecentOpenFileData(scene.name, scene.path);
            recentSceneQueue.Enqueue(d);
            config.RecentSceneList.Add(d);

            if(config.RecentSceneList.Count > RecentFileViewerConfig.QUEUELIMIT)
            {
                config.RecentSceneList.Remove(recentSceneQueue.Dequeue());
            }

            //Record that there has been a change.
            EditorUtility.SetDirty(config);
        }

        public void AddPrefabQueue(GameObject prefab)
        {
            var projectprefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
            string path = AssetDatabase.GetAssetPath(projectprefab);

            //Check if already registered in queue.
            if (recentPrefabQueue.Count(s => s.Id == AssetDatabase.AssetPathToGUID(path)) != 0)
            {
                return;
            }

            var d = new RecentOpenFileData(projectprefab.name, path);
            recentPrefabQueue.Enqueue(d);
            config.RecentPrefabList.Add(d);

            if (config.RecentPrefabList.Count > RecentFileViewerConfig.QUEUELIMIT)
            {
                config.RecentPrefabList.Remove(recentPrefabQueue.Dequeue());
            }

            //Record that there has been a change.
            EditorUtility.SetDirty(config);
        }

        [MenuItem("UTK/RecentFileViewer",false,12)]
        static void Open()
        {
            if (recentFileViewer == null)
            {
                recentFileViewer = CreateInstance<RecentFileViewer>();
            }
            recentFileViewer.config = RecentFileViewerConfig.GetRecentFileViewerConfig();
            recentFileViewer.RegisterEvent();
            recentFileViewer.InitQueue();

            recentFileViewer.titleContent.text = "RecentFileViewer";
            recentFileViewer.Show();
         
        }

        #region Internal

        private void OnEnable()
        {
            if(recentFileViewer==null)
            {
                recentFileViewer = this;
                config = RecentFileViewerConfig.GetRecentFileViewerConfig();
                RegisterEvent();
                InitQueue();
            }
        }

        void OnDestroy()
        {
            RemoveEvent();
        }

        void OnGUI()
        {
            currentTabIndex = GUILayout.Toolbar(currentTabIndex,tabs);

            if ((int)ViewerAssetType.Scene == currentTabIndex)
            {

                sceneScrollPos = EditorGUILayout.BeginScrollView(sceneScrollPos, GUI.skin.box);
                {
                  
                    foreach (var s in recentSceneQueue)
                    {
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.LabelField(s.Path);
                        if (GUILayout.Button(s.Name , GUILayout.Width(100)))
                        {
                            EditorSceneManager.OpenScene(s.Path);
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                }

                EditorGUILayout.EndScrollView();
            }
            else if((int)ViewerAssetType.Prefab == currentTabIndex)
            {

                prefabScrollPos = EditorGUILayout.BeginScrollView(prefabScrollPos , GUI.skin.box);
                {

                    foreach (var p in recentPrefabQueue)
                    {
                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.LabelField(p.Path);
                        if (GUILayout.Button(p.Name, GUILayout.Width(100)))
                        {
                            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(p.Path));
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                }

                EditorGUILayout.EndScrollView();
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

        void InitQueue()
        {
            if (config == null) return;

            if(recentPrefabQueue ==null)
            {
                recentPrefabQueue = new Queue<RecentOpenFileData>();
                foreach(var d in config.RecentPrefabList)
                {
                    recentPrefabQueue.Enqueue(d);
                }
            }

            if(recentSceneQueue==null)
            {
                recentSceneQueue = new Queue<RecentOpenFileData>();
                foreach (var d in config.RecentSceneList)
                {
                    recentSceneQueue.Enqueue(d);
                }
            }
        }

        static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            recentFileViewer.AddSceneQueue(scene);
        }

        static void OnPrefabUpdated(GameObject instance)
        {
            recentFileViewer.AddPrefabQueue(instance);
        }

        #endregion
    }

}
