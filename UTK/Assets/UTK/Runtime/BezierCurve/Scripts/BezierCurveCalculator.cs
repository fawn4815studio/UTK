using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTK.Runtime.BezierCurve
{
    public static class BezierCurveCalculator
    {
        public static List<Vector3> GetPoints(List<BezierPoint> pathlist, int segment)
        {
            var points = new List<Vector3>();
            float fs = segment; //to float.

            for (var i = 1; i < pathlist.Count; i++)
            {
                var point0 = pathlist[i - 1].Anchore;
                var point1 = pathlist[i - 1].Handle2;
                var point2 = pathlist[i].Handle1;
                var point3 = pathlist[i].Anchore;

                for (var j = 0; j <= segment; j++)
                {
                    points.Add(GetPoint(point0, point1, point2, point3, j / fs));
                }
            }

            return points;
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            var omt = 1f - t;

            return omt * omt * omt * p0 +
                   3f * omt * omt * t * p1 +
                   3f * omt * t * t * p2 +
                   t * t * t * p3;
        }

        public static void ToSmooth(BezierPoint point, Axis axis, float length)
        {
            switch (axis)
            {
                case Axis.X:
                    point.Handle1 = point.Anchore + Vector3.right * length;
                    point.Handle2 = point.Anchore + Vector3.left * length;
                    break;

                case Axis.Y:
                    point.Handle1 = point.Anchore + Vector3.up * length;
                    point.Handle2 = point.Anchore + Vector3.down * length;
                    break;

                case Axis.Z:
                    point.Handle1 = point.Anchore + Vector3.forward * length;
                    point.Handle2 = point.Anchore + Vector3.back * length;
                    break;
            }

        }

        public static void ToCorner(BezierPoint point)
        {
            point.Handle1 = point.Anchore;
            point.Handle2 = point.Anchore;
        }

        public static void MoveAnchore(BezierPoint point, Vector3 newpos)
        {
            var direction = newpos - point.Anchore;
            point.Handle1 += direction;
            point.Handle2 += direction;
            point.Anchore = newpos;
        }

        public static void MoveHandle1(BezierPoint point, Vector3 newpos)
        {
            point.Handle1 = newpos;
            var direction = (point.Anchore - point.Handle1).normalized;
            var length = (point.Anchore - point.Handle2).magnitude;
            point.Handle2 = point.Anchore + direction * length;
        }

        public static void MoveHandle2(BezierPoint point, Vector3 newpos)
        {
            point.Handle2 = newpos;
            var direction = (point.Anchore - point.Handle2).normalized;
            var length = (point.Anchore - point.Handle1).magnitude;
            point.Handle1 = point.Anchore + direction * length;
        }

        public static void MoveHandle1Symmetry(BezierPoint point, Vector3 newpos)
        {
            point.Handle1 = newpos;
            point.Handle2 = Vector3.LerpUnclamped(newpos, point.Anchore, 2f);
        }

        public static void MoveHandle2Symmetry(BezierPoint point, Vector3 newpos)
        {
            point.Handle2 = newpos;
            point.Handle1 = Vector3.LerpUnclamped(newpos, point.Anchore, 2f);
        }


#if UNITY_EDITOR

        public static List<Vector3> GetPoints(SerializedProperty pathlist, int segment)
        {
            var points = new List<Vector3>();
            float fs = segment; //to float.

            for (var i = 1; i < pathlist.arraySize; i++)
            {
                var preelement = pathlist.GetArrayElementAtIndex(i - 1);
                var element = pathlist.GetArrayElementAtIndex(i);
                var point0 = preelement.FindPropertyRelative("anchore").vector3Value;
                var point1 = preelement.FindPropertyRelative("handle2").vector3Value;
                var point2 = element.FindPropertyRelative("handle1").vector3Value;
                var point3 = element.FindPropertyRelative("anchore").vector3Value;


                for (var j = 0; j <= segment; j++)
                {
                    points.Add(GetPoint(point0, point1, point2, point3, j / fs));
                }
            }

            return points;
        }

        public static void MoveAnchore(SerializedProperty anchore, SerializedProperty handle1, SerializedProperty handle2, Vector3 newpos)
        {
            var direction = newpos - anchore.vector3Value;
            handle1.vector3Value += direction;
            handle2.vector3Value += direction;
            anchore.vector3Value = newpos;
        }

        public static void MoveHandle1UsingHandle(SerializedProperty anchore, SerializedProperty handle1, SerializedProperty handle2, Transform parent)
        {
            var newpos = Handles.PositionHandle(parent.TransformPoint(handle1.vector3Value), Quaternion.identity);
            handle1.vector3Value = parent.InverseTransformPoint(newpos);

            var direction = (anchore.vector3Value - handle1.vector3Value).normalized;
            var length = (anchore.vector3Value - handle2.vector3Value).magnitude;
            handle2.vector3Value = anchore.vector3Value + direction * length;
        }

        public static void MoveHandle2UsingHandle(SerializedProperty anchore, SerializedProperty handle1, SerializedProperty handle2, Transform parent)
        {
            var newpos = Handles.PositionHandle(parent.TransformPoint(handle2.vector3Value), Quaternion.identity);
            handle2.vector3Value = parent.InverseTransformPoint(newpos);

            var direction = (anchore.vector3Value - handle2.vector3Value).normalized;
            var length = (anchore.vector3Value - handle1.vector3Value).magnitude;
            handle1.vector3Value = anchore.vector3Value + direction * length;
        }

        public static void ToSmooth(SerializedProperty anchore, SerializedProperty handle1, SerializedProperty handle2, Axis axis, float length)
        {
            switch (axis)
            {
                case Axis.X:
                    handle1.vector3Value = anchore.vector3Value + Vector3.right * length;
                    handle2.vector3Value = anchore.vector3Value + Vector3.left * length;
                    break;

                case Axis.Y:
                    handle1.vector3Value = anchore.vector3Value + Vector3.up * length;
                    handle2.vector3Value = anchore.vector3Value + Vector3.down * length;
                    break;

                case Axis.Z:
                    handle1.vector3Value = anchore.vector3Value + Vector3.forward * length;
                    handle2.vector3Value = anchore.vector3Value + Vector3.back * length;
                    break;
            }
        }

        public static void ToCorner(SerializedProperty anchore, SerializedProperty handle1, SerializedProperty handle2)
        {
            handle1.vector3Value = anchore.vector3Value;
            handle2.vector3Value = anchore.vector3Value;
        }
#endif
    }
}

