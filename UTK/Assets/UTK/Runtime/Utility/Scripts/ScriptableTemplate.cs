using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTK.Runtime.Utility
{
    public class ScriptableTemplate : ScriptableObject
    {
        /// <summary>
        /// Change to a suitable storage location.
        /// </summary>
        private static readonly string SAVE_PATH = "Assets/UTK/Config/ScriptableTemplate.asset";
        private static ScriptableTemplate instance;

#if UNITY_EDITOR
        [MenuItem("UTK/Utility/Open ScriptableTemplate")]
        private static void CreateOrOpenScriptableTemplate()
        {
            Selection.activeObject = Instance;
        }

#endif

        public static ScriptableTemplate Instance
        {
            get
            {
#if UNITY_EDITOR
                var path = AssetDatabase.AssetPathToGUID(SAVE_PATH);
                if (string.IsNullOrEmpty(path))
                {
                    instance = ScriptableObject.CreateInstance<ScriptableTemplate>();
                    AssetDatabase.CreateAsset(instance, SAVE_PATH);
                }
#endif
                return instance;
            }
        }


    }

}
