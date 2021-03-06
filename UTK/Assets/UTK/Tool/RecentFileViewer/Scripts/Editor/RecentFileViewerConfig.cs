﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace UTK.Tool.RecentFileViewer
{
    [System.Serializable]
    public class RecentOpenFileData
    {
        [SerializeField]
        string name;

        [SerializeField]
        string path;

        [SerializeField]
        string id;

        public string Id { get => id; }
        public string Name { get => name; }
        public string Path { get => path; }

        public RecentOpenFileData(string name, string path)
        {
            id = AssetDatabase.AssetPathToGUID(path);
            this.name = name;
            this.path = path;
        }
    }

    public class RecentFileViewerConfig : ScriptableObject
    {
        [SerializeField]
        int queueLimit = 5;

        [SerializeField]
        List<RecentOpenFileData> recentSceneList = new List<RecentOpenFileData>();

        [SerializeField]
        List<RecentOpenFileData> recentPrefabList = new List<RecentOpenFileData>();

        [SerializeField]
        List<RecentOpenFileData> recentMaterialList = new List<RecentOpenFileData>();

        [SerializeField]
        List<RecentOpenFileData> recentTextureList = new List<RecentOpenFileData>();

        [SerializeField]
        List<RecentOpenFileData> recentScriptList = new List<RecentOpenFileData>();

        public List<RecentOpenFileData> RecentSceneList { get => recentSceneList; }
        public List<RecentOpenFileData> RecentPrefabList { get => recentPrefabList; }
        public List<RecentOpenFileData> RecentMaterialList { get => recentMaterialList; }
        public List<RecentOpenFileData> RecentTextureList { get => recentTextureList;}
        public List<RecentOpenFileData> RecentScriptList { get => recentScriptList;}
        public int QueueLimit { get => queueLimit; set => queueLimit = value; }
   
    }
}

