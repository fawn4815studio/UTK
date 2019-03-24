using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTK.RecentFileViewer
{
    public class RecentFileViewerConfig : ScriptableObject
    {
        public static readonly string CONFIGDIRECTORYPATH = "Assets/UTK/Config";
        public static readonly string CONFIGFILEPATH = "Assets/UTK/Config/RecentFileViewerConfig.asset";

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

