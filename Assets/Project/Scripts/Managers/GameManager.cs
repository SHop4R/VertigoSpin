using DG.Tweening;
using UnityEngine;
using VertigoSpin.Project.Scripts.Utils;

namespace VertigoSpin.Project.Scripts.Managers
{
    /// <summary>
    /// A sealed class that manages the game state and handles gameplay events.
    /// </summary>
    /// <remarks>
    /// This class is a singleton, inheriting from <see cref="MonoSingleton{T}"/> to ensure only one instance exists.
    /// </remarks>
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