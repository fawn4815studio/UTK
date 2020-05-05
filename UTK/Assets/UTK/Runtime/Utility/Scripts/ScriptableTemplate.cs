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
        /// <summary>
        /// Change to a suitable storage location.
        /// </summary>
        private static readonly string SAVE_PATH = "Assets/UTK/Config/Resources/GameData.asset";
        private static T instance;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                var path = AssetDatabase.AssetPathToGUID(SAVE_PATH);
                if (string.IsNullOrEmpty(path))
                {
                    instance = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(instance, SAVE_PATH);
                }
#endif

                if (instance == null)
                {
                    instance = AssetManager.Instance.LoadSync<T>(SAVE_PATH);
                }

                return instance;
            }
        }
    }
}
