using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.BezierCurve
{
    public class BezierCurveController : MonoBehaviour
    {
        [SerializeField]
        private List<BezierPoint> points;

        [SerializeField]
        private bool loop;

        [SerializeField]
        private float time;

        [SerializeField]
        private int segment;

        private class RuntimePoint
        {
            public Vector3 Point;
            public float Time;
            public float Speed;
            public float Distance;
            public float TotalTime;
            public float TotalDistance;

        }

        private List<RuntimePoint> runtimePoints = new List<RuntimePoint>();
        private float elapsedTime = 0.0f;

        private bool end = false;

        #region Internal

        // Start is called before the first frame update
        void Start()
        {
            InitRunitmePoints();
        }

        // Update is called once per frame
        void Update()
        {
            if (end) return;

            RuntimePoint previous = null, current = null;
            float min, max;
            Vector3 minpos, maxpos;

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
            }
            else
            {
                min = previous.TotalTime;
                max = current.TotalTime;

                minpos = previous.Point;
                maxpos = current.Point;
            }

            //Map to 0-1 range.
            var div = (Mathf.Clamp(elapsedTime, min, max) - min) / (max - min);
            var newpos = Vector3.Slerp(minpos, maxpos, div);

            if (!float.IsNaN(newpos.x))
            {
                transform.position = newpos;
            }

            elapsedTime += Time.deltaTime;
            //Debug.Log(elapsedTime);
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
                rp.Point = transform.TransformPoint(vecpoints[i]);
                rp.Distance = i == 0 ? 0 : Vector3.Distance(vecpoints[i - 1], vecpoints[i]);
                totaldistance += rp.Distance;
                rp.TotalDistance = totaldistance;
                runtimePoints.Add(rp);

                /*
                if (i > 1)
                {
                    Debug.DrawLine(runtimePoints[i - 1].Point, runtimePoints[i].Point, Color.blue, 100.0f);
                }
                */
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

