using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Manager
{
    /// <summary>
    /// Manager class for managing audio loading, playback, management, etc.
    /// </summary>
    public class AudioManager : SingletonBase<AudioManager>
    {

        #region Internal

        void Start()
        {
            name = "AudioManager";
        }

        void Update()
        {

        }

        #endregion
    }

}
