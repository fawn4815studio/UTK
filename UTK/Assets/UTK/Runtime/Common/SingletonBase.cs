using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Common
{
    public abstract class SingletonBase<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    var ob = new GameObject();
                    instance = ob.AddComponent<T>();

                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(ob);
                    }
                }

                return instance;
            }
        }

        protected SingletonBase() { }
    }
}
