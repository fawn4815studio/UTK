﻿using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTK.Tool.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigUtility
    {
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
    }
}

