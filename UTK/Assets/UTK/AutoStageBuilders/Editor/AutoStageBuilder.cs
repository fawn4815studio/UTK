using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UTK.AutoStageBuilder
{
    public class AutoStageBuilder : EditorWindow
    {
        static AutoStageBuilder autoStageBuilder;

        enum ViewerTabType : int
        {
            MainProp,
            Option
        }
        readonly string[] tabs = new string[] { "Main Prop","Option" };
        int currentTabIndex;
        Vector2 mainPropDataScrollPos;
      
        private Runtime.StageData editStageData;
        private string dataSavePath = null;
        private string stageDataName;
        private bool isDirty = false;

        [MenuItem("UTK/Procedural/AutoStageBuilder", false, 2)]
        static void Open()
        {
            if (autoStageBuilder == null)
            {
               autoStageBuilder = CreateInstance<AutoStageBuilder>();
            }

            autoStageBuilder.minSize = new Vector2(500, 400);
            autoStageBuilder.titleContent.text = "AutoStageBuilder";
            autoStageBuilder.ShowUtility();
        }

        #region Internal

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if(GUILayout.Button("Create"))
                {
                    editStageData = new Runtime.StageData();
                    stageDataName = "NoTitle";
                }

                if(GUILayout.Button("Open"))
                {
                    //First check if the current editing is saved.
                    if(editStageData != null && isDirty)
                    {
                        if(EditorUtility.DisplayDialog("Stage Data Have Been Modified", "Do you want to save the changes you made in the stage datas", "Save","Don't Save"))
                        {
                            if (!Save()) return;
                        }
                    }

                    Load();
                }

                if (editStageData != null && GUILayout.Button("Save"))
                {
                    Save();
                }
            }

            if (editStageData == null) return;

            EditorGUI.BeginChangeCheck();

            var stagename = stageDataName;
            if (isDirty) stagename += "*";
            EditorGUILayout.LabelField(stagename);
            EditorGUILayout.Space();

            currentTabIndex = GUILayout.Toolbar(currentTabIndex, tabs);

            if(currentTabIndex == (int)ViewerTabType.MainProp)
            {
                MainPropGUI();
            }
            else if(currentTabIndex == (int)ViewerTabType.Option)
            {

            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if(editStageData.mainPropCreationData.propDatas.Count > 0)
                {
                    if (GUILayout.Button("Generate"))
                    {
                        if (CheckStageDataCollect())
                        {
                            Runtime.RuntimeAutoStageBuilderManager.Instance.GenerateStageSync(editStageData);
                            DestroyImmediate(GameObject.FindObjectOfType<Runtime.RuntimeAutoStageBuilderManager>().gameObject);

                            var scene = SceneManager.GetActiveScene();
                            EditorSceneManager.MarkSceneDirty(scene);
                        }
                    }
                }
             
                if (GUILayout.Button("Clear(name base)"))
                {
                    var roots = Array.FindAll(FindObjectsOfType<GameObject>(), (item) => (item.transform.parent == null && item.name == stageDataName));
                    foreach(var r in roots)
                    {
                        DestroyImmediate(r);
                    }

                    var scene = SceneManager.GetActiveScene();
                    EditorSceneManager.MarkSceneDirty(scene);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                isDirty = true;
            }
        }

        private void MainPropGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                var data = editStageData.mainPropCreationData;

                EditorGUILayout.LabelField("Creation count");
                data.creationCount = EditorGUILayout.IntField(data.creationCount);

                data.origin = EditorGUILayout.Vector3Field("Origin", data.origin);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Mode");
                data.mode = (Runtime.MainPropCreationMode)GUILayout.Toolbar((int)data.mode, new string[] { "Random", "Direction" });

                if (data.mode == Runtime.MainPropCreationMode.Direction)
                {
                    data.direction = EditorGUILayout.Vector3Field("Creation direction", data.direction);
                    EditorGUILayout.LabelField("Distance between prop");
                    data.distanceBetweenProp = EditorGUILayout.FloatField(data.distanceBetweenProp);
                }
                else if (data.mode == Runtime.MainPropCreationMode.Random)
                {
                    EditorGUILayout.LabelField("Whether sure to concatenate main prop");
                    data.whetherSureToConcatenateProps = EditorGUILayout.Toggle(data.whetherSureToConcatenateProps);
                }

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Add", GUILayout.Width(100)))
                    {
                        data.propDatas.Add(new Runtime.MainPropData());
                    }

                    if (GUILayout.Button("Remove All", GUILayout.Width(100)))
                    {
                        data.propDatas.Clear();
                    }
                }

                mainPropDataScrollPos = EditorGUILayout.BeginScrollView(mainPropDataScrollPos, GUI.skin.box);
                {
                    Runtime.MainPropData removepropdata = null;
                    foreach (var prop in data.propDatas)
                    {
                        EditorGUILayout.Space();
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                        {
                            if (GUILayout.Button("Remove", GUILayout.Width(100)))
                            {
                                removepropdata = prop;
                            }

                            EditorGUILayout.LabelField("Source object");
                            prop.prefabSource = (GameObject)EditorGUILayout.ObjectField(prop.prefabSource, typeof(GameObject), false);

                            //Check if it belongs under the Resources folder.
                            if (prop.prefabSource != null)
                            {
                                string path = AssetDatabase.GetAssetPath(prop.prefabSource);
                                if (!path.Contains("Resources"))
                                {
                                    EditorUtility.DisplayDialog("AutoStageBuilder Error", "You need to set up a prefab that belongs to the resource folder.", "Ok");
                                    prop.prefabSource = null;
                                    prop.prefabPath = null;
                                }
                                else if (prop.prefabPath == null || !prop.prefabPath.Equals(path))
                                {
                                    prop.prefabPath = path;

                                    //Since adding a component to a prefab can not delete it, you can temporarily generate gameobjects and obtain their sizes.
                                    var temp = Instantiate(prop.prefabSource);
                                    var box = temp.AddComponent<BoxCollider>();
                                    prop.size = box.size;
                                    DestroyImmediate(temp);
                                }
                            }
                            else
                            {
                                prop.prefabSource = null;
                                prop.prefabPath = null;
                            }

                            prop.enableMinimumLimit = EditorGUILayout.Toggle("Minimum Limit", prop.enableMinimumLimit);
                            if (prop.enableMinimumLimit)
                            {
                                prop.minimumCount = EditorGUILayout.IntField("Minumum Count", prop.minimumCount);
                            }

                            prop.enableMaximumLimit = EditorGUILayout.Toggle("Maximum Limit", prop.enableMaximumLimit);
                            if (prop.enableMaximumLimit)
                            {
                                prop.maximumCount = EditorGUILayout.IntField("Maximum Count", prop.maximumCount);
                            }
                        }
                    }

                    if (removepropdata != null)
                    {
                        data.propDatas.Remove(removepropdata);
                    }
                }
                EditorGUILayout.EndScrollView();

                using (new EditorGUILayout.VerticalScope())
                {
                    var totalminimumcount = 0;
                    var totalmaximumcount = 0;
                    foreach (var p in editStageData.mainPropCreationData.propDatas)
                    {
                        if (p.enableMaximumLimit) totalmaximumcount += p.maximumCount;
                        if (p.enableMinimumLimit) totalminimumcount += p.minimumCount;
                    }
                    EditorGUILayout.LabelField("Total Maximum Count : " + totalmaximumcount);
                    EditorGUILayout.LabelField("Total Minimum Count : " + totalminimumcount);
                }

            }
        }

        private bool Save()
        {
            if (!CheckStageDataCollect()) return false;

            if (dataSavePath == null || !File.Exists(dataSavePath))
            {
                dataSavePath = EditorUtility.SaveFilePanel("Save stage data", "Assets", stageDataName, "bytes");
            }

            if (!string.IsNullOrEmpty(dataSavePath))
            {
                editStageData.name = stageDataName = Path.GetFileNameWithoutExtension(dataSavePath);

                var json = JsonUtility.ToJson(editStageData);
                var fs = new FileStream(dataSavePath, FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite);
                var writer = new StreamWriter(fs);
                writer.WriteLine(json);
                writer.Flush();
                writer.Close();
                isDirty = false;
                return true;
            }

            return false;
        }

        private bool Load()
        {
            string initialpath = "Assets";
            if (dataSavePath != null) initialpath = dataSavePath;

            dataSavePath = EditorUtility.OpenFilePanel("Open stage data", initialpath, "bytes");

            if (!string.IsNullOrEmpty(dataSavePath))
            {
                var reader = new StreamReader(dataSavePath);
                var json = reader.ReadToEnd();
                editStageData = JsonUtility.FromJson<Runtime.StageData>(json);
                isDirty = false;
                stageDataName = Path.GetFileNameWithoutExtension(dataSavePath);
                reader.Close();

                foreach (var p in editStageData.mainPropCreationData.propDatas)
                {
                    p.prefabSource = AssetDatabase.LoadAssetAtPath<GameObject>(p.prefabPath);
                }

                return true;
            }

            return false;
        }

        private bool CheckStageDataCollect()
        {
            int totalminimumcount = 0;
            int totalmaximumcount = 0;
            bool isallmainpropenablemaximumlimit = true;

            //Main prop error check
            foreach(var mp in editStageData.mainPropCreationData.propDatas)
            {
                if (mp.enableMinimumLimit)  totalminimumcount += mp.minimumCount;
                if (mp.enableMaximumLimit)  totalmaximumcount += mp.maximumCount;
                if(isallmainpropenablemaximumlimit) isallmainpropenablemaximumlimit = mp.enableMaximumLimit;

                //Error if prefab source is null
                if (mp.prefabSource == null)
                {
                    EditorUtility.DisplayDialog("An error was found in the stage data",
                      "Prefab source has not been set. (Null found)",
                      "Ok");
                    return false;
                }

                //Error if the minimum value exceeds the maximum value
                if (mp.enableMinimumLimit && mp.enableMaximumLimit && mp.minimumCount > mp.maximumCount)
                {
                    EditorUtility.DisplayDialog("An error was found in the stage data",
                        mp.prefabSource.name + " : minimumCount must be less than maximumCount.", 
                        "Ok");
                    return false;
                }
            }

            //Error if the minimum number of creations exceeds the total number of creations.
            if (totalminimumcount > editStageData.mainPropCreationData.creationCount)
            {
                EditorUtility.DisplayDialog("An error was found in the stage data",
                     "Total minimum count must be less than or equal to main prop creation count", 
                     "Ok");
                return false;
            }

            /*
            //Error if the maximum number of creations exceeds the total number of creations.
            if (totalmaximumcount > editStageData.mainPropGenerateData.generateCount)
            {
                EditorUtility.DisplayDialog("An error was found in the stage data",
                    "Total maximum count must be less than or equal to main prop generate count",
                    "Ok");
                return false;
            }
            */

            //If the maximum creation limit is enabled for all MainProp, an error if the maximum creation count is less than the total creation count.
            if(isallmainpropenablemaximumlimit && totalmaximumcount < editStageData.mainPropCreationData.creationCount)
            {
                EditorUtility.DisplayDialog(
                    "An error was found in the stage data", 
                    "If the maximum limit flag is enabled for all MainProp, the total maximum count must be set to the main prop creation count or more.", 
                    "Ok");
                return false;
            }

            return true;
        }

        #endregion
    }

}
