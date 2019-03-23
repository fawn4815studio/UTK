using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UTK.Screenshot
{
    public class ScreenshotEditor : EditorWindow
    {
        static ScreenshotEditor screenshotEditor;

        [SerializeField]
        ScreenshotEditorConfig config;
        GameObject targetCamera;

        [MenuItem("UTK/ScreenshotEditor")]
        static void Open()
        {
            if (screenshotEditor == null)
            {
                screenshotEditor = CreateInstance<ScreenshotEditor>();
            }

            screenshotEditor.config = ScreenshotEditorConfig.GetScreenshotEditorConfig();

            screenshotEditor.ShowUtility();
        }

        [MenuItem("UTK/Quick Screenshot")]
        static void QuickScreenshot()
        {
            ScreenCapture.CaptureScreenshot(GetScreenshotFileNameFromDateTime());
        }

        #region Internal

        void OnGUI()
        {
            var path = config.SaveFolderPath;
            EditorGUILayout.TextField("Save destination",path);
            if (GUILayout.Button("Select Folder"))
            {
                config.SaveFolderPath = EditorUtility.OpenFolderPanel("Please select a save destination.", "", "");
            }
            if (GUILayout.Button("Open folder"))
            {
                if(Application.platform == RuntimePlatform.WindowsEditor)
                {
                    System.Diagnostics.Process.Start(config.SaveFolderPath);
                }
                else
                {
                    EditorUtility.RevealInFinder(config.SaveFolderPath);
                }
             
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Camera");
                targetCamera = (GameObject)EditorGUILayout.ObjectField(targetCamera,typeof(GameObject),true);

                if(targetCamera!=null && targetCamera.GetComponent<Camera>()==null){
                    targetCamera = null;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Resolution");
                config.ResolutionWidth  = EditorGUILayout.IntField(config.ResolutionWidth);
                config.ResolutionHeight = EditorGUILayout.IntField(config.ResolutionHeight);
            }
            EditorGUILayout.EndHorizontal();

            config.Scale = EditorGUILayout.IntSlider("Scale", config.Scale,1, 10);

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Options");
                if (GUILayout.Button("Set resolution to screen size"))
                {
                    config.ResolutionWidth = Screen.currentResolution.width;
                    config.ResolutionHeight = Screen.currentResolution.height;
                }

                var buttonexplain = string.Format("Set default resolution ( {0}x{1} )", ScreenshotEditorConfig.DEFAULTRESOLUTIONWIDTH, ScreenshotEditorConfig.DEFAULTRESOLUTIONHEIGHT);
                if (GUILayout.Button(buttonexplain))
                {
                    config.ResolutionWidth =  ScreenshotEditorConfig.DEFAULTRESOLUTIONWIDTH;
                    config.ResolutionHeight = ScreenshotEditorConfig.DEFAULTRESOLUTIONHEIGHT;
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var explain = string.Format("Screenshot taken size : {0}x{1} px", config.ResolutionWidth * config.Scale, config.ResolutionHeight * config.Scale);
            GUILayout.Label(explain);
            if (GUILayout.Button("Take Screenshot",GUILayout.Width(200),GUILayout.Height(50)))
            {
                if(PrepareScreenshot())
                {
                    TakeScreenshot();
                }
            }

        }

        bool PrepareScreenshot()
        {
            if (targetCamera == null)
            {
                EditorUtility.DisplayDialog("ScreenshotEditor", "Failed take screenshot because targetCamera is null.", "Ok");
                return false;
            }

            if (config.SaveFolderPath == null || !Directory.Exists(config.SaveFolderPath))
            {
                EditorUtility.DisplayDialog("ScreenshotEditor", "Failed take screenshot because save destination is invalid.", "Ok");
                return false;
            }

            if (config.ResolutionWidth <= 0 || config.ResolutionHeight <= 0)
            {
                config.ResolutionWidth = ScreenshotEditorConfig.DEFAULTRESOLUTIONWIDTH;
                config.ResolutionHeight = ScreenshotEditorConfig.DEFAULTRESOLUTIONHEIGHT;
            }

            return true;
        }

        void TakeScreenshot()
        {
            var camera = targetCamera.GetComponent<Camera>();
            var width = config.ResolutionWidth * config.Scale;
            var height = config.ResolutionHeight * config.Scale;
            var current = camera.targetTexture;

            Texture2D ss = new Texture2D(width,height,TextureFormat.RGB24, false);
            RenderTexture rt = new RenderTexture(width,height,24);
            RenderTexture.active = rt;

            camera.targetTexture = rt;
            camera.Render();

            ss.ReadPixels(new Rect(0, 0,width,height),0, 0);
            ss.Apply();

            byte[] bytes = ss.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(config.SaveFolderPath,GetScreenshotFileNameFromDateTime()),bytes);

            DestroyImmediate(ss);
            camera.targetTexture = current;
        }

        static string GetScreenshotFileNameFromDateTime()
        {
            var name = string.Format("{0}_{1}.png", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            name = name.Replace("/", "_");
            name = name.Replace(":", "_");
            return name;
        }

        #endregion
    }

}
