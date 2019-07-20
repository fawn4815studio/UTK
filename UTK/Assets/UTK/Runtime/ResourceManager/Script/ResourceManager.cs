using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTK.Runtime.Common;

namespace UTK.Runtime.Manager
{
    /// <summary>
    /// ResourceManager can not load AssetBundle. Use AssetBundleManager to load AssetBundle.
    /// </summary>
    public class ResourceManager : SingletonBase<ResourceManager>
    {
        private Dictionary<string, UnityEngine.Object> cacheDic = new Dictionary<string, UnityEngine.Object>();

        /// <summary>
        /// Load resource specified by path synchronously.
        /// </summary>
        /// <param name="filepath">Resource file path.</param>
        public Type LoadSync<Type>(string filepath)
            where Type : UnityEngine.Object

        {
            if (cacheDic.ContainsKey(filepath))
            {
                return cacheDic[filepath] as Type;
            }

            var resource = Resources.Load<Type>(filepath);
            cacheDic[filepath] = resource;

            return resource;
        }

        /// <summary>
        /// Loads the resource specified by path asynchronously.
        /// </summary>
        /// <param name="filepath">Resource file path.</param>
        /// <param name="oncomplete">Called when resource loaded.</param>
        public void LoadAsync<Type>(string filepath, UnityEngine.Events.UnityAction<Type> oncomplete)
            where Type : UnityEngine.Object

        {
            if (cacheDic.ContainsKey(filepath))
            {
                oncomplete?.Invoke(cacheDic[filepath] as Type);
            }

            StartCoroutine(LoadAsyncCo(filepath, oncomplete));
        }

        /// <summary>
        /// Unload resource if loaded.
        /// </summary>
        /// <param name="filepath">Unload resource file path.</param>
        public void Unload(string filepath)
        {
            if (cacheDic.ContainsKey(filepath))
            {
                Resources.UnloadAsset(cacheDic[filepath]);
                cacheDic.Remove(filepath);
            }
        }

        /// <summary>
        /// Unload all resource.
        /// </summary>
        public void UnloadAll()
        {
            cacheDic.Clear();
            Resources.UnloadUnusedAssets();
        }

        #region Internal

        IEnumerator LoadAsyncCo<Type>(string filepath, UnityEngine.Events.UnityAction<Type> oncomplete)
              where Type : UnityEngine.Object
        {
            ResourceRequest resReq = Resources.LoadAsync(filepath);

            while (resReq.isDone == false)
            {
                yield return null;
            }

            cacheDic[filepath] = resReq.asset;
            oncomplete?.Invoke(resReq.asset as Type);
        }

        void Start()
        {
            name = "ResourceManager";
        }

        void Update()
        {

        }

        #endregion

    }
}

