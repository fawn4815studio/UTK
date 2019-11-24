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
        private Dictionary<string, AudioSource> sePointDic = new Dictionary<string, AudioSource>();

        /// <summary>
        /// Load audio data specified by path synchronously.
        /// </summary>
        /// <param name="name">Register audio data name.</param>
        /// <param name="assetname"><see cref="AudioClip"/> name.</param>
        /// <param name="loop">Is loop.</param>
        /// <param name="volume">Audio volume.</param>
        public void LoadSync(string name, string assetname, bool loop = false, float volume = 1.0f)
        {
            if (!dataDic.ContainsKey(name))
            {
                dataDic[name] = new AudioData(name, assetname, AssetManager.Instance.LoadSync<AudioClip>(assetname))
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
        /// <param name="assetname"><see cref="AudioClip"/> name.</param>
        /// <param name="loop">Is loop.</param>
        /// <param name="volume">Audio volume.</param>
        public void LoadAsync(string name, string assetname, bool loop = false, float volume = 1.0f)
        {
            if (!dataDic.ContainsKey(name))
            {
                AssetManager.Instance.LoadAsync(assetname, (AudioClip clip) =>
                {
                    dataDic[name] = new AudioData(name, assetname, clip)
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
                var removecllippath = dataDic[name].AssetName;
                dataDic[name].Clear();
                dataDic.Remove(name);
                AssetManager.Instance.Unload(removecllippath);
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

        /// <summary>
        /// Create new se point.
        /// </summary>
        /// <param name="name">Point name.</param>
        /// <param name="pos">Point pos.</param>
        /// <param name="secount">Number of AudioSources to create.</param>
        public void CreateSePoint(string name, Vector3 pos)
        {
            if (sePointDic.ContainsKey(name))
            {
                Debug.LogWarning(string.Format("{0} se point already exists.", name));
                return;
            }

            var ob = new GameObject(name);
            ob.transform.position = pos;
            ob.transform.parent = transform;
            sePointDic[name] = ob.AddComponent<AudioSource>();
        }

        /// <summary>
        /// Delete se point.
        /// </summary>
        /// <param name="name">Delete se point name.</param>
        public void DeleteSePoint(string name)
        {
            if (!sePointDic.ContainsKey(name))
            {
                Debug.LogWarning(string.Format("{0} se point not exists.", name));
                return;
            }

            Destroy(sePointDic[name].gameObject);
            sePointDic[name] = null;
            sePointDic.Remove(name);
        }

        /// <summary>
        /// Play se.
        /// </summary>
        /// <param name="name">Se name.</param>
        /// <param name="point">Se point name.</param>
        public void PlaySe(string name, string point)
        {
            if (!sePointDic.ContainsKey(point))
            {
                Debug.LogWarning(string.Format("{0} se point not exists.", name));
                return;
            }

            if (!dataDic.ContainsKey(name))
            {
                Debug.LogWarning(string.Format("Failed play bgm. {0} not loaded.", name));
                return;
            }

            var data = dataDic[name];
            var source = sePointDic[point];

            source.PlayOneShot(data.Clip, data.VolumeScale);
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
