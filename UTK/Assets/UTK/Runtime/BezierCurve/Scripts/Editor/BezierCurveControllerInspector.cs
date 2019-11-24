using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UTK.Tool.Common;

namespace UTK.Runtime.BezierCurve
{
    [CustomEditor(typeof(BezierCurveController))]
    public class BezierCurveControllerInspector : Editor
    {
        GameObject rootObject;
        GameObject previewRootObject;
        SerializedProperty points;
        SerializedProperty loop;
        SerializedProperty time;
        SerializedProperty segment;
        SerializedProperty autoLookDirection;
        ReorderableList reorderableList;

        private class PreviewPoint
        {
            SerializedProperty element;
            GameObject parentobject;
            GameObject anchoreobject;
            GameObject handle1object;
            GameObject handle2object;

            public SerializedProperty Element
            {
                get => element;
            }

            public SerializedProperty AnchoreProperty
            {
                get => element.FindPropertyRelative("anchore");
            }

            public SerializedProperty Handle1Property
            {
                get => element.FindPropertyRelative("handle1");
            }

            public SerializedProperty Handle2Property
            {
                get => element.FindPropertyRelative("handle2");
            }

            public GameObject AnchoreObject
            {
                get => anchoreobject;
            }

            public GameObject Handle1Object
            {
                get => handle1object;
            }

            public GameObject Handle2Object
            {
                get => handle2object;
            }

            public Vector3 Anchore
            {
                get => element.FindPropertyRelative("anchore").vector3Value;

                set
                {
                    element.FindPropertyRelative("anchore").vector3Value = value;
                }
            }

            public Vector3 Handle1
            {
                get => element.FindPropertyRelative("handle1").vector3Value;

                set
                {
                    element.FindPropertyRelative("handle1").vector3Value = value;
                }
            }

            public Vector3 Handle2
            {
                get => element.FindPropertyRelative("handle2").vector3Value;

                set
                {
                    element.FindPropertyRelative("handle2").vector3Value = value;
                }
            }

            public bool IsAnchoreSelect
            {
                get
                {
                    return Selection.objects.FirstOrDefault(t => t.Equals(anchoreobject)) != null;
                }

                set
                {

                    if (value)
                    {
                        Selection.activeGameObject = anchoreobject;
                    }
                    else
                    {
                        if (Selection.activeGameObject != null && Selection.activeGameObject.Equals(anchoreobject))
                        {
                            Selection.activeGameObject = null;
                        }
                    }
                }
            }

            public bool IsHandle1Select
            {
                get
                {
                    return Selection.objects.FirstOrDefault(t => t.Equals(handle1object)) != null;
                }

                set
                {
                    var selects = Selection.objects.ToList();

                    if (value)
                    {
                        if (!selects.Contains(handle1object))
                        {
                            selects.Add(handle1object);
                        }
                    }
                    else
                    {
                        if (selects.Contains(handle1object))
                        {
                            selects.Remove(handle1object);
                        }
                    }

                    Selection.objects = selects.ToArray();
                }
            }

            public bool IsHandle2Select
            {
                get
                {
                    return Selection.objects.FirstOrDefault(t => t.Equals(handle2object)) != null;
                }

                set
                {
                    var selects = Selection.objects.ToList();

                    if (value)
                    {
                        if (!selects.Contains(handle2object))
                        {
                            selects.Add(handle2object);
                        }
                    }
                    else
                    {
                        if (selects.Contains(handle2object))
                        {
                            selects.Remove(handle2object);
                        }
                    }

                    Selection.objects = selects.ToArray();
                }
            }

            public bool IsEqual(GameObject go)
            {
                return anchoreobject.Equals(go) || handle1object.Equals(go) || handle2object.Equals(go);
            }

            public PreviewPoint(SerializedProperty property, GameObject parent)
            {
                parentobject = parent;
                element = property;
                CreatePreviewObjectsIfNotExists();
            }

            public void CreatePreviewObjectsIfNotExists()
            {
                if (anchoreobject == null)
                {
                    anchoreobject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    anchoreobject.hideFlags = HideFlags.DontSave;
                    anchoreobject.transform.parent = parentobject.transform;
                    anchoreobject.transform.localPosition = Anchore;
                }

                if (handle1object == null)
                {
                    handle1object = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    handle1object.hideFlags = HideFlags.DontSave;
                    handle1object.transform.parent = anchoreobject.transform;
                    handle1object.transform.localPosition = Handle1;
                }

                if (handle2object == null)
                {
                    handle2object = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    handle2object.hideFlags = HideFlags.DontSave;
                    handle2object.transform.parent = anchoreobject.transform;
                    handle2object.transform.localPosition = Handle2;
                }
            }

