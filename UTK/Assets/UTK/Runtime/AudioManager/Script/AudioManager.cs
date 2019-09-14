using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Manager
{
    /// <summary>
    /// Manager class for managing audio loading, playback, etc.
    /// </summary>
    public class AudioManager : SingletonBase<AudioManager>
    {
        private AudioSource bgmSource;
        private Dictionary<string, AudioData> dataDic = new Dictionary<string, AudioData>();

        /// <summary>
        /// Load audio data specified by path synchronously.
        /// </summary>
        /// <param name="name">Register audio data name.</param>
        /// <param name="filepath"><see cref="AudioClip"/> file path.</param>
        /// <param name="loop">Is loop.</param>
        /// <param name="volume">Audio volume.</param>
        public void LoadSync(string name, string filepath, bool loop = false, float volume = 1.0f)
        {
            if (!dataDic.ContainsKey(name))
            {
                dataDic[name] = new AudioData(name, filepath, ResourceManager.Instance.LoadSync<AudioClip>(filepath))
                {
                    IsLoop = loop,
                    Volume = volume
                };
            }
            else
            {
                Debug.LogWarning(string.Format("{0} already exists.", name));
            }
        }

        /// <summary>
        /// Load audio data specified by path asynchronously.
        /// </summary>
        /// <param name="name">Register audio data name.</param>
        /// <param name="filepath"><see cref="AudioClip"/> file path.</param>
        /// <param name="loop">Is loop.</param>
        /// <param name="volume">Audio volume.</param>
        public void LoadAsync(string name, string filepath, bool loop = false, float volume = 1.0f)
        {
            if (!dataDic.ContainsKey(name))
            {
                ResourceManager.Instance.LoadAsync(filepath, (AudioClip clip) =>
                {
                    dataDic[name] = new AudioData(name, filepath, clip)
                    {
                        IsLoop = loop,
                        Volume = volume
                    };
                });

            }
            else
            {
                Debug.LogWarning(string.Format("{0} already exists.", name));
            }
        }

        /// <summary>
        /// Unload audio data.
        /// </summary>
        /// <param name="name">Delete audio data name.</param>
        public void Unload(string name)
        {
            if (dataDic.ContainsKey(name))
            {
                ResourceManager.Instance.Unload(dataDic[name].Clip.name);
                dataDic.Remove(name);
            }
        }

        /// <summary>
        /// Play bgm.
        /// </summary>
        /// <param name="name">Bgm name.</param>
        /// <param name="reset">True = Reset audio time.</param>
        public void PlayBgm(string name, bool reset = false)
        {
            if (!dataDic.ContainsKey(name))
            {
                Debug.LogWarning(string.Format("Failed play bgm. {0} not loaded.", name));
                return;
            }

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
            }

            var data = dataDic[name];
            bgmSource.clip = data.Clip;
            bgmSource.loop = data.IsLoop;
            bgmSource.volume = data.Volume;
            bgmSource.mute = data.Mute;
            if (reset) bgmSource.time = 0.0f;
            bgmSource.Play();
        }

        /// <summary>
        /// Stop bgm.
        /// </summary>
        public void StopBgm()
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
        }

        #region Internal

        void Start()
        {
            name = "AudioManager";

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
            }
        }

        void Update()
        {

        }

        #endregion
    }

}
