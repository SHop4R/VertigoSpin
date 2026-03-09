using UnityEngine;
using VertigoSpin.Project.Scripts.Utils.Helpers;

namespace VertigoSpin.Project.Scripts.Utils.Loading.Steps
{
    public sealed class SceneLoadingStep : ILoadingStep
    {
        public float Progress{ get; private set; }
        public bool IsComplete{ get; private set; }

        private const float TargetProgress = 0.9f;

        public void Begin() => SceneHelper.LoadGameScene(false);

        public void Update()
        {
            if (IsComplete) return;

            AsyncOperation asyncOp = SceneHelper.AsyncOperation;
            if (asyncOp == null)
            {
                Progress = 0f;
                return;
            }

            Progress = asyncOp.progress / TargetProgress;
            Progress = Mathf.Clamp01(Progress);

            if (!SceneHelper.NextSceneReady) return;
            Progress = 1f;
            IsComplete = true;
        }
    }
}
