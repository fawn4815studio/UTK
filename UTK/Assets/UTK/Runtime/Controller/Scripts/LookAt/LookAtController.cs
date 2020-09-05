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

        float elapsedTime;

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

        #endregion

        #region Internal

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (head == null || target == null) return;

            var direction = Vector3.RotateTowards(head.forward, HeadToTarget, 1, 0);
            var lookrot = Quaternion.LookRotation(HeadToTarget);
            var framerot = Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(head.parent.rotation) * lookrot, LerpTime);
            var limitrot = Quaternion.RotateTowards(Quaternion.identity, framerot, headLimit);

            head.localRotation = limitrot;

            if (useBodyRotate)
            {
                if (Quaternion.Angle(framerot, limitrot) > 0)
                {
                    //Debug.Log(string.Format("Qa : {0}", Quaternion.Angle(framerot, limitrot)));

                    var basehead = Quaternion.Inverse(head.localRotation) * head.forward;
                    var futurehead = framerot * basehead;
                    basehead.y = futurehead.y = 0;
                    var angle = Vector3.SignedAngle(basehead, futurehead, Vector3.up);

                    /*
                    Debug.DrawRay(head.position, basehead * 10, Color.blue);
                    Debug.DrawRay(head.position, futurehead * 10, Color.red);
                    Debug.Log(string.Format("Angle : {0}", angle));
                    */

                    if (Mathf.Abs(angle) > headLimit)
                    {
                        var hf = head.forward;
                        hf.y = 0;
                        var frameangle = Vector3.SignedAngle(hf, futurehead, Vector3.up);
                        //Debug.Log(string.Format("FrameAngle : {0}", frameangle));
                        body.localRotation = body.localRotation * Quaternion.AngleAxis(frameangle, Vector3.up);
                    }
                }
            }

            elapsedTime += Time.deltaTime;
        }

        #endregion
    }

}
