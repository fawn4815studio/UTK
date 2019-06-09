using System;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.AutoStageBuilder.Runtime
{
    public enum MainPropCreationMode : int
    {
        Random,
        Direction
    }

    [Serializable]
    public class MainPropData
    {
        public bool enableMinimumLimit;
        public bool enableMaximumLimit;
        public int minimumCount;
        public int maximumCount;

        public Vector3 size;

        public string prefabPath;

        //[NonSerialized]
        public GameObject prefabSource; //Set from Assetdatabase using prefabPath.
    }

    [Serializable]
    public class MainPropCreationData
    {
        public int creationCount;
        public MainPropCreationMode mode;
        public Vector3 origin;
        public Vector3 direction;
        public float distanceBetweenProp;
        public bool whetherSureToConcatenateProps;
        public List<MainPropData> propDatas;

        public MainPropCreationData()
        {
            propDatas = new List<MainPropData>();
        }
    }

   
    [Serializable]
    public class StageData
    {
        public string name;
        public MainPropCreationData mainPropCreationData;

        [NonSerialized]
        public GameObject rootObject; //Root game object create at generate time.

        public StageData()
        {
            mainPropCreationData = new MainPropCreationData();
        }
    }
}

