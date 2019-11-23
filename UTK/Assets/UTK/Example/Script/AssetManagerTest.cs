using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTK.Runtime.Manager;

namespace UTK.Test
{
    public class AssetManagerTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            AssetManager.Instance.LoadAsync<GameObject>("RedCube", (GameObject ob) =>
            {
                Instantiate(ob, Vector3.zero, Quaternion.identity);
                AssetManager.Instance.UnloadAll();
            });

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
