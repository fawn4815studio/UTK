using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTK.Runtime.Controller.Wave
{
    public class WaveEventSender : MonoBehaviour
    {
        WaveController waveController;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        public void Initialize(WaveController controller)
        {
            waveController = controller;
        }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyOnDataDestroy()
        {
            Debug.Assert(waveController != null);
            waveController.OnDataDestory?.Invoke();
        }

        #region Internal

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion
    }

}
