using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Controller
{
    public class SimpleAutoMoveAndRotateController : MonoBehaviour
    {
        [Serializable]
        public class ControllerData
        {
            public Vector3 value = Vector3.zero;
            public Space space = Space.Self;
        }

        [SerializeField]
        private ControllerData moveData;   //Update units per second.
        [SerializeField]
        private ControllerData rotateData; //Update degrees per second.

        private bool ignoreTimescale;
        private float lastRealTime;

        private Vector3 initialPos;
        private Quaternion initialRot;

        public void ResetPositionAndRotation()
        {
            transform.position = initialPos;
            transform.rotation = initialRot;
        }

        #region Internal

        // Start is called before the first frame update
        void Start()
        {
            initialPos = transform.position;
            initialRot = transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            var deltaTime = Time.deltaTime;

            if (ignoreTimescale)
            {
                deltaTime = Time.realtimeSinceStartup - lastRealTime;
                lastRealTime = Time.realtimeSinceStartup;
            }

            transform.Translate(moveData.value * deltaTime, moveData.space);
            transform.Rotate(rotateData.value * deltaTime, rotateData.space);
        }

        #endregion
    }

}
