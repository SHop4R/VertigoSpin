using DG.Tweening;
using UnityEngine;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.Managers
{
    public sealed class GameManager : MonoSingleton<GameManager>
    {
        private void Awake()
        {
            DOTween.SetTweensCapacity(5000, 500);
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}