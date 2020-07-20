using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UTK.Runtime.Save;

namespace UTK.Test
{
    [System.Serializable]
    public class TestSaveData : SaveData<TestSaveData>
    {
        [System.Serializable]
        public struct TestStruct
        {
            public int hp;
            public float damage;
            public string number;
        }

        public int testInt;
        public float testFloat;

        public List<TestStruct> testStructs = new List<TestStruct>();

        public TestSaveData()
        {
            testInt = 5;
            testFloat = 25.24f;

            testStructs.Add(new TestStruct()
            {
                hp = 3,
                damage = 12.5f,
                number = "AA"
            });

            testStructs.Add(new TestStruct()
            {
                hp = 4,
                damage = 13.5f,
                number = "BB"
            });

            testStructs.Add(new TestStruct()
            {
                hp = 5,
                damage = 14.5f,
                number = "CC"
            });
        }
    }

    public class SaveDataTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SaveAndLoadTestCo());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator SaveAndLoadTestCo()
        {

            var path = Path.Combine(Application.persistentDataPath, "TestData.asset");
            Debug.Log(path);
            var t = new TestSaveData();
            t.SaveToDisc(path);

            yield return new WaitForSeconds(2.0f);

            var lt = SaveData<TestSaveData>.CreateFromJSON<TestSaveData>(path);
            Debug.Log(lt.testFloat);
            Debug.Log(lt.testInt);

            foreach(var st in lt.testStructs)
            {
                Debug.Log(st.number);
                Debug.Log(st.hp);
                Debug.Log(st.damage);
            }
        }
    }

}
