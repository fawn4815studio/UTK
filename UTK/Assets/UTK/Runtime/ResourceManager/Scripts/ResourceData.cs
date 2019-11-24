using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime
{
    /// <summary>
    /// Resource management data.
    /// Use through <see cref="Manager.ResourceManager"/>.
    /// </summary>
    public class ResourceData
    {
        #region Property

        /// <summary>
        /// Loaded data.
        /// </summary>
        private Object Data { get; set; } = null;

        /// <summary>
        /// Resource reference count.
        /// </summary>
        public int RefCount { get; private set; } = 0;

        #endregion

        public ResourceData(Object d)
        {
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