            public void OnDestroy()
            {
                DestroyImmediate(anchoreobject);
            }

        }

        private List<PreviewPoint> previewPoints;

        private void OnEnable()
        {
            Selection.selectionChanged += SelectionChanged;
            Reset();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            loop = serializedObject.FindProperty("loop");
            loop.boolValue = EditorGUILayout.Toggle("Loop bezier curve", loop.boolValue);

            time = serializedObject.FindProperty("time");
            time.floatValue = EditorGUILayout.FloatField("Time to spend", time.floatValue);

            segment = serializedObject.FindProperty("segment");
            segment.intValue = EditorGUILayout.IntField("Path segment", segment.intValue);

            autoLookDirection = serializedObject.FindProperty("autoLookDirection");
            autoLookDirection.boolValue = EditorGUILayout.Toggle("Auto rotate", autoLookDirection.boolValue);

            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            CreatePreviewRootIfNotExists();
            previewRootObject.transform.position = rootObject.transform.position;
            previewRootObject.transform.rotation = rootObject.transform.rotation;

            //Undo and Redo may cause null to appear in Selection.
            //If null is entered, Reset Selection.
            if (Selection.objects.Contains(null))
            {
                ToolUtility.ResetSelection();
                return;
            }

            var size = points.arraySize;
            for (int i = 0; i < size; i++)
            {
                var pd = GetPreviewPointFromElement(points.GetArrayElementAtIndex(i));
                if (pd == null) continue;

                // Check every frame because it may be erased on the scene side.
                pd.CreatePreviewObjectsIfNotExists();

                BezierCurveCalculator.MoveAnchore(pd.AnchoreProperty, pd.Handle1Property, pd.Handle2Property, pd.AnchoreObject.transform.localPosition);

                if (pd.IsAnchoreSelect)
                {
                    BezierCurveCalculator.MoveHandle1UsingHandle(pd.AnchoreProperty, pd.Handle1Property, pd.Handle2Property, previewRootObject.transform);
                    BezierCurveCalculator.MoveHandle2UsingHandle(pd.AnchoreProperty, pd.Handle1Property, pd.Handle2Property, previewRootObject.transform);
                }

                if (pd.IsHandle1Select && !pd.IsAnchoreSelect)
                {
                    //Since Handle1 is moved by a dedicated function, the selected state is canceled.
                    pd.IsHandle1Select = false;
                    pd.IsAnchoreSelect = true;
                }

                if (pd.IsHandle2Select && !pd.IsAnchoreSelect)
                {
                    //Since Handle2 is moved by a dedicated function, the selected state is canceled.
                    pd.IsHandle2Select = false;
                    pd.IsAnchoreSelect = true;
                }

                //
                pd.Handle1Object.transform.position = rootObject.transform.TransformPoint(pd.Handle1);
                pd.Handle2Object.transform.position = rootObject.transform.TransformPoint(pd.Handle2);

                if (pd.IsAnchoreSelect)
                {
                    Handles.DrawLine(pd.Handle1Object.transform.position, pd.AnchoreObject.transform.position);
                    Handles.DrawLine(pd.AnchoreObject.transform.position, pd.Handle2Object.transform.position);
                }
            }

            DrawBezierCurve(BezierCurveCalculator.GetPoints(points, 50, previewRootObject.transform));
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void OnDisable()
        {
            DestroyImmediate(previewRootObject);
            Selection.selectionChanged -= SelectionChanged;
        }

        private void InitReorderableList()
        {

            reorderableList = new ReorderableList(serializedObject, points);
            reorderableList.elementHeight = 130;

            reorderableList.drawHeaderCallback = (rect) =>
                     EditorGUI.LabelField(rect, points.displayName);

            reorderableList.onReorderCallback = (rect) =>
            {
                Reset();
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = points.GetArrayElementAtIndex(index);
                var anchore = element.FindPropertyRelative("anchore");
                var handle1 = element.FindPropertyRelative("handle1");
                var handle2 = element.FindPropertyRelative("handle2");
                var pd = GetPreviewPointFromElement(element);

                if (pd == null) return;

                var newanchorepos = EditorGUI.Vector3Field(rect, "Anchore", anchore.vector3Value);
                BezierCurveCalculator.MoveAnchore(anchore, handle1, handle2, newanchorepos);
                pd.AnchoreObject.transform.localPosition = newanchorepos;

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                handle1.vector3Value = EditorGUI.Vector3Field(rect, "Handle1", handle1.vector3Value);

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                handle2.vector3Value = EditorGUI.Vector3Field(rect, "Handle2", handle2.vector3Value);

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = 80;
                if (GUI.Button(rect, "SmoothX"))
                {
                    BezierCurveCalculator.ToSmooth(anchore, handle1, handle2, UnityEngine.Animations.Axis.X, 5);
                }

                rect.x += 90;
                rect.width = 80;
                if (GUI.Button(rect, "SmoothY"))
                {
                    BezierCurveCalculator.ToSmooth(anchore, handle1, handle2, UnityEngine.Animations.Axis.Y, 5);
                }

                rect.x += 90;
                rect.width = 80;
                if (GUI.Button(rect, "SmoothZ"))
                {
                    BezierCurveCalculator.ToSmooth(anchore, handle1, handle2, UnityEngine.Animations.Axis.Z, 5);
                }

                rect.y += 20;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.x -= 180;
                rect.width = 80;
                if (GUI.Button(rect, "Corner"))
                {
                    BezierCurveCalculator.ToCorner(anchore, handle1, handle2);
                }
            };

            reorderableList.onAddCallback += (ReorderableList list) =>
            {
                points.arraySize++;
                list.index = points.arraySize - 1;
                var element = points.GetArrayElementAtIndex(list.index);

                CreatePreviewPoint(element).IsAnchoreSelect = true;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            };

            reorderableList.onSelectCallback += (ReorderableList list) =>
            {
                var element = points.GetArrayElementAtIndex(list.index);
                var pd = GetPreviewPointFromElement(element);

                if (pd != null)
                {
                    ToolUtility.ResetSelection();
                    pd.IsAnchoreSelect = true;
                }
            };


            reorderableList.onRemoveCallback += (ReorderableList list) =>
            {
                var element = points.GetArrayElementAtIndex(list.index);

                points.DeleteArrayElementAtIndex(list.index);
                Reset();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            };

            var size = points.arraySize;
            for (int i = 0; i < size; i++)
            {
                var element = points.GetArrayElementAtIndex(i);
                CreatePreviewPoint(element);
            }
        }

        private void CreatePreviewRootIfNotExists()
        {
            if (GameObject.Find("BezierCurveControllerInspector_preview_root") != null) return;

            if (previewRootObject == null)
            {
                previewRootObject = new GameObject("BezierCurveControllerInspector_preview_root");
                previewRootObject.hideFlags = HideFlags.DontSaveInEditor;
            }
        }

        private PreviewPoint CreatePreviewPoint(SerializedProperty element)
        {
            if (previewRootObject == null) return null;
            var p = new PreviewPoint(element, previewRootObject);
            previewPoints.Add(p);
            return p;
        }

        private PreviewPoint GetPreviewPointFromElement(SerializedProperty element)
        {
            for (int i = 0; i < previewPoints.Count; i++)
            {
                if (SerializedProperty.EqualContents(previewPoints[i].Element, element))
                {
                    return previewPoints[i];
                }
            }

            return null;
        }

        private void SelectionChanged()
        {
            var pd = previewPoints.FirstOrDefault(t => t.IsEqual(Selection.activeGameObject));
            if (pd == null) return;

            var size = points.arraySize;
            for (int i = 0; i < size; i++)
            {
                var element = points.GetArrayElementAtIndex(i);
                if (SerializedProperty.EqualContents(element, pd.Element))
                {
                    reorderableList.index = i; //Change to selected state
                    return;
                }
            }
        }

        private void DrawBezierCurve(List<Vector3> points)
        {
            var linerenderer = previewRootObject.GetComponent<LineRenderer>();

            if (linerenderer == null)
            {
                linerenderer = previewRootObject.AddComponent<LineRenderer>();
                linerenderer.material = new Material(Shader.Find("Sprites/Default"));
                linerenderer.startColor = Color.red;
                linerenderer.endColor = Color.blue;
                linerenderer.startWidth = 0.25f;
                linerenderer.endWidth = 0.25f;
                linerenderer.numCapVertices = 90;
                linerenderer.numCornerVertices = 90;
            }

            linerenderer.positionCount = points.Count;
            linerenderer.SetPositions(points.ToArray());
        }

        private void Reset()
        {
            reorderableList = null;
            points = serializedObject.FindProperty("points");
            rootObject = (target as BezierCurveController).gameObject;

            previewPoints = null;
            previewPoints = new List<PreviewPoint>();

            DestroyImmediate(previewRootObject);
            CreatePreviewRootIfNotExists();
            InitReorderableList();
        }

    }
}
