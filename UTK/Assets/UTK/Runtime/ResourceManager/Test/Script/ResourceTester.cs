using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Manager.Test
{
    public class ResourceTester : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var resource = ResourceManager.Instance.LoadSync<GameObject>("CapsuleTestResource");
            var ob = Instantiate(resource, Vector3.zero, Quaternion.identity);

            ResourceManager.Instance.LoadAsync<GameObject>("CubeTestResource", (GameObject prefab) =>
             {
                 Instantiate(prefab, new Vector3(0, 5, 0), Quaternion.identity);
             }
            );

            ResourceManager.Instance.LoadAsync<Material>("TestResourceMat", (Material mat) =>
            {
                ob.GetComponent<Renderer>().material = mat;
            }
            );

            ResourceManager.Instance.UnloadAll();
        }
    }

}
