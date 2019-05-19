using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Cameras
{
    public class SimpleFollowCamera : MonoBehaviour
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

        [SerializeField]
        bool ignoreAxisX = false;

        [SerializeField]
        bool ignoreAxisY = false;

        [SerializeField]
        bool ignoreAxisZ = false;

        Vector3 velocity = Vector3.zero;

        #region Property
        public GameObject Target { get => target; private set => target = value; }
        public Vector3 Offset { get => offset; set => offset = value; }
        public Vector3 Velocity { get => velocity;}
        public bool EnableLookAt { get => enableLookAt; set => enableLookAt = value; }
        public bool IgnoreAxisX { get => ignoreAxisX; set => ignoreAxisX = value; }
        public bool IgnoreAxisY { get => ignoreAxisY; set => ignoreAxisY = value; }
        public bool IgnoreAxisZ { get => ignoreAxisZ; set => ignoreAxisZ = value; }
        #endregion

        public void ChangeTarget(GameObject t)
        {
            target = t;
        }

        void LateUpdate()
        {
            if (target == null) return;

            var newpos = target.transform.position;

            if (!IgnoreAxisX) newpos.x += offset.x;
            if (!IgnoreAxisY) newpos.y += offset.y;
            if (!IgnoreAxisZ) newpos.z += offset.z;

            transform.position = Vector3.SmoothDamp(transform.position,newpos,ref velocity, smoothTime);

            if(EnableLookAt)
            {
                transform.LookAt(target.transform);
            }
        }

    }
}

