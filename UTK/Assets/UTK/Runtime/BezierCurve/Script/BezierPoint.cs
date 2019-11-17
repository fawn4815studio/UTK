using System;
using UnityEngine;

namespace UTK.Runtime.BezierCurve
{
    [Serializable]
    public class BezierPoint
    {
        [SerializeField]
        private Vector3 anchore;

        [SerializeField]
        private Vector3 handle1;

        [SerializeField]
        private Vector3 handle2;

        #region Property

        public Vector3 Anchore { get => anchore; set => anchore = value; }
        public Vector3 Handle1 { get => handle1; set => handle1 = value; }
        public Vector3 Handle2 { get => handle2; set => handle2 = value; }
        
        #endregion
    }

}
