using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTK.Tool.AssetManager
{
    public class AssetManagerEditorCommands
    {
        [MenuItem("UTK/AssetManager/Commands/Set bundle names to resources assets", false, 1)]
        static void SetBundleNamesToResourcesAssets()
        {
            var resourcesfolders = System.IO.Directory.GetDirectories(Application.dataPath, "Resources", System.IO.SearchOption.AllDirectories);
            SetBundleNamesToFolderAssets(resourcesfolders);
        }

        [MenuItem("UTK/AssetManager/Commands/Remove bundle names to resources assets", false, 1)]
        static void RemoveBundleNamesToResourcesAssets()
        {
            var resourcesfolders = System.IO.Directory.GetDirectories(Application.dataPath, "Resources", System.IO.SearchOption.AllDirectories);
            SetBundleNamesToFolderAssets(resourcesfolders, true);
        }

        #region Internal

        static void SetBundleNamesToFolderAssets(string[] folders, bool remove = false)
        {
            Uri baseuri = new Uri(Application.dataPath);

            foreach (var f in folders)
            {
                var assets = new System.IO.DirectoryInfo(f).GetFiles("*", System.IO.SearchOption.AllDirectories);
                foreach (var a in assets)
                {
                    if (a.Extension.Equals(".meta") == false)
                    {
                        Uri relativeuri = baseuri.MakeRelativeUri(new Uri(a.FullName));
                        AssetImporter importer = AssetImporter.GetAtPath(relativeuri.OriginalString); //Get the relative path for searching AssetImporter.
                        if (importer != null)
                        {
                            importer.assetBundleName = remove ? string.Empty : System.IO.Path.GetFileNameWithoutExtension(a.Name) + ".ab";
                        }
                        else
                        {
                            Debug.LogWarning(string.Format("Invalid asset name found : {0}", relativeuri.OriginalString));
                        }

                    }
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        #endregion
    }
}

