using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UTK.Runtime.Save
{
    [System.Serializable]
    public class SaveData<T>
    {
        public void Load(string savedData)
        {
            JsonUtility.FromJsonOverwrite(savedData, this);
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

        public static TJ CreateFromJSON<TJ>(string filePath) where TJ : SaveData<TJ>
        {
            using (var reader = new StreamReader(filePath))
            {
                return JsonUtility.FromJson<TJ>(reader.ReadToEnd());
            }
        }
    }

}
