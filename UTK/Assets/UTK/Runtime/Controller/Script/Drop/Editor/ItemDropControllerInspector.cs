using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UTK.Runtime.Controller.Drop
{
    [CustomEditor(typeof(ItemDropController))]
    public class ItemDropControllerInspector : Editor
    {
        /// <summary>
        /// Reference object set in the <see cref="dropDatas"/>.
        /// </summary>
        Dictionary<string, Object> objectDatas;

        /// <summary>
        /// GameObject for preview generated based on <see cref="objectDatas"/>
        /// </summary>
        Dictionary<string, GameObject> previewDatas;

        /// <summary>
        /// The object currently displayed as a preview.
        /// </summary>
        GameObject currentPreviewObject;

        SerializedProperty degreeOfProgress;
        SerializedProperty dropDatas;
        ReorderableList reorderableList;

        float cubeSize = 0.5f;
        bool autoFocus = true;
        bool copyScale = true;

        private void OnEnable()
        {
            Tools.hidden = true;
            objectDatas = new Dictionary<string, Object>();
            previewDatas = new Dictionary<string, GameObject>();
            degreeOfProgress = serializedObject.FindProperty("degreeOfProgress");
            dropDatas = serializedObject.FindProperty("dropDatas");

            reorderableList = new ReorderableList(serializedObject, dropDatas);
            reorderableList.elementHeight = 160;

            reorderableList.drawHeaderCallback = (rect) =>
                     EditorGUI.LabelField(rect, dropDatas.displayName);

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {

                var element = dropDatas.GetArrayElementAtIndex(index);

                var evaluationvalue = element.FindPropertyRelative("evaluationValue");
                var datapath = element.FindPropertyRelative("dataPath");
                var dataid = element.FindPropertyRelative("dataId");
                var position = element.FindPropertyRelative("position");
                var rotation = element.FindPropertyRelative("rotation");
                var scale = element.FindPropertyRelative("scale");

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                evaluationvalue.intValue = EditorGUI.IntField(rect, "Evaluation value", evaluationvalue.intValue);

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;

                Object ob = null;
                if (objectDatas.ContainsKey(dataid.stringValue))
                {
                    ob = objectDatas[dataid.stringValue];
                }
                else
                {
                    ob = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(dataid.stringValue), typeof(Object));
                }

                ob = EditorGUI.ObjectField(rect, "Data", ob, typeof(GameObject), false);

                if (ob != null)
                {
                    var path = AssetDatabase.GetAssetPath(ob);
                    var id = AssetDatabase.AssetPathToGUID(path);

                    //TODO : Support asset bundle
                    //Do not set if the object is not in the Resources folder
                    if (path.Contains("Resources"))
                    {
                        string[] del = { "Resources/" };
                        datapath.stringValue = path.Split(del, System.StringSplitOptions.None)[1].Replace(".prefab", string.Empty);
                        dataid.stringValue = id;
                        objectDatas[dataid.stringValue] = ob;
                    }
                    else
                    {
                        Debug.LogWarning("Data must be a prefab located Resources folder or must be single asset bundle.");
                    }
                }

                /*
                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.TextField(rect, "Id ", dataid.stringValue); //Read only
                */

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                position.vector3Value = EditorGUI.Vector3Field(rect, "Position", position.vector3Value);

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                rotation.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(rect, "Rotation", rotation.quaternionValue.eulerAngles));

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                scale.vector3Value = EditorGUI.Vector3Field(rect, "Scale", scale.vector3Value);

            };

            reorderableList.onAddCallback += (list) =>
            {
                dropDatas.arraySize++;
                list.index = dropDatas.arraySize - 1;
                var element = dropDatas.GetArrayElementAtIndex(list.index);

                if (!copyScale)
                {
                    element.FindPropertyRelative("scale").vector3Value = Vector3.one;
                }
            
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            };

            reorderableList.onSelectCallback += (ReorderableList list) =>
            {
                var element = dropDatas.GetArrayElementAtIndex(list.index);
                var dataid = element.FindPropertyRelative("dataId");

                //Focus the scene camera on the selected element.
                if (autoFocus && previewDatas.ContainsKey(dataid.stringValue))
                {
                    var view = Tool.Common.ToolUtility.LastActiveView;
                    if (view != null)
                    {
                        view.LookAt(previewDatas[dataid.stringValue].transform.position);
                    }
                }
            };

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            degreeOfProgress.intValue = EditorGUILayout.IntField("Degree of progress", degreeOfProgress.intValue);

            EditorGUILayout.Space();
            reorderableList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DEBUG SETTING");
            cubeSize = EditorGUILayout.FloatField("Cube Size", cubeSize);

            autoFocus = GUILayout.Toggle(autoFocus, "Auto focus on select element");
            copyScale = GUILayout.Toggle(copyScale, "Inherit the copy source scale value");

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

            serializedObject.ApplyModifiedProperties();

            //If reorderableList becomes empty, delete all preview data
            if (reorderableList.count == 0)
            {
                foreach (var o in previewDatas)
                {
                    DestroyImmediate(o.Value);
                }
            }

        }

        private void OnSceneGUI()
        {
            foreach (var p in previewDatas)
            {
                p.Value.SetActive(false);
            }

            var size = dropDatas.arraySize;
            if (size <= 0) return;

            for (int i = 0; i < size; i++)
            {
                var element = dropDatas.GetArrayElementAtIndex(i);
                var dataid = element.FindPropertyRelative("dataId");
                var position = element.FindPropertyRelative("position");
                var rotation = element.FindPropertyRelative("rotation");
                var scale = element.FindPropertyRelative("scale");

                //Select index.
                if (reorderableList.index == i)
                {
                    //Ignore if asset is not found
                    if (!objectDatas.ContainsKey(dataid.stringValue)) return;

                    //Set preview data.
                    if (!previewDatas.ContainsKey(dataid.stringValue))
                    {
                        previewDatas[dataid.stringValue] = (GameObject)Instantiate(objectDatas[dataid.stringValue], Vector3.zero, Quaternion.identity);
                        previewDatas[dataid.stringValue].hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
                    }
                    else
                    {
                        previewDatas[dataid.stringValue].SetActive(true);
                    }

                    currentPreviewObject = previewDatas[dataid.stringValue];

                    //Inital preview transform.
                    currentPreviewObject.transform.position = position.vector3Value;
                    currentPreviewObject.transform.rotation = rotation.quaternionValue;
                    currentPreviewObject.transform.localScale = scale.vector3Value;

                    switch (UnityEditor.Tools.current)
                    {
                        case UnityEditor.Tool.Move:

                            if (Tools.pivotRotation == PivotRotation.Global)
                            {
                                position.vector3Value = Handles.PositionHandle(position.vector3Value, Quaternion.identity);

                            }
                            else
                            {
                                position.vector3Value = Handles.PositionHandle(position.vector3Value, rotation.quaternionValue);
                            }

                            break;

                        case UnityEditor.Tool.Rotate:

                            //TODO : Correspond to the global rotating gizmo.
                            rotation.quaternionValue = Handles.RotationHandle(rotation.quaternionValue, position.vector3Value);
                            break;

                        case UnityEditor.Tool.Scale:
                            scale.vector3Value = Handles.ScaleHandle(scale.vector3Value, position.vector3Value, rotation.quaternionValue, HandleUtility.GetHandleSize(position.vector3Value));
                            break;
                    }
                }
                else
                {
                    Handles.color = Color.blue;
                    Handles.CubeHandleCap(10, position.vector3Value, rotation.quaternionValue, cubeSize, EventType.Repaint);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDisable()
        {
            foreach (var o in previewDatas)
            {
                DestroyImmediate(o.Value);
            }

            Tools.hidden = false;
        }

        private void OnDestroy()
        {
            foreach (var o in previewDatas)
            {
                DestroyImmediate(o.Value);
            }

            Tools.hidden = false;
        }

    }
}
