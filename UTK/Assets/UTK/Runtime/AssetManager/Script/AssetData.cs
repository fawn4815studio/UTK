using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime
{
    /// <summary>
    /// Asset management data.
    /// Use through <see cref="Manager.AssetManager"/>.
    /// </summary>
    public class AssetData
    {
        public enum AssetType
        {
            None,
            Resources,
            AssetBundle
        }

        #region Property

        public string Name { get; private set; } = null;

        public AssetType Type { get; private set; } = AssetType.None;

        /// <summary>
        /// Loaded data.
        /// </summary>
        private Object Data { get; set; } = null;

        /// <summary>
        /// Asset reference count.
        /// </summary>
        public int RefCount { get; private set; } = 0;

        #endregion

        public AssetData(string name,Object d, AssetType type)
        {
            Name = name;
            Type = type;
            Data = d;
            ++RefCount;
        }

        public Type GetData<Type>()
            where Type : UnityEngine.Object
        {
            return Data as Type;
        }

        public int IncRef()
        {
            return ++RefCount;
        }

        public int DecRef()
        {
            return --RefCount;
        }
    }

}


