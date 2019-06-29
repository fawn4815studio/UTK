using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Manager
{
    public class ResourceManager : MonoBehaviour
    {
        #region Singleton

        private static ResourceManager instance;
        private ResourceManager() { }

        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var ob = new GameObject("ResourceManager");
                    instance = ob.AddComponent<ResourceManager>();

                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(ob);
                    }
                }

                return instance;
            }
        }

        #endregion

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        void Start()
        {

        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {

        }
    }
}

