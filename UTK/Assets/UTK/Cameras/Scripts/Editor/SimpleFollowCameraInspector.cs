using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTK.Cameras
{
    [CustomEditor(typeof(SimpleFollowCamera))]
    public class SimpleFollowCameraInspector : Editor
    {
        SimpleFollowCamera camera;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            camera = target as SimpleFollowCamera;
            if (camera == null)
            {
                return;
            }

            if (GUILayout.Button("Apply offset (No smmoth)"))
            {
                camera.Editor_AcceptOffset();
            }
        }
    }

}
