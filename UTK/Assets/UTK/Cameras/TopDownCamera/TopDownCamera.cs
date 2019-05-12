using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Cameras
{
    public class TopDownCamera : MonoBehaviour
    {
        [SerializeField]
        GameObject target;

        [SerializeField]
        Vector3 offset;

        [SerializeField]
        [Range(0,5)]
        float smoothTime;

        [SerializeField]
        bool enableLookAt;

        Vector3 velocity = Vector3.zero;

        #region Property
        public GameObject Target { get => target; private set => target = value; }
        public Vector3 Offset { get => offset; set => offset = value; }
        public Vector3 Velocity { get => velocity;}
        public bool EnableLookAt { get => enableLookAt; set => enableLookAt = value; }
        #endregion

        public void ChangeTarget(GameObject t)
        {
            target = t;
        }

        void LateUpdate()
        {
            if (target == null) return;

            var newpos = target.transform.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position,newpos,ref velocity, smoothTime);

            if(EnableLookAt)
            {
                transform.LookAt(target.transform);
            }
        }

    }
}

