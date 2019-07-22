using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UTK.Runtime.AutoStageBuilder
{
    public class RuntimeAutoStageBuilderManager : SingletonBase<RuntimeAutoStageBuilderManager>
    {
        /// <summary>
        /// Temporary main prop data to hold information at generation time
        /// </summary>
        private class TempMainPropData
        {
            public MainPropData Data { get; private set; }
            public int Hash { get; private set; }
            public int GenerateCount { get; set; }

            public TempMainPropData(MainPropData data)
            {
                Data = data;
                Hash = data.prefabPath.GetHashCode();
                GenerateCount = 0;
            }
        }

        /// <summary>
        /// List of stage data that has been generated.
        /// </summary>
        private List<StageData> stageDatas = new List<StageData>();

        /// <summary>
        /// List of main prop object(prefab) that has been generated. (Clear when the stage data generation is finished)
        /// </summary>
        private List<GameObject> generateMainProps = new List<GameObject>();

        /// <summary>
        /// Most recently generated main prop data. (Set null when the stage data generation is finished)
        /// </summary>
        private TempMainPropData previousUsingTempMainPropData = null;

        /// <summary>
        /// Count number to generate , data to generate
        /// </summary>
        private Dictionary<int, TempMainPropData> minimumLimitMainPropGenerateTimingDic = new Dictionary<int, TempMainPropData>();

        public void GenerateStageSync(string path)
        {
            var data = Load(path);
            if (data != null)
            {
                Debug.LogError(string.Format("Failed load stage data. [ {0} ]", path));
                return;
            }

            InternalGenerateStageSync(data);
        }
        public void GenerateStageSync(StageData data)
        {
            InternalGenerateStageSync(data);
        }

        void Start()
        {
            name = "RuntimeAutoStageBuilderManager";
        }

        void Update()
        {

        }

        StageData Load(string path)
        {
            var reader = new StreamReader(path);
            var json = reader.ReadToEnd();
            return JsonUtility.FromJson<StageData>(json);
        }

        void InternalGenerateStageSync(StageData data)
        {
            //Create root object.
            data.rootObject = new GameObject(data.name);
            data.rootObject.transform.position = data.mainPropCreationData.origin;
            data.rootObject.transform.rotation = Quaternion.identity;

            switch (data.mainPropCreationData.mode)
            {
                case MainPropCreationMode.Direction:
                    GenerateMainProp_DirectionMode(data);
                    break;
                case MainPropCreationMode.Random:
                    GenerateMainProp_RandomMode(data);
                    break;
            }

            stageDatas.Add(data);
        }

        void GenerateMainProp_DirectionMode(StageData data)
        {
            var temppropdatas = new List<TempMainPropData>();
            var mainpropscount = data.mainPropCreationData.propDatas.Count;

            foreach (var d in data.mainPropCreationData.propDatas)
            {
                temppropdatas.Add(new TempMainPropData(d));
            }
            temppropdatas.TrimExcess();

            //Set minimum limit main prop generate timing
            foreach (var td in temppropdatas)
            {
                int count = 0;

                if (!td.Data.enableMinimumLimit) continue;

                while (true)
                {
                    var generateindex = UnityEngine.Random.Range(0, data.mainPropCreationData.creationCount);
                    if (generateindex <= data.mainPropCreationData.creationCount && !minimumLimitMainPropGenerateTimingDic.ContainsKey(generateindex))
                    {
                        minimumLimitMainPropGenerateTimingDic[generateindex] = td;
                        ++count;
                    }

                    if (count == td.Data.minimumCount)
                    {
                        break;
                    }
                }
            }

            //Start generate
            for (int i = 0; i < data.mainPropCreationData.creationCount; i++)
            {
                TempMainPropData td = null;
                bool isminimumcreate = false;

                if (minimumLimitMainPropGenerateTimingDic.ContainsKey(i))
                {
                    var md = minimumLimitMainPropGenerateTimingDic[i];

                    if (md.Data.enableMaximumLimit && md.GenerateCount + 1 > md.Data.maximumCount)
                    {
                        isminimumcreate = false;
                    }
                    else
                    {
                        td = md;
                        isminimumcreate = true;
                    }
                }

                if (!isminimumcreate)
                {
                    while (true)
                    {
                        bool ok = true;

                        var mainproprandomindex = UnityEngine.Random.Range(0, mainpropscount);
                        if (mainproprandomindex == mainpropscount) mainproprandomindex--;
                        td = temppropdatas[mainproprandomindex];

                        //Maximum limit check
                        if (td.Data.enableMaximumLimit && td.GenerateCount + 1 > td.Data.maximumCount)
                        {
                            ok = false;
                        }

                        if (ok) break;
                    }
                }

                GameObject mainprop = null;

                //Is first generate
                if (generateMainProps.Count == 0)
                {
                    mainprop = Instantiate(td.Data.prefabSource, data.mainPropCreationData.origin, Quaternion.identity);
                    mainprop.transform.parent = data.rootObject.transform;
                }
                else
                {
                    var previousprop = generateMainProps.Last();
                    var direction = data.mainPropCreationData.direction.normalized;

                    mainprop = Instantiate(td.Data.prefabSource, previousprop.transform.position, Quaternion.identity);
                    mainprop.transform.parent = data.rootObject.transform;

                    var newpos = direction * data.mainPropCreationData.distanceBetweenProp;
                    mainprop.transform.position += newpos;
                }

                td.GenerateCount++;

                generateMainProps.Add(mainprop);
                previousUsingTempMainPropData = td;
            }

        }
        void GenerateMainProp_RandomMode(StageData data)
        {

        }
    }

}
