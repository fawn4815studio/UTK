using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTK.Runtime.Manager;

namespace UTK.Runtime.Controller.Wave
{
    /// <summary>
    /// Control the enemy waves in the game.
    /// </summary>
    public class WaveController : MonoBehaviour
    {
        public enum PriorityOption
        {
            Greater,
            Equal
        };

        [SerializeField]
        private WaveData[] waveDatas;

        private GameObject waveRoot;

        private Dictionary<string, GameObject> preloadObjects = new Dictionary<string, GameObject>();

        private Queue<WaveData> frameRemoveQueue = new Queue<WaveData>();

        private bool pauseWave = false;

        #region Property

        public List<WaveData> CurrentWaves { get; private set; } = new List<WaveData>();
        public int CurrentPriority { get; private set; } = 0;
        public int CurrentWaveDataCount { get; private set; } = 0;

        /// <summary>
        /// Called when wave completed.
        /// </summary>
        public event System.Action OnWaveComplete;

        /// <summary>
        /// Called when data added.
        /// </summary>
        public System.Action<int> OnDataAdd;

        /// <summary>
        /// Called when data is destroyed.
        /// </summary>
        public System.Action OnDataDestory { get; set; }

        /// <summary>
        /// Called when <see cref="CurrentWaveDataCount"/> changed.
        /// </summary>
        public event System.Action<int> OnWaveDataCountChanged;

        /// <summary>
        /// Called when the time limit set in <see cref="WaveData"/> is exceeded.
        /// </summary>
        public event System.Action<WaveData> OnTimeOut;

        /// <summary>
        /// Called when the <see cref="WaveData.ElapsedTime"/> has been updated.
        /// </summary>
        public event System.Action<WaveData> OnElapsedTimeUpdated;

        #endregion

