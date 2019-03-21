using System.IO;
using UnityEditor;
using UnityEngine;

namespace UTK
{
    public class ScreenshotEditor : EditorWindow
    {
        static ScreenshotEditor screenshotEditor;

        [MenuItem("UTK/ScreenshotEditor")]
        static void Open()
        {
            if (screenshotEditor == null)
            {
                screenshotEditor = CreateInstance<ScreenshotEditor>();
            }
            screenshotEditor.ShowUtility();
        }

        [MenuItem("UTK/Quick Screenshot")]
        static void QuickScreenshot()
        {
            var name = "QuickScreenshot";
            var extension = ".png";

            var path = name + extension;
            var count = 0;

            while(File.Exists(path))
            {
                count++;
                path = string.Format("{0}_{1}{2}", name, count, extension);
            }

            ScreenCapture.CaptureScreenshot(path);
        }

        #region Internal

        void OnGUI()
        {
             
        }

        #endregion
    }

}
