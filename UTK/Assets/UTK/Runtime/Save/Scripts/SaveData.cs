using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTK.Runtime.Save
{
    [System.Serializable]
    public class SaveData<T>
    {
      
        public void Load(string filePath)
        {
            JsonUtility.FromJsonOverwrite(GetJsonString(filePath), this);
        }

        public string SaveToString()
        {
            return JsonUtility.ToJson(this);
        }

        public void SaveToDisc(string savePath)
        {
            using (var writer = new StreamWriter(savePath))
            {
                writer.Write(SaveToString());
            }
        }

        #region Static 

        public static bool IsExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }


        public static TJ CreateFromJSON<TJ>(string filePath) where TJ : SaveData<TJ>
        {
            return JsonUtility.FromJson<TJ>(GetJsonString(filePath));
        }

        private static string GetJsonString(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }

}
