using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Controller.Simple
{
    /// <summary>
    /// A simple camera controller with the ability to follow a specific GameObject.
    /// </summary>
    public class SimpleFollowCameraController : MonoBehaviour
    {
        [SerializeField]
        GameObject target;

        [SerializeField]
        Vector3 offset;

        [SerializeField]
        [Range(0, 5)]
        float smoothTime;

        [SerializeField]
        bool isLateUpdate = true;

        [SerializeField]
        bool enableLookAt;

        [SerializeField]
        bool ignoreTargetAxisX = false;

        [SerializeField]
        bool ignoreTargetAxisY = false;

        [SerializeField]
        bool ignoreTargetAxisZ = false;

        [SerializeField]
        Vector3[] offsetArray;

        [SerializeField]
        uint offsetIndex;

        [SerializeField]
        float offsetSmoothTime;

        Vector3 initialPos = Vector3.zero;
        Vector3 velocity = Vector3.zero;

        bool isOffsetSmoothing = false;
        float tempOffsetSmoothTime = 0.0f;
        Vector3 tempOffset = Vector3.zero;

        #region Property

        public GameObject Target { get => target; private set => target = value; }
        public Vector3 Offset { get => offset; private set => offset = value; }
        public Vector3 Velocity { get => velocity; private set => velocity = value; }
        public float SmoothTime { get => smoothTime; protected set => smoothTime = value; }
        public bool IsLateUpdate { get => isLateUpdate; protected set => isLateUpdate = value; }
        public bool EnableLookAt { get => enableLookAt; protected set => enableLookAt = value; }
        public bool IgnoreTargetAxisX { get => ignoreTargetAxisX; protected set => ignoreTargetAxisX = value; }
        public bool IgnoreTargetAxisY { get => ignoreTargetAxisY; protected set => ignoreTargetAxisY = value; }
        public bool IgnoreTargetAxisZ { get => ignoreTargetAxisZ; protected set => ignoreTargetAxisZ = value; }

        #endregion

        public virtual void ChangeTarget(GameObject t)
        {
            target = t;
            initialPos = t.transform.position + offset;
        }

        public virtual void ChangeOffsetSmoothlyUsingArray(bool next, Action oncomplete = null)
        {
            if (offsetArray.Length == 0 || isOffsetSmoothing) return;

            if (next)
            {
                ++offsetIndex;
                if (offsetIndex > offsetArray.Length - 1) offsetIndex = 0;
            }
            else
            {
                --offsetIndex;
                if (offsetIndex < 0) offsetIndex = (uint)offsetArray.Length - 1;
            }

            tempOffset = offsetArray[offsetIndex];
            isOffsetSmoothing = true;
            tempOffsetSmoothTime = 1.0f / (offsetSmoothTime * 10.0f); //Vector3.Lerp takes the range of 0-1 seconds, adjust the value.

            StartCoroutine(UpdateOffsetEndCo(oncomplete));
        }

        #region Editor

#if UNITY_EDITOR

        public void Editor_AcceptOffset()
        {
            if (target == null)
            {
                Debug.LogError("Need to set the target.");
                return;
            }

            //Because want to apply the offset in one frame, ir temporarily set smmothTime to 0.
            var st = smoothTime;
            smoothTime = 0.0f;

            ChangeTarget(target);
            Move();

            //Back smoothTime setting.
            smoothTime = st;
        }

#endif

        #endregion

        #region Internal

        protected virtual void Start()
        {
            if (target != null)
            {
                initialPos = target.transform.position + offset;
            }
        }

        protected virtual void Update()
        {
            if (!IsLateUpdate)
            {
                Move();

                if (isOffsetSmoothing)
                {
                    UpdateOffset();
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (IsLateUpdate)
            {
                Move();

                if (isOffsetSmoothing)
                {
                    UpdateOffset();
                }
            }
        }

        void Move()
        {
            if (target == null) return;

            var newpos = target.transform.position + offset;

            if (IgnoreTargetAxisX) newpos.x = initialPos.x;
            if (IgnoreTargetAxisY) newpos.y = initialPos.y;
            if (IgnoreTargetAxisZ) newpos.z = initialPos.z;

            transform.position = Vector3.SmoothDamp(transform.position, newpos, ref velocity, smoothTime);

            if (EnableLookAt)
            {
                transform.LookAt(target.transform);
            }
        }

        void UpdateOffset()
        {
            offset = Vector3.Lerp(offset, tempOffset, tempOffsetSmoothTime);
        }

        IEnumerator UpdateOffsetEndCo(Action oncomplete)
        {
            yield return new WaitForSeconds(offsetSmoothTime);

            isOffsetSmoothing = false;
            offset = tempOffset;
            tempOffset = Vector3.zero;
            tempOffsetSmoothTime = 0.0f;
            oncomplete?.Invoke();
        }

        #endregion

    }
}

