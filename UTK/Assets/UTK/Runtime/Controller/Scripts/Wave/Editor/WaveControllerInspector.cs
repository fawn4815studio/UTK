using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UTK.Runtime.Controller.Wave
{
    [CustomEditor(typeof(WaveController))]
    public class WaveControllerInspector : Editor
    {
        SerializedProperty waveDatas;
        ReorderableList reorderableList;

        int waveSelectedIndex = -1;
        int openWaveIndex = -1;

        float coneSize = 2.0f;
        bool autoFocus = false;
        bool showAllPointGizmo = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("New"))
                {
                    waveDatas.InsertArrayElementAtIndex(waveDatas.arraySize);
                    var newelement = waveDatas.GetArrayElementAtIndex(waveDatas.arraySize - 1);
                    newelement.FindPropertyRelative("points").arraySize = 0;
                    newelement.FindPropertyRelative("name").stringValue = "New Wave Data" + waveDatas.arraySize;
                    waveSelectedIndex = waveDatas.arraySize - 1;
                }

                if (GUILayout.Button("Delete"))
                {
                    if (waveSelectedIndex < waveDatas.arraySize)
                    {
                        waveDatas.DeleteArrayElementAtIndex(waveSelectedIndex);
                        waveSelectedIndex--;
                        reorderableList = null;
                        openWaveIndex = -1;
                    }
                }

                if (GUILayout.Button("Open"))
                {
                    openWaveIndex = waveSelectedIndex;
                    InitReorderableList(waveDatas.GetArrayElementAtIndex(waveSelectedIndex));
                }
            }

            EditorGUILayout.EndHorizontal();

            if (waveSelectedIndex >= 0)
            {
                var element = waveDatas.GetArrayElementAtIndex(waveSelectedIndex);
                var name = element.FindPropertyRelative("name");
                var priority = element.FindPropertyRelative("priority");
                var istimelimit = element.FindPropertyRelative("isTimeLimit");
                var limittime = element.FindPropertyRelative("limitTime");

                var newname = EditorGUILayout.TextField("Name", name.stringValue);
                if (string.IsNullOrEmpty(newname) == false)
                {
                    name.stringValue = newname;
                }

                priority.intValue = EditorGUILayout.IntSlider("Priority", priority.intValue, 0, 100);
                istimelimit.boolValue = EditorGUILayout.Toggle("Time Limit", istimelimit.boolValue);

                if (istimelimit.boolValue)
                {
                    limittime.floatValue = EditorGUILayout.Slider("Limit Time", limittime.floatValue, 0, 100);
                }
            }

            if (waveDatas.arraySize >= 0)
            {
                var popups = new string[waveDatas.arraySize];

                for (int i = 0; i < waveDatas.arraySize; i++)
                {
                    popups[i] = waveDatas.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
                }

                waveSelectedIndex = EditorGUILayout.Popup("Waves", waveSelectedIndex, popups);
            }

            if (reorderableList != null)
            {
                reorderableList.DoLayoutList();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DEBUG SETTING");
            coneSize = EditorGUILayout.FloatField("Cone Size", coneSize);
            autoFocus = GUILayout.Toggle(autoFocus, "Auto focus on select element");
            showAllPointGizmo = GUILayout.Toggle(showAllPointGizmo, "Show all point gizmo");

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Show main gizmo"))
                {
                    Tools.hidden = false;
                }

                if (GUILayout.Button("Hide main gizmo"))
                {
                    Tools.hidden = true;
                }
            }
            EditorGUILayout.EndHorizontal();

        }

        private void OnEnable()
        {
            Tools.hidden = true;
            waveDatas = serializedObject.FindProperty("waveDatas");
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        private void OnSceneGUI()
        {
            if (openWaveIndex >= 0)
            {
                Handles.color = Color.yellow;

                var selectwave = waveDatas.GetArrayElementAtIndex(openWaveIndex);
                var points = selectwave.FindPropertyRelative("points");
                var size = points.arraySize;

                for (int i = 0; i < size; i++)
                {
                    var element = points.GetArrayElementAtIndex(i);
                    var pos = element.FindPropertyRelative("position");
                    var rot = element.FindPropertyRelative("rot");

                    if (showAllPointGizmo)
                    {
                        pos.vector3Value = Handles.PositionHandle(pos.vector3Value, Quaternion.identity);
                        serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        //Select index.
                        if (reorderableList != null && reorderableList.index == i)
                        {
                            pos.vector3Value = Handles.PositionHandle(pos.vector3Value, Quaternion.identity);
                            serializedObject.ApplyModifiedProperties();
                        }
                    }

                    Handles.ConeHandleCap(10, pos.vector3Value, Quaternion.Euler(-90, 0, 0), coneSize, EventType.Repaint);
                }
            }

        }

        private void InitReorderableList(SerializedProperty element)
        {
            reorderableList = null;

            var points = element.FindPropertyRelative("points");

            reorderableList = new ReorderableList(serializedObject, points);
            reorderableList.elementHeight = 220;

            reorderableList.drawHeaderCallback = (rect) =>
                     EditorGUI.LabelField(rect, element.FindPropertyRelative("name").stringValue);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {

                var drawelement = points.GetArrayElementAtIndex(index);
                var data = drawelement.FindPropertyRelative("data");
                var position = drawelement.FindPropertyRelative("position");
                var rot = drawelement.FindPropertyRelative("rot");
                var raycastGround = drawelement.FindPropertyRelative("raycastGround");
                var spawn = drawelement.FindPropertyRelative("spawn");
                var spawneffect = drawelement.FindPropertyRelative("spawnEffect");
                var delayeffect = drawelement.FindPropertyRelative("delayEffect");
                var delaytime = drawelement.FindPropertyRelative("delayTime");
                var ignorecount = drawelement.FindPropertyRelative("ignoreCount");
                var raycastoffset = drawelement.FindPropertyRelative("raycastOffset");

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                var e = EditorGUI.EnumPopup(rect, "SpawnType", (WaveData.WavePoint.SpawnType)spawn.enumValueIndex);
                spawn.enumValueIndex = (int)(WaveData.WavePoint.SpawnType)e;

                if ((Wave.WaveData.WavePoint.SpawnType)e == WaveData.WavePoint.SpawnType.Delay)
                {
                    rect.y += 20;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    delayeffect.stringValue = EditorGUI.TextField(rect, "Delay Effect", delayeffect.stringValue);

                    rect.y += 20;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    delaytime.floatValue = EditorGUI.Slider(rect, "Delay Time", delaytime.floatValue, 0, 100);
                }

                rect.y += 20;
                spawneffect.stringValue = EditorGUI.TextField(rect, "Spawn Effect", spawneffect.stringValue);

                rect.y += 20;
                data.stringValue = EditorGUI.TextField(rect, "Data", data.stringValue);

                rect.y += 20;
                position.vector3Value = EditorGUI.Vector3Field(rect, "Data Position", position.vector3Value);

                rect.y += 20;
                rot.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(rect, "Data Rotation", rot.quaternionValue.eulerAngles));

                var defaultrectwidth = rect.width;
                rect.width = 120;

                rect.y += 20;
                raycastGround.boolValue = EditorGUI.ToggleLeft(rect, "Raycast Ground", raycastGround.boolValue);

                rect.x += 120;
                ignorecount.boolValue = EditorGUI.ToggleLeft(rect, "Ignore Count", ignorecount.boolValue);

                if (raycastGround.boolValue)
                {
                    rect.y += 20;
                    rect.x -= 120;
                    rect.width = defaultrectwidth;
                    raycastoffset.vector3Value = EditorGUI.Vector3Field(rect, "Raycast Offset", raycastoffset.vector3Value);
                }
            };

            reorderableList.onAddCallback += (list) =>
            {

                points.arraySize++;
                list.index = points.arraySize - 1;
                var addelement = points.GetArrayElementAtIndex(list.index);
            };

            reorderableList.onSelectCallback += (ReorderableList list) =>
            {
                var selectelement = points.GetArrayElementAtIndex(list.index);
                var pos = selectelement.FindPropertyRelative("position");

                //Focus the scene camera on the selected element.
                if (autoFocus)
                {
                    var view = Tool.Common.ToolUtility.LastActiveView;
                    if (view != null)
                    {
                        view.LookAt(pos.vector3Value);
                    }
                }
            };

        }
    }
}

