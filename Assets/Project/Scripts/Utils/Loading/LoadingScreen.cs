using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VertigoSpin.Project.Scripts.Utils.Helpers;
using VertigoSpin.Project.Scripts.Utils.Loading.Steps;

namespace VertigoSpin.Project.Scripts.Utils.Loading
{
    public sealed class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image loadingBar;
        
        private CanvasGroup _loadingCanvasGroup;

        private List<ILoadingStep> _steps = new();
        
        private int _currentStepIndex;
        private float _startTime;
        private bool _isFinalizing;
        private Tween _fadeTween;

        private const float MinLoadTime = 2f;
        private const float FadeDuration = 0.5f;

        private void Awake()
        {
            _loadingCanvasGroup = GetComponent<CanvasGroup>();
            
            _loadingCanvasGroup.alpha = 1f;
            loadingBar.fillAmount = 0f;
            
            _steps = new()
            {
                new SceneLoadingStep()
            };
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _startTime = Time.time;
            _steps[0].Begin();
        }
        
        private void OnDestroy()
        {
            _fadeTween?.Kill();
            loadingBar?.DOKill();
        }

        private void FixedUpdate()
        {
            if (_isFinalizing) return;

            for (int i = _currentStepIndex; i < _steps.Count; i++)
            {
                _steps[i].Update();

                if (!_steps[i].IsComplete) break;

                if (i != _currentStepIndex) continue;
                _currentStepIndex++;
                    
                if (_currentStepIndex < _steps.Count)
                    _steps[_currentStepIndex].Begin();
            }

            float totalProgress = CalculateTotalProgress();
            loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, totalProgress, 0.15f);

            bool allStepsComplete = _currentStepIndex >= _steps.Count &&
                                    _steps[^1].IsComplete;
            
            float elapsedTime = Time.time - _startTime;
            bool minTimeMet = elapsedTime >= MinLoadTime;

            if (allStepsComplete && minTimeMet) 
                FinalizeLoading();
        }

        private float CalculateTotalProgress()
        {
            if (_steps.Count == 0) return 0f;

            float stepWeight = 1f / _steps.Count;
            float completedProgress = _currentStepIndex * stepWeight;

            if (_currentStepIndex < _steps.Count) 
                completedProgress += _steps[_currentStepIndex].Progress * stepWeight;

            return completedProgress;
        }

        private void FinalizeLoading()
        {
            _isFinalizing = true;
            loadingBar.DOKill();

            loadingBar.DOFillAmount(1f, FadeDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(ActivateSceneAndFadeOut);
        }

        private void ActivateSceneAndFadeOut()
        {
            SceneHelper.ChangeScene();
            FadeOutLoadingScreen();
        }

        private void FadeOutLoadingScreen()
        {
            _fadeTween = _loadingCanvasGroup.DOFade(0f, FadeDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete((() => Destroy(gameObject)));
        }
    }
}
