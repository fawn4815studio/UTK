using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTK.Tool.Screenshot
{
    public class ScreenshotEditorConfig : ScriptableObject
    {
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

        [SerializeField]
        bool isTransparent;

        public string SaveFolderPath { get { return saveFolderPath; } set { saveFolderPath = value; } }
        public int ResolutionWidth { get { return resolutionWidth; } set { resolutionWidth = value; } }
        public int ResolutionHeight { get { return resolutionHeight; } set { resolutionHeight = value; } }
        public int Scale { get { return scale; } set { scale = value; } }
        public bool IsTransparent { get { return isTransparent; } set { isTransparent = value; } }
    }
}

