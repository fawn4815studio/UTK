using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime
{
    /// <summary>
    /// Ausio management data.
    /// Use through <see cref="Manager.AudioManager"/>.
    /// </summary>
    public class AudioData
    {
        #region Property

        public string AssetName { get; private set; }

        public string Name { get; private set; }

        public AudioSource Owner { get; set; }

        public AudioClip Clip { get; private set; }

        public bool IsLoop { get; set; } = false;

        public float Volume { get; set; } = 1.0f;

        public float VolumeScale { get; set; } = 1.0f;

        public bool Mute { get; set; } = false;

        public bool IsPlaying
        {
            get
            {
                if (Owner == null) return false;
                return Owner.isPlaying;
            }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Audio data name.</param>
        /// <param name=""><see cref="AudioClip"/> name.</param>
        /// <param name="clip"><see cref="AudioClip"/> instance.</param>
        public AudioData(string name, string assetname, AudioClip clip)
        {
            AssetName = assetname;
            Name = name;
            Clip = clip;
        }

        public void Clear()
        {
            Clip = null;
        }

    }
}

