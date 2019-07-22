using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UTK.Runtime.Controller.Effect
{
    /// <summary>
    /// Particle controller with the function to process particle system attached to GameObject efficiently.
    /// </summary>
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
                return particles.Any(t => t.IsAlive(true));
            }
        }

        #endregion


        void Start()
        {
            particles = GetComponentsInChildren<ParticleSystem>(true).ToList();
            renderers = GetComponentsInChildren<Renderer>(true).ToList();
        }

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

        public void Pause()
        {
            foreach (var p in particles)
            {
                p.Pause(true);
            }

            foreach (var r in renderers)
            {
                r.enabled = false;
            }
        }

        public void Resume()
        {
            foreach (var p in particles)
            {
                p.Play(true);
            }

            foreach (var r in renderers)
            {
                r.enabled = true;
            }
        }
    }

}
