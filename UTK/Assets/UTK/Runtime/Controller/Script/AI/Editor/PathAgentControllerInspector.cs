using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UTK.Runtime.Controller.AI
{
    [CustomEditor(typeof(PathAgentController))]
    public class PathAgentControllerInspector : Editor
    {
        SerializedProperty pathDatas;
        SerializedProperty goalJudgeRemainingDistance;
        ReorderableList reorderableList;

        float coneSize = 0.5f;

        private void OnEnable()
        {
            pathDatas = serializedObject.FindProperty("pathDatas");
            goalJudgeRemainingDistance = serializedObject.FindProperty("goalJudgeRemainingDistance");

            reorderableList = new ReorderableList(serializedObject, pathDatas);
            reorderableList.elementHeight = 50;

            reorderableList.drawHeaderCallback = (rect) =>
                     EditorGUI.LabelField(rect, pathDatas.displayName);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {

                var element = pathDatas.GetArrayElementAtIndex(index);
                var position = element.FindPropertyRelative("position");
                var interval = element.FindPropertyRelative("interval");

                position.vector3Value = EditorGUI.Vector3Field(rect, "Position " + index, position.vector3Value);

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                interval.floatValue = EditorGUI.FloatField(rect, "Interval", interval.floatValue);
            };

            reorderableList.onAddCallback += (list) =>
            {

                pathDatas.arraySize++;
                list.index = pathDatas.arraySize - 1;
                var element = pathDatas.GetArrayElementAtIndex(list.index);
            };

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            goalJudgeRemainingDistance.floatValue = EditorGUILayout.FloatField("goal judge distance", goalJudgeRemainingDistance.floatValue);
            reorderableList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DEBUG SETTING");
            coneSize = EditorGUILayout.FloatField("Cone Size", coneSize);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            var size = pathDatas.arraySize;
            if (size <= 0) return;

            //Draw first element cone.
            Handles.color = Color.yellow;
            Handles.ConeHandleCap(10, pathDatas.GetArrayElementAtIndex(0).FindPropertyRelative("position").vector3Value + new Vector3(0, 0.3f, 0), Quaternion.Euler(-90, 0, 0), coneSize, EventType.Repaint);

            if (size > 1)
            {
                for (int i = 1; i < size; i++)
                {
                    var previous = pathDatas.GetArrayElementAtIndex(i - 1).FindPropertyRelative("position");
                    var current = pathDatas.GetArrayElementAtIndex(i).FindPropertyRelative("position");

                    Handles.DrawLine(previous.vector3Value, current.vector3Value);
                    Handles.ConeHandleCap(10, current.vector3Value + new Vector3(0, 0.3f, 0), Quaternion.Euler(-90, 0, 0), coneSize, EventType.Repaint);
                }

                //Combine first and last.
                var first = pathDatas.GetArrayElementAtIndex(0).FindPropertyRelative("position");
                var last = pathDatas.GetArrayElementAtIndex(size - 1).FindPropertyRelative("position");
                Handles.DrawLine(first.vector3Value, last.vector3Value);
            }

            //Select index.
            if (reorderableList.index != -1) //-1 == No select
            {
                var select = pathDatas.GetArrayElementAtIndex(reorderableList.index).FindPropertyRelative("position");
                select.vector3Value = Handles.PositionHandle(select.vector3Value, Quaternion.identity);
                serializedObject.ApplyModifiedProperties();
            }

        }

    }

}
