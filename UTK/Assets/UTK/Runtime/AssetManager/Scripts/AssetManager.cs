using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTK.Runtime.Manager
{
    /// <summary>
    /// Class that manages loading and unloading of assets(resource).
    /// Processes by absorbing differences such as asset bundles and resources.
    /// </summary>
    public sealed class AssetManager : SingletonBase<AssetManager>
    {
        private Dictionary<string, AssetData> cacheDic = new Dictionary<string, AssetData>();

        private readonly string assetBundlePath = Application.streamingAssetsPath;

        /// <summary>
        /// Load asset specified by path synchronously.
        /// </summary>
        /// <param name="assetname">Asset name.(No extension)</param>
        public Type LoadSync<Type>(string assetname)
            where Type : UnityEngine.Object

        {
            if (cacheDic.ContainsKey(assetname))
            {
                var d = cacheDic[assetname];
                d.IncRef();
                return d.GetData<Type>();
            }

            if (IsBundleExists(assetname))
            {
                //TODO : Load asset bundle
            }
            else
            {
                return LoadResourceSync<Type>(assetname);
            }

            return null;
        }

        /// <summary>
        /// Load asset specified by name asynchronously.
        /// </summary>
        /// <param name="assetname">Asset name.(No extension)</param>
        /// <param name="oncomplete">Called when resource loaded.</param>
        public void LoadAsync<Type>(string assetname, UnityEngine.Events.UnityAction<Type> oncomplete)
            where Type : UnityEngine.Object

        {
            if (cacheDic.ContainsKey(assetname))
            {
                var d = cacheDic[assetname];
                d.IncRef();
                oncomplete?.Invoke(d.GetData<Type>());
                return;
            }

            if (IsBundleExists(assetname))
            {
                //TODO : Load asset bundle
            }
            else
            {
                StartCoroutine(LoadResourceAsyncCo<Type>(assetname, oncomplete));
            }
        }

        /// <summary>
        /// Unload asset if loaded.
        /// </summary>
        /// <param name="assetname">Unload asset name.</param>
        public void Unload(string assetname)
        {
            if (cacheDic.ContainsKey(assetname))
            {
                var d = cacheDic[assetname];

                if (d.DecRef() <= 0)
                {
                    switch (d.Type)
                    {
                        case AssetData.AssetType.Resources:
                            UnloadResource(d);
                            break;

                        case AssetData.AssetType.AssetBundle:
                            //TODO : Unload asset bundle.
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Unload all.
        /// </summary>
        public void UnloadAll()
        {
            var removes = cacheDic.Values.ToArray();
            foreach (var d in removes)
            {
                switch (d.Type)
                {
                    case AssetData.AssetType.Resources:
                        UnloadResource(d);
                        break;

                    case AssetData.AssetType.AssetBundle:
                        //TODO : Unload asset bundle.
                        break;
                }
            }

            cacheDic.Clear();
        }

        #region Internal

        void Start()
        {
            name = "AssetManager";
        }

        void Update()
        {

        }

        bool IsBundleExists(string assetname)
        {
            return System.IO.File.Exists(System.IO.Path.Combine(assetBundlePath, assetname) + ".ab");
        }

        #region Resources

        Type LoadResourceSync<Type>(string assetname)
           where Type : UnityEngine.Object
        {
            var resource = Resources.Load<Type>(GetResourcePath(assetname));

            if (resource != null)
            {
                var data = cacheDic[assetname] = new AssetData(assetname, resource, AssetData.AssetType.Resources);
                return resource;
            }

            return null;
        }

        IEnumerator LoadResourceAsyncCo<Type>(string assetname, UnityEngine.Events.UnityAction<Type> oncomplete)
              where Type : UnityEngine.Object
        {
            ResourceRequest resReq = Resources.LoadAsync(GetResourcePath(assetname));

            while (resReq.isDone == false)
            {
                yield return null;
            }

            cacheDic[assetname] = new AssetData(assetname, resReq.asset, AssetData.AssetType.Resources);
            oncomplete?.Invoke(resReq.asset as Type);
        }

        string GetResourcePath(string assetname)
        {
            var resourcepath = string.Empty;

#if UNITY_EDITOR

            var pathes = AssetDatabase.GetAssetPathsFromAssetBundle(assetname.ToLower() + ".ab");

            if (pathes.Length == 0)
            {
                Debug.LogError(string.Format("{0} is not exists.", assetname));
                return null;
            }
            else if (pathes.Length > 2)
            {
                Debug.LogWarning(string.Format("{0} Duplicate asset bundle name.", assetname));
            }

            var splitpathes = pathes[0].Split('/');
            var index = Array.IndexOf(splitpathes, "Resources") + 1; //+1 = Resources not include.


            for (int i = index; i < splitpathes.Length; i++)
            {
                resourcepath = System.IO.Path.Combine(resourcepath, System.IO.Path.GetFileNameWithoutExtension(splitpathes[i]));
            }
#endif
            return resourcepath;
        }

        void UnloadResource(AssetData data)
        {
            cacheDic.Remove(data.Name);
            Resources.UnloadUnusedAssets();
        }

        #endregion


        #region AssetBundle


        #endregion

        #endregion
    }

}