        /// <summary>
        /// Play wave based on priority.
        /// </summary>
        /// <param name="priority">Reference priority.</param>
        /// <param name="matchname">If specified, only waves with matching names will be played.</param>
        public void AdvanceWave(int priority, PriorityOption option, string matchname = null)
        {
            CurrentPriority = priority;

            IEnumerable<WaveData> waves = null;
            switch (option)
            {
                case PriorityOption.Greater:
                    waves = waveDatas.Where(t => t.Priority <= priority);
                    break;

                case PriorityOption.Equal:
                    waves = waveDatas.Where(t => t.Priority == priority);
                    break;
            }


            foreach (var w in waves)
            {
                w.ElapsedTime = 0;

                if (string.IsNullOrEmpty(matchname))
                {
                    InitializeWave(w);
                }
                else
                {
                    if (matchname.Equals(w.Name))
                    {
                        InitializeWave(w);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the number of wave less than or equal to the priority.
        /// </summary>
        /// <param name="priority">Wave priority.</param>
        public int GetWaveCount(int priority)
        {
            return waveDatas.Count(t => t.Priority >= priority);
        }

        /// <summary>
        /// Preload all assets used in wave.
        /// </summary>
        public void PreloadAllWave()
        {
            foreach (var w in waveDatas)
            {
                foreach (var p in w.Points)
                {
                    PreloadObject(p.Data);
                    PreloadObject(p.DelayEffect);
                    PreloadObject(p.SpawnEffect);
                }
            }
        }

        /// <summary>
        /// Reset controller.
        /// </summary>
        public void ResetController(bool include_preload)
        {
            SetPause(false);
            CurrentPriority = 0;
            CurrentWaveDataCount = 0;
            CurrentWaves.Clear();
            CreateWaveRoot(true); //Recreate root = delete all generated wave

            if (include_preload)
            {
                foreach (var o in preloadObjects)
                {
                    AssetManager.Instance.Unload(o.Key);
                }

                preloadObjects.Clear();
            }

        }

        /// <summary>
        /// Set <see cref="pauseWave"/>.
        /// </summary>
        public void SetPause(bool value)
        {
            pauseWave = value;
        }

        #region Internal

        void Start()
        {
            CreateWaveRoot();

            OnWaveComplete += () =>
            {
                CurrentWaves.Clear();
            };

            OnDataDestory += () =>
            {
                --CurrentWaveDataCount;
                OnWaveDataCountChanged?.Invoke(CurrentWaveDataCount);

                if (CurrentWaveDataCount <= 0)
                {
                    OnWaveComplete?.Invoke();
                }
            };
        }

        void Update()
        {
            if (pauseWave) return;

            foreach (var w in CurrentWaves)
            {
                if (w.IsTimeLimit)
                {
                    w.ElapsedTime += Time.deltaTime;

                    if (w.ElapsedTime > w.LimitTime)
                    {
                        OnTimeOut?.Invoke(w);
                        frameRemoveQueue.Enqueue(w);
                    }
                    else
                    {
                        OnElapsedTimeUpdated?.Invoke(w);
                    }
                }
            }

            while (frameRemoveQueue.Count > 0)
            {
                CurrentWaves.Remove(frameRemoveQueue.Dequeue());
            }
        }

        void CreateWaveRoot(bool force = false)
        {
            if (force == false && waveRoot != null)
            {
                return;
            }

            Destroy(waveRoot);
            waveRoot = new GameObject("waveRoot");
            waveRoot.transform.SetParent(this.transform.parent);
            waveRoot.transform.position = Vector3.zero;
            waveRoot.transform.localPosition = Vector3.zero;
        }

        void InitializeWave(WaveData data)
        {
            CurrentWaves.Add(data);

            CreateWaveRoot();

            foreach (var p in data.Points)
            {
                if (p.IgnoreCount == false)
                {
                    ++CurrentWaveDataCount;
                }

                switch (p.Spawn)
                {
                    case WaveData.WavePoint.SpawnType.Immediately:
                        p.SpawnEffectObject = InstantiateObject(p.SpawnEffect, false, p.RaycastGround, false, p.Position, Quaternion.identity, p.RaycastOffset);
                        p.DataObject = InstantiateObject(p.Data, true, p.RaycastGround, !(p.IgnoreCount), p.Position, p.Rot, p.RaycastOffset);
                        break;

                    case WaveData.WavePoint.SpawnType.Delay:
                        StartCoroutine(DelaySpawn(p));
                        break;
                }

            }
        }

        void PreloadObject(string name)
        {
            if (string.IsNullOrEmpty(name) || preloadObjects.ContainsKey(name)) return;

            AssetManager.Instance.LoadAsync<GameObject>(name, (GameObject data) =>
            {
                preloadObjects[name] = data;
            });
        }

        GameObject InstantiateObject(string name, bool applyrot, bool raycastground, bool attachsender, Vector3 pos, Quaternion rot, Vector3 raycastoffset)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            GameObject ob = null;

            if (raycastground)
            {
                RaycastHit hit;
                if (Physics.Raycast(waveRoot.transform.TransformPoint(pos), Vector3.down, out hit, 100.0f))
                {
                    pos = hit.point;
                    pos += raycastoffset;
                }
            }

            ob = preloadObjects.ContainsKey(name) ? Instantiate(preloadObjects[name]) : Instantiate(AssetManager.Instance.LoadSync<GameObject>(name));
            ob.transform.SetParent(waveRoot.transform);
            ob.transform.position = pos;
            ob.transform.localPosition = waveRoot.transform.InverseTransformPoint(ob.transform.position);

            if (applyrot)
            {
                ob.transform.localRotation = rot;
            }

            if (attachsender)
            {
                var sender = ob.AddComponent<WaveEventSender>();
                sender.Initialize(this);
                OnDataAdd?.Invoke(CurrentWaveDataCount);
            }

            return ob;
        }

        IEnumerator DelaySpawn(WaveData.WavePoint p)
        {
            p.DelayEffectObject = InstantiateObject(p.DelayEffect, false, p.RaycastGround, false, p.Position, Quaternion.identity, p.RaycastOffset);

            yield return new WaitForSeconds(p.DelayTime);
            Destroy(p.DelayEffectObject);

            p.SpawnEffectObject = InstantiateObject(p.SpawnEffect, false, p.RaycastGround, false, p.Position, Quaternion.identity, p.RaycastOffset);
            p.DataObject = InstantiateObject(p.Data, true, p.RaycastGround, !(p.IgnoreCount), p.Position, p.Rot, p.RaycastOffset);
        }

        #endregion
    }

}
