using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Controller.AI
{
    /// <summary>
    /// Path data used by Agent.
    /// Use through <see cref="PathAgentController"/>.
    /// </summary>
    public class PathData
    {
        /// <summary>
        /// 
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// 
        /// </summary>
        private GameObject target = null;

        #region Property

        public Vector3 Position { get => position; }

        public GameObject Target { get => target; }

        #endregion


    }

}
