using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UTK.Runtime.Manager;
using UTK.Runtime.Utility;

namespace UTK.Runtime
{
    [System.Serializable]
    public class ResourceList : ScriptableObject
    {
        [System.Serializable]
        public class ResourcePath
        {
            public string bundleName;
            public string path; //Resources folder path
        }

        public List<ResourcePath> resourcePaths;

        public static readonly string SAVE_PATH = "Assets/UTK/Config/Resources/ResourceList.asset";
        public static readonly string RESOURCES_PATH = "ResourceList";

        private static ResourceList instance;

        public static ResourceList Instance
        {
            get
            {
#if UNITY_EDITOR

                var path = Application.dataPath;
                path = path.Remove(path.IndexOf("Assets"));
                path = System.IO.Path.Combine(path, SAVE_PATH);
                if (!System.IO.File.Exists(path))
                {
                    instance = ScriptableObject.CreateInstance<ResourceList>();
                    AssetDatabase.CreateAsset(instance, SAVE_PATH);
                    EditorApplication.ExecuteMenuItem("UTK/AssetManager/Commands/Set bundle names to resources assets");
                }
#endif

                if (instance == null)
                {
                    instance = Resources.Load<ResourceList>(RESOURCES_PATH);
                }

                return instance;
            }
        }


    }
}

