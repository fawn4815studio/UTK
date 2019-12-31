using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Manager
{
    /// <summary>
    /// ResourceManager can not load AssetBundle. Use AssetBundleManager to load AssetBundle.
    /// </summary>
    [Obsolete("Use AssetManager instead")]
    public sealed class ResourceManager : SingletonBase<ResourceManager>
    {
        private Dictionary<string, ResourceData> cacheDic = new Dictionary<string, ResourceData>();

        /// <summary>
        /// Load resource specified by path synchronously.
        /// </summary>
        /// <param name="filepath">Resource file path.</param>
        public Type LoadSync<Type>(string filepath)
            where Type : UnityEngine.Object

        {
            if (cacheDic.ContainsKey(filepath))
            {
                var d = cacheDic[filepath];
                d.IncRef();
                return d.GetData<Type>();
            }

            var resource = Resources.Load<Type>(filepath);
            cacheDic[filepath] = new ResourceData(resource);

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
                var d = cacheDic[filepath];
                d.IncRef();
                oncomplete?.Invoke(d.GetData<Type>());
                return;
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
                var d = cacheDic[filepath];

                if (d.DecRef() <= 0)
                {
                    cacheDic.Remove(filepath);
                    Resources.UnloadUnusedAssets();
                }
            }
        }

        /// <summary>
        /// Unload all resource.
        /// </summary>
        public void UnloadAll()
        {
            var removelist = new List<KeyValuePair<string, ResourceData>>();

            foreach (var c in cacheDic)
            {
                if (c.Value.DecRef() <= 0)
                {
                    removelist.Add(c);
                }
            }

            var ra = removelist.ToArray();
            for (int i = 0; i < ra.Length; i++)
            {
                cacheDic.Remove(ra[i].Key);
            }

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

            cacheDic[filepath] = new ResourceData(resReq.asset);
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

