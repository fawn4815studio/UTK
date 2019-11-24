using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Utility
{
    public class Billboard : MonoBehaviour
    {
        public enum IgnoreAxis
        {
            None,
            X,
            Y,
            Z
        };

        [SerializeField]
        private IgnoreAxis ignoreAxis = IgnoreAxis.None;

        [SerializeField]
        private bool autoAttachMainCamera = true;

        private Camera targetCamera = null;

        /// <summary>
        /// Set billboard target camera.
        /// </summary>
        /// <param name="camera"><see cref="Camera"/></param>
        public void SetCamera(Camera camera)
        {
            targetCamera = camera;
        }

        #region Internal

        void Start()
        {
            if (autoAttachMainCamera)
            {
                targetCamera = Camera.main;
            }
        }

        void LateUpdate()
        {
            if (targetCamera == null) return;

            var cf = targetCamera.transform.rotation * Vector3.forward;
            var reverse = cf * -1f;

            switch (ignoreAxis)
            {
                case IgnoreAxis.X: reverse.x = 0.0f; break;
                case IgnoreAxis.Y: reverse.y = 0.0f; break;
                case IgnoreAxis.Z: reverse.z = 0.0f; break;
            }

            transform.localRotation = Quaternion.FromToRotation(Vector3.back, reverse);
        }

        #endregion
    }

}
