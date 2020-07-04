using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTK.Runtime.Manager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTK.Runtime.Utility
{
    public class ScriptableTemplate<T> : ScriptableObject
        where T : ScriptableObject
    {
        private static readonly string SAVE_PATH = "Assets/UTK/Config/Resources/Template.asset";
        private static readonly string BUNDLE_NAME = "template";

        private static T instance;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR

                var path = Application.dataPath;
                path = path.Remove(path.IndexOf("Assets"));
                path = System.IO.Path.Combine(path, SAVE_PATH);

                if (!System.IO.File.Exists(path))
                {
                    instance = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(instance, SAVE_PATH);
                    EditorApplication.ExecuteMenuItem("UTK/AssetManager/Commands/Set bundle names to resources assets");
                }
#endif

                if (instance == null)
                {
                    instance = AssetManager.Instance.LoadSync<T>(BUNDLE_NAME);
                }

                return instance;
            }
        }

#if UNITY_EDITOR

        /*
        [MenuItem("UTK/Scriptable", false, 1)]
        static void CreateScriptableData()
        {
            var instance = AssetDatabase.LoadAssetAtPath(SAVE_PATH, typeof(ScriptableTemplate));
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<ScriptableTemplate>();
                AssetDatabase.CreateAsset(instance, SAVE_PATH);
            }
            Selection.activeObject = instance;
        }
        */

#endif
    }
}
