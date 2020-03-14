using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UTK.Tool.Common;

namespace UTK.Tool.AssetManager
{
    public class AssetImporter : EditorWindow
    {
        static readonly string CONFIGDIRECTORYPATH = "Assets/UTK/Config";
        static readonly string CONFIGFILEPATH = "Assets/UTK/Config/AssetImporterConfig.asset";
        static AssetImporter assetImporter;

        [SerializeField]
        AssetImporterConfig config;

        [MenuItem("UTK/AssetManager/Importer", false, 1)]
        static void Open()
        {
            if (assetImporter == null)
            {
                assetImporter = CreateInstance<AssetImporter>();
            }

            assetImporter.config = ToolUtility.GetOrCreateToolConfig<AssetImporterConfig>(CONFIGDIRECTORYPATH, CONFIGFILEPATH);

            assetImporter.minSize = new Vector2(500, 400);
            assetImporter.titleContent.text = "Asset Importer";
            assetImporter.ShowUtility();
        }

        #region Internal

        void OnGUI()
        {
          
        }

        #endregion
    }

}
