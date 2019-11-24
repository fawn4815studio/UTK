using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTK.Runtime.Controller.Simple
{
    [CustomEditor(typeof(SimpleFollowCameraController))]
    public class SimpleFollowCameraControllerInspector : Editor
    {
        SimpleFollowCameraController camera;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            camera = target as SimpleFollowCameraController;
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
