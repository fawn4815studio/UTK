using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.BezierCurve
{
    public class BezierCurveController : MonoBehaviour
    {
        [SerializeField]
        private List<BezierPoint> points = null;

        [SerializeField]
        private bool loop = false;

        [SerializeField]
        private bool pause = false;

        [SerializeField]
        private float time = 0.0f;

        [SerializeField]
        private int segment = 0;

        [SerializeField]
        private bool autoLookDirection = false;


        private class RuntimePoint
        {
            public Vector3 Point;
            public float Time;
            public float Speed;
            public float Distance;
            public float TotalTime;
            public float TotalDistance;
            public Quaternion Rot;
        }

        private List<RuntimePoint> runtimePoints = new List<RuntimePoint>();
        private float elapsedTime = 0.0f;
        private bool end = false;

        #region Property

        public bool Pause { get => pause; set => pause = value; }

        #endregion

        public void ResetElapsedTime()
        {
            elapsedTime = 0.0f;
            end = false;
        }

        #region Internal

        // Start is called before the first frame update
        void Start()
        {
            InitRunitmePoints();
        }

        // Update is called once per frame
        void Update()
        {
            if (Pause || end) return;

            RuntimePoint previous = null, current = null;
            float min, max;
            Vector3 minpos, maxpos;
            Quaternion minrot, maxrot;

            foreach (var p in runtimePoints)
            {
                if (p.TotalTime >= elapsedTime)
                {
                    current = p;
                    break;
                }

                previous = p;
            }

            if (current == null) //Pass end detection
            {
                if (loop)
                {
                    elapsedTime = 0.0f;
                }
                else
                {
                    end = true;
                }

                return;
            }

            //Set minimum and maximum range.
            if (previous == null)
            {
                min = 0;
                max = current.TotalTime;

                minpos = Vector3.zero;
                maxpos = current.Point;

                minrot = transform.localRotation;
                maxrot = current.Rot;
            }
            else
            {
                min = previous.TotalTime;
                max = current.TotalTime;

                minpos = previous.Point;
                maxpos = current.Point;

                minrot = previous.Rot;
                maxrot = current.Rot;
            }

            //Map to 0-1 range.
            var div = (Mathf.Clamp(elapsedTime, min, max) - min) / (max - min);
            var newpos = Vector3.Lerp(minpos, maxpos, div);
            var newrot = Quaternion.Slerp(minrot, maxrot, div);

            if (autoLookDirection && !float.IsNaN(newrot.x))
            {
                transform.localRotation = newrot;
            }

            if (!float.IsNaN(newpos.x))
            {
                transform.localPosition = newpos;
            }

            elapsedTime += Time.deltaTime;
        }

        void InitRunitmePoints()
        {
            var vecpoints = BezierCurveCalculator.GetPoints(points, segment);
            float totaldistance = 0;
            float totaltime = 0;

            var size = vecpoints.Count;
            for (int i = 0; i < size; i++)
            {
                var rp = new RuntimePoint();
                rp.Point = vecpoints[i];
                rp.Distance = i == 0 ? 0 : Vector3.Distance(vecpoints[i - 1], vecpoints[i]);
                totaldistance += rp.Distance;
                rp.TotalDistance = totaldistance;

                if (i == 0)
                {
                    rp.Point = transform.localPosition;
                    rp.Rot = transform.localRotation;
                }
                else
                {
                    var diff = vecpoints[i] - vecpoints[i - 1];
                    rp.Rot = diff.Equals(Vector3.zero) ? runtimePoints[i - 1].Rot : Quaternion.LookRotation(diff);
                }

                runtimePoints.Add(rp);
            }

            var averagespeed = totaldistance / time;

            foreach (var p in runtimePoints)
            {
                p.Time = p.Distance / averagespeed;
                p.Speed = p.Distance / p.Time;

                totaltime += p.Time;
                p.TotalTime = totaltime;
            }

        }

        #endregion
    }
}

