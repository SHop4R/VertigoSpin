using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertigoSpin.Project.Scripts.Utils.Helpers;

namespace VertigoSpin.Project.Scripts.Animations
{
    public sealed class PanelAnimation : MonoBehaviour
    {
        [SerializeField, Tooltip("Sequence of panel elements to be animated")]
        private List<GameObject> panelElements = new();

        [Header("Animation")]
        [SerializeField] private float animationDelay = 0.3f;

        private void OnEnable() => StartCoroutine(Animate());

        private void OnDisable()
        {
            foreach (GameObject obj in panelElements)
            {
                obj.SetActive(false);
            }
        }
        
        private IEnumerator Animate()
        {
            foreach (GameObject obj in panelElements)
            {
                obj.SetActive(true);
                yield return WaitHelper.WaitForSeconds(animationDelay);
            }
        }
    }
}