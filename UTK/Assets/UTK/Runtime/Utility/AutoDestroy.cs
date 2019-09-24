using System.Collections;
using UnityEngine;

namespace UTK.Runtime.Utility
{
    /// <summary>
    /// Class that has a function to automatically delete object.
    /// </summary>
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField]
        private float waitTime = 1.0f;

        void Start()
        {
            StartCoroutine(DestroyCo());
        }

        IEnumerator DestroyCo()
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }
}

