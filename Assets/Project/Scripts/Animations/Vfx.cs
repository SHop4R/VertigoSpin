using System.Collections;
using UnityEngine;
using VertigoSpin.Project.Scripts.Utils.Helpers;

namespace VertigoSpin.Project.Scripts.Animations
{
    /// <summary>
    /// A sealed class that handles visual effects (VFX) using a particle system.
    /// </summary>
    public class Vfx : MonoBehaviour
    {
        public ParticleSystem Particle{ get; private set; }

        private Transform _parent;
        private Vector3 _startPosition;
        
        private void Awake()
        {
            Particle = GetComponent<ParticleSystem>();
            _parent = transform.parent;
            _startPosition = transform.localPosition;
        }
    
        /// <summary>
        /// Plays the VFX by detaching it from its parent, resetting its position, and starting the particle system.
        /// </summary>
        public void Play()
        {
            transform.SetParent(null);
            Particle.Play();

            StartCoroutine(ReturnToPlace());
        }

        /// <summary>
        /// Stops the VFX by stopping the particle system, reattaching it to its parent, and resetting its position.
        /// </summary>
        protected virtual void Stop()
        {
            Particle.Stop();

            if (!_parent)
            {
                Destroy(gameObject);
                return;
            }
            
            transform.SetParent(_parent);
            transform.localPosition = _startPosition;
        }
        
        private IEnumerator ReturnToPlace()
        {
            yield return WaitHelper.WaitForSeconds(Particle.main.duration);
            Stop();
        }
    }
}

