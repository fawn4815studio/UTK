using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTK.Screenshot
{
    public class ScreenshotEditorConfig : ScriptableObject
    {
        public static readonly string CONFIGDIRECTORYPATH = "Assets/UTK/Config";
        public static readonly string CONFIGFILEPATH = "Assets/UTK/Config/ScreenshotEditorConfig.asset";
        public static readonly int DEFAULTRESOLUTIONWIDTH = 1024;
        public static readonly int DEFAULTRESOLUTIONHEIGHT = 1024;

        [SerializeField]
        string saveFolderPath;

        [SerializeField]
        int resolutionWidth;

        [SerializeField]
        int resolutionHeight;

        [SerializeField]
        int scale;

        public string SaveFolderPath { get => saveFolderPath; set => saveFolderPath = value; }
        public int ResolutionWidth { get => resolutionWidth; set => resolutionWidth = value; }
        public int ResolutionHeight { get => resolutionHeight; set => resolutionHeight = value; }
        public int Scale { get => scale; set => scale = value; }


        public static ScreenshotEditorConfig GetScreenshotEditorConfig()
        {
            var config =
                    (ScreenshotEditorConfig)AssetDatabase.FindAssets("t:ScriptableObject",new string[] {CONFIGDIRECTORYPATH})
                    .Select(id => AssetDatabase.GUIDToAssetPath(id))
                    .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(ScreenshotEditorConfig)))
                    .FirstOrDefault();

            if(config==null)
            {
                config = CreateInstance<ScreenshotEditorConfig>();
                Save(config);
            }

            return config;
        }

        private static void Save(ScreenshotEditorConfig config)
        {
            if(!Directory.Exists(CONFIGDIRECTORYPATH))
            {
                Directory.CreateDirectory(CONFIGDIRECTORYPATH);
            }

            if(!File.Exists(CONFIGFILEPATH))
            {
                AssetDatabase.CreateAsset(config, CONFIGFILEPATH);
                AssetDatabase.Refresh();
            }
          
        }
    }
}

