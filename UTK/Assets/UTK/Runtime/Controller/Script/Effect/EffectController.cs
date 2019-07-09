using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UTK.Runtime.Controller.Effect
{
    public class EffectController : MonoBehaviour
    {
        public bool isAutoDestroy = false;
        private List<ParticleSystem> particles;
        private List<Renderer> renderers;

        #region Property

        public bool IsPaused
        {
            get
            {
                return particles.Any(t => t.isPaused);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return particles.Any(t => t.isPlaying);
            }
        }

        public bool IsEmitting
        {
            get
            {
                return particles.Any(t => t.isEmitting);
            }
        }

        public bool IsAlive
        {
            get
            {
                return particles[0].IsAlive(true);
            }
        }

        #endregion


        /// <summary>
        ///  Start is called before the first frame update.
        /// </summary>
        void Start()
        {
            particles = GetComponentsInChildren<ParticleSystem>(true).ToList();
            renderers = GetComponentsInChildren<Renderer>(true).ToList();
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        void Update()
        {
            if (isAutoDestroy)
            {
                if (!IsAlive)
                {
                    Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// Pause effect.
        /// </summary>
        public void Pause()
        {
            particles[0].Pause(true);

            foreach (var r in renderers)
            {
                r.enabled = false;
            }
        }

        /// <summary>
        /// Resume effect.
        /// </summary>
        public void Resume()
        {
            particles[0].Play(true);

            foreach (var r in renderers)
            {
                r.enabled = true;
            }
        }
    }

}
