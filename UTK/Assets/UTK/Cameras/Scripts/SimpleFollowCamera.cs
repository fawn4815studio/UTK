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

        Vector3 initialPos;
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
            initialPos = t.transform.position + offset;
        }

        #region Internal

        void Start()
        {
            if(target!=null)
            {
                initialPos = target.transform.position + offset;
            }
        }

        void LateUpdate()
        {
            if (target == null) return;

            var newpos = target.transform.position + offset;

            if (IgnoreTargetAxisX) newpos.x = initialPos.x;
            if (IgnoreTargetAxisY) newpos.y = initialPos.y;
            if (IgnoreTargetAxisZ) newpos.z = initialPos.z;

            transform.position = Vector3.SmoothDamp(transform.position,newpos,ref velocity, smoothTime);

            if(EnableLookAt)
            {
                transform.LookAt(target.transform);
            }
        }

        #endregion

    }
}

