using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Controller
{
    public class LookAtController : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        Transform body;

        [SerializeField]
        Transform head;

        [SerializeField]
        float targetTime;

        [SerializeField]
        bool useBodyRotate;

        [SerializeField]
        float headLimit;

        [SerializeField]
        GameObject debugSphere;

        float elapsedTime;
        Quaternion startHead;
        Quaternion startBody;
        Vector3 startHeadVec;

        #region Property

        float LerpTime
        {
            get
            {
                if (elapsedTime == 0) return 0;
                return elapsedTime / targetTime;
            }
        }

        Vector3 HeadToTarget
        {
            get => target.position - head.position;
        }

        float TargetDistance
        {
            get
            {
                var to = target.transform.position;
                var from = head.transform.position;
                return Vector3.Distance(to, from);
            }
        }

        #endregion

        #region Internal

        // Start is called before the first frame update
        void Start()
        {
            startHead = head.transform.localRotation;
            startBody = body.transform.localRotation;
            startHeadVec = head.forward.normalized;

        }

        // Update is called once per frame
        void Update()
        {
            if (head == null || target == null) return;

            Debug.DrawRay(head.transform.position, HeadToTarget.normalized * TargetDistance, Color.red);

            var angle = Vector3.Angle(HeadToTarget, startHeadVec);
            var axis = Vector3.Cross(startHeadVec, HeadToTarget).normalized;
            var angleAxis = Quaternion.AngleAxis(Mathf.Lerp(0, angle, LerpTime), axis);
            var pos = angleAxis * startHeadVec;

            debugSphere.transform.position = head.transform.position + (pos * TargetDistance);

            var framerot = Quaternion.LookRotation(debugSphere.transform.position - head.transform.position);
            var futureangle = Vector3.Angle(startHeadVec, framerot * startHeadVec);

            head.localRotation = framerot;
            elapsedTime += Time.deltaTime;
        }

        #endregion
    }

}
