using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Controller.AI
{
    /// <summary>
    /// Path data used by Agent.
    /// Use through <see cref="PathAgentController"/>.
    /// </summary>
    [Serializable]
    public class PathData
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Vector3 position = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float interval = 0.0f;

        #region Property

        public Vector3 Position { get => position; }

        public float Interval { get => interval; }

        #endregion

    }

}
