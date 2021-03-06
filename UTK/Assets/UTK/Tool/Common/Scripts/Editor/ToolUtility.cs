﻿using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTK.Tool.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ToolUtility
    {
        public enum AssetType
        {
            Scene,
            Prefab,
            Material,
            Texture,
            Audio,
            Script,
            None  //Not found
        }

        public static string[] ImportableExtension = new string[]
        {
            ".fbx",
            ".jpg",
            ".png",
            ".bmp"
        };

        /// <summary>
        /// The Camera that is rendering last active SceneView.
        /// </summary>
        public static Camera SceneCamera
        {
            get => SceneView.lastActiveSceneView.camera;
        }

        /// <summary>
        /// The SceneView that was most recently in focus.
        /// </summary>
        public static SceneView LastActiveView
        {
            get => SceneView.lastActiveSceneView;
        }

        /// <summary>
        /// Get asset type from asset file path.
        /// </summary>
        /// <param name="path">Asset file path.</param>
        /// <returns><see cref="AssetType"/></returns>
        public static AssetType GetAssetType(string path)
        {
            switch (System.IO.Path.GetExtension(path))
            {
                case ".unity": return AssetType.Scene;
                case ".mat": return AssetType.Material;
                case ".prefab": return AssetType.Prefab;
                case ".cs": return AssetType.Script;
            }

            return AssetType.None;
        }

        /// <summary>
        /// Get or create tool config.
        /// </summary>
        /// <typeparam name="Type">Tool config type.</typeparam>
        /// <param name="directorypath">Path to save destination folder.</param>
        /// <param name="configpath">Path to save destination file.</param>
        /// <returns>Return Type of <see cref="ScriptableObject"/></returns>
        public static Type GetOrCreateToolConfig<Type>(string directorypath, string configpath)
            where Type : ScriptableObject

        {
            var config =
                  (Type)AssetDatabase.FindAssets("t:ScriptableObject", new string[] { directorypath })
                  .Select(id => AssetDatabase.GUIDToAssetPath(id))
                  .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(Type)))
                  .Where(c => c != null)
                  .FirstOrDefault();

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<Type>();
                Save(config, directorypath, configpath);
            }

            return config;
        }

        /// <summary>
        /// Save tool config.
        /// </summary>
        /// <typeparam name="Type">Tool config type.</typeparam>
        /// <param name="config">Save config instance.</param>
        /// <param name="directorypath">Path to save destination folder.</param>
        /// <param name="configpath">Path to save destination file.</param>
        public static void Save<Type>(Type config, string directorypath, string configpath)
              where Type : ScriptableObject
        {
            if (!Directory.Exists(directorypath))
            {
                Directory.CreateDirectory(directorypath);
            }

            if (!File.Exists(configpath))
            {
                AssetDatabase.CreateAsset(config, configpath);
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// Reset scene selections.
        /// </summary>
        public static void ResetSelection()
        {
            Selection.activeObject = null;
            Selection.objects = null;
        }

        /// <summary>
        /// Select the folder path child in <see cref="Application.dataPath"/>
        /// </summary>
        /// <param name="valid">Is valid path.</param>
        /// <returns>If valid return select path, else return null.</returns>
        public static string GetAssetsChildFolderUsingPanel(out bool valid)
        {
            var path = Application.dataPath;
            valid = true;

            path = EditorUtility.OpenFolderPanel("Please select a import destination.", Application.dataPath, "");

            if (!path.Contains(Application.dataPath))
            {
                valid = false;
                return null;
            }

            return path;
        }

        public static bool IsImportableAsset(string name)
        {
            return ImportableExtension.Contains(System.IO.Path.GetExtension(name));
        }
    }
}

