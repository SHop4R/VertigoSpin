using UnityEngine;
using UnityEngine.SceneManagement;

namespace VertigoSpin.Project.Scripts.Utils.Helpers
{
    public static class SceneHelper
    {
        public static bool NextSceneReady => AsyncOperation is { progress: >= 0.9f };
        public static AsyncOperation AsyncOperation{ get; private set; }
        
        private const int GameSceneIndex = 1;

        public static void LoadGameScene(bool allowSceneActivation = true) 
            => LoadScene(GameSceneIndex, allowSceneActivation);

        public static void LoadScene(int index, bool allowSceneActivation = true)
        {
            AsyncOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);

            if (AsyncOperation != null)
                AsyncOperation.allowSceneActivation = allowSceneActivation;
        }
        
        public static void ChangeScene()
        {
            AsyncOperation.allowSceneActivation = true;
            AsyncOperation = null;
        }
    }
}