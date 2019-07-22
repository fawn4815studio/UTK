using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime
{
    /// <summary>
    /// Singleton template class in Unity.
    /// Create a gameobject and add a class as a component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
