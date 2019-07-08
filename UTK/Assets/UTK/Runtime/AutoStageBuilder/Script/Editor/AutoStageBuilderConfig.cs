using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTK.Runtime.AutoStageBuilder
{
    public class AutoStageBuilderConfig : ScriptableObject
    {
        public static readonly string CONFIGDIRECTORYPATH = "Assets/UTK/Config";
        public static readonly string CONFIGFILEPATH = "Assets/UTK/Config/AutoStageBuilderConfig.asset";

        [SerializeField]
        private string recentEditStageDataPath = null;

        [SerializeField]
        private string recentSaveDestinationPath = null;

        [SerializeField]
        private string recentLoadDestinationPath = null;

        /// <summary>
        /// True = When you start the tool, the stage data you edited last is automatically loaded.
        /// </summary>
        [SerializeField]
        private bool isAutoLoadStageData = false;

        #region Property
        public string RecentEditStageDataPath { get => recentEditStageDataPath; set => recentEditStageDataPath = value; }
        public string RecentSaveDestinationPath { get => recentSaveDestinationPath; set => recentSaveDestinationPath = value; }
        public string RecentLoadDestinationPath { get => recentLoadDestinationPath; set => recentLoadDestinationPath = value; }
        public bool IsAutoLoadStageData { get => isAutoLoadStageData; set => isAutoLoadStageData = value; }
        #endregion

        public static AutoStageBuilderConfig GetAutoStageBuilderConfig()
        {
            var config =
                    (AutoStageBuilderConfig)AssetDatabase.FindAssets("t:ScriptableObject", new string[] { CONFIGDIRECTORYPATH })
                    .Select(id => AssetDatabase.GUIDToAssetPath(id))
                    .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(AutoStageBuilderConfig)))
                    .Where(c => c != null)
                    .FirstOrDefault();

            if (config == null)
            {
                config = CreateInstance<AutoStageBuilderConfig>();
                Save(config);
            }

            return config;
        }

        private static void Save(AutoStageBuilderConfig config)
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
