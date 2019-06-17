using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace UTK.RecentFileViewer
{
    [System.Serializable]
    public class RecentOpenFileData
    {
        [SerializeField]
        string name;

        [SerializeField]
        string path;

        [SerializeField]
        string id;

        public string Id { get => id; }
        public string Name { get => name; }
        public string Path { get => path; }

        public RecentOpenFileData(string name, string path)
        {
            id = AssetDatabase.AssetPathToGUID(path);
            this.name = name;
            this.path = path;
        }
    }

    public class RecentFileViewerConfig : ScriptableObject
    {
        public static readonly string CONFIGDIRECTORYPATH = "Assets/UTK/Config";
        public static readonly string CONFIGFILEPATH = "Assets/UTK/Config/RecentFileViewerConfig.asset";

        [SerializeField]
        int queueLimit = 5;

        [SerializeField]
        List<RecentOpenFileData> recentSceneList = new List<RecentOpenFileData>();

        [SerializeField]
        List<RecentOpenFileData> recentPrefabList = new List<RecentOpenFileData>();

        public List<RecentOpenFileData> RecentSceneList { get => recentSceneList; }
        public List<RecentOpenFileData> RecentPrefabList { get => recentPrefabList; }
        public int QueueLimit { get => queueLimit; set => queueLimit = value; }

        public static RecentFileViewerConfig GetRecentFileViewerConfig()
        {
            var config =
                    (RecentFileViewerConfig)AssetDatabase.FindAssets("t:ScriptableObject", new string[] { CONFIGDIRECTORYPATH })
                    .Select(id => AssetDatabase.GUIDToAssetPath(id))
                    .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(RecentFileViewerConfig)))
                    .Where(c => c != null)
                    .FirstOrDefault();

            if (config == null)
            {
                config = CreateInstance<RecentFileViewerConfig>();
                Save(config);
            }

            return config;
        }

        private static void Save(RecentFileViewerConfig config)
        {
            if (!Directory.Exists(CONFIGDIRECTORYPATH))
            {
                Directory.CreateDirectory(CONFIGDIRECTORYPATH);
            }

            if (!File.Exists(CONFIGFILEPATH))
            {
                AssetDatabase.CreateAsset(config, CONFIGFILEPATH);
                AssetDatabase.Refresh();
            }

        }
    }
}

