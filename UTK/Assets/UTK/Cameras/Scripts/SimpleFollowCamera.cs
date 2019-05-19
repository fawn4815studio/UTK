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
        bool ignoreTargetAxisX = false;

        [SerializeField]
        bool ignoreTargetAxisY = false;

        [SerializeField]
        bool ignoreTargetAxisZ = false;

        Vector3 initialTargetPos;
        Vector3 velocity = Vector3.zero;

        #region Property
        public GameObject Target { get => target; private set => target = value; }
        public Vector3 Offset { get => offset; set => offset = value; }
        public Vector3 Velocity { get => velocity;}
        public bool EnableLookAt { get => enableLookAt; set => enableLookAt = value; }
        public bool IgnoreTargetAxisX { get => ignoreTargetAxisX; set => ignoreTargetAxisX = value; }
        public bool IgnoreTargetAxisY { get => ignoreTargetAxisY; set => ignoreTargetAxisY = value; }
        public bool IgnoreTargetAxisZ { get => ignoreTargetAxisZ; set => ignoreTargetAxisZ = value; }
        #endregion

        public void ChangeTarget(GameObject t)
        {
            target = t;
            initialTargetPos = t.transform.position;
        }

        #region Internal

        void Start()
        {
            if(target!=null)
            {
                initialTargetPos = target.transform.position;
            }
        }

        void LateUpdate()
        {
            if (target == null) return;

            var newpos = target.transform.position + offset;

            if (IgnoreTargetAxisX) newpos.x = initialTargetPos.x;
            if (IgnoreTargetAxisY) newpos.y = initialTargetPos.y;
            if (IgnoreTargetAxisZ) newpos.z = initialTargetPos.z;

            transform.position = Vector3.SmoothDamp(transform.position,newpos,ref velocity, smoothTime);

            if(EnableLookAt)
            {
                transform.LookAt(target.transform);
            }
        }

        #endregion

    }
}

