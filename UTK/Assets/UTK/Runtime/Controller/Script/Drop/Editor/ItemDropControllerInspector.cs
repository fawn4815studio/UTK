using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTK.Runtime.Controller.Drop
{
    [CustomEditor(typeof(ItemDropController))]
    public class ItemDropControllerInspector : Editor
    {

        private void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();



            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {

        }
    }
}
