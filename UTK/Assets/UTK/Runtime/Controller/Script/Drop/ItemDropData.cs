using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Controller.Drop
{
    /// <summary>
    /// Item drop data.
    /// Use through <see cref="ItemDropController"/>.
    /// </summary>
    [Serializable]
    public class ItemDropData
    {
        /// <summary>
        /// Must be a prefab path located Resources folder or must be single asset bundle name.
        /// </summary>
        [SerializeField]
        private string dataPath;

        /// <summary>
        /// A unique id stored in metadata.
        /// </summary>
        [SerializeField]
        private string dataId;

        /// <summary>
        /// Reference value to place
        /// </summary>
        [SerializeField]
        private int evaluationValue;

        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private Quaternion rotation;

        [SerializeField]
        private Vector3 scale;

        #region Property

        public GameObject dataObject { get; set; } = null; //Runtime only.
        public bool IsDrop { get; set; } = false;          //Runtime only.

        public string DataPath { get => dataPath; }
        public string DataId { get => dataId; }
        public int EvaluationValue { get => evaluationValue; }
        public Vector3 Position { get => position; }
        public Quaternion Rotation { get => rotation; }
        public Vector3 Scale { get => scale; }

        #endregion

    }
}

