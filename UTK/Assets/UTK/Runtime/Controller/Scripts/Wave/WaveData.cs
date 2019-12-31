using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Controller.Wave
{
    /// <summary>
    /// Wave data used by controller.
    /// Use through <see cref="WaveController"/>.
    /// </summary>
    [Serializable]
    public class WaveData
    {

        [Serializable]
        public class WavePoint
        {
            public enum SpawnType : int
            {
                Immediately,
                Delay
            }

            [SerializeField]
            private string data;

            [SerializeField]
            private Vector3 position;

            [SerializeField]
            private Quaternion rot;

            [SerializeField]
            private bool raycastGround;

            [SerializeField]
            private SpawnType spawn;

            [SerializeField]
            private string spawnEffect;

            [SerializeField]
            private string delayEffect;

            [SerializeField]
            private float delayTime;

            /// <summary>
            /// Whether to count as enemy.(True = Not count)
            /// </summary>
            [SerializeField]
            private bool ignoreCount = false;

            public string Data { get => data; }
            public Vector3 Position { get => position; }
            public Quaternion Rot { get => rot; }
            public bool RaycastGround { get => raycastGround; }
            public SpawnType Spawn { get => spawn; }
            public string SpawnEffect { get => spawnEffect; }
            public string DelayEffect { get => delayEffect; }
            public float DelayTime { get => delayTime; }

            public GameObject DelayEffectObject { get; set; } = null;
            public GameObject SpawnEffectObject { get; set; } = null;
            public GameObject DataObject { get; set; } = null;
            public bool IgnoreCount { get => ignoreCount; }
        }

        /// <summary>
        /// Wave name.
        /// </summary>
        [SerializeField]
        private string name;

        /// <summary>
        /// Spawn points.
        /// </summary>
        [SerializeField]
        private WavePoint[] points;

        /// <summary>
        /// Wave priority.
        /// </summary>
        [SerializeField]
        private int priority;

        /// <summary>
        /// Is apply time limit.
        /// </summary>
        [SerializeField]
        private bool isTimeLimit;

        /// <summary>
        /// Time limit.
        /// </summary>
        [SerializeField]
        private float limitTime;

        #region Property

        public string Name { get => name; }

        public WavePoint[] Points { get => points; }

        public int Priority { get => priority; }

        public bool IsTimeLimit { get => isTimeLimit; }

        public float LimitTime { get => limitTime; }

        public float ElapsedTime { get; set; } = 0;

        #endregion
    }
}
