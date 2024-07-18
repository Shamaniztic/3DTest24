using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Lucerna.UI;

namespace Lucerna.Utils
{
	public class SceneLoader : MonoBehaviour
	{
		// VARIABLES
		[Header("Settings")]
		[SerializeField] private string[] gameplayScenes;

		[Header("References")]
		[SerializeField] private CanvasGroupFader loadingScreenCanvas;
		[SerializeField] private Image loadingBarImage;

		private List<AsyncOperation> scenesToLoad = new();

		public bool IsLoading => scenesToLoad.Count > 0;

		public event EventHandler OnLoadingCompleted;

        // METHODS
        public void LoadGameplayScene(string sceneName, List<string> additionalScenes = null)
		{
            void loadGameplaySceneAction() => LoadGameplaySceneAction(sceneName + " (Static)", sceneName + " (Dynamic)", additionalScenes);
            StartCoroutine(LoadCoroutine(loadGameplaySceneAction));
        }

		public void LoadGameplayScene(string staticSceneName, string dynamicSceneName, List<string> additionalScenes = null)
		{
            void loadGameplaySceneAction() => LoadGameplaySceneAction(staticSceneName, dynamicSceneName, additionalScenes);
            StartCoroutine(LoadCoroutine(loadGameplaySceneAction));
        }

        public void LoadScene(string sceneName)
		{
            void loadSimpleSceneAction() => LoadSimpleSceneAction(sceneName);
            StartCoroutine(LoadCoroutine(loadSimpleSceneAction));
        }

        private IEnumerator LoadCoroutine(Action sceneLoadingAction)
        {
            foreach (var button in FindObjectsOfType<Button>())
            {
                button.interactable = false;
            }

            loadingScreenCanvas.FadeIn(0.5f);

            yield return new WaitForSecondsRealtime(0.6f);
            sceneLoadingAction?.Invoke();

            yield return StartCoroutine(LoadingScreenCoroutine());
            yield return new WaitForSecondsRealtime(0.5f);

            loadingScreenCanvas.FadeOut(0.5f);

            yield return new WaitForSecondsRealtime(0.5f);

            loadingBarImage.fillAmount = 0f;

            OnLoadingCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void LoadSimpleSceneAction(string sceneName, List<string> additionalScenes = null)
        {
            scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single));

            if (additionalScenes != null)
			{
                LoadAdditionalScenes(additionalScenes);
			}
        }

        private void LoadGameplaySceneAction(string staticSceneName, string dynamicSceneName, List<string> additionalScenes = null)
        {
            scenesToLoad.Add(SceneManager.LoadSceneAsync(staticSceneName, LoadSceneMode.Single));
            scenesToLoad.Add(SceneManager.LoadSceneAsync(dynamicSceneName, LoadSceneMode.Additive));

            foreach (var scene in gameplayScenes)
            {
                scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
            }

            if (additionalScenes != null)
            {
                LoadAdditionalScenes(additionalScenes);
            }
        }

        private void LoadAdditionalScenes(List<string> additionalScenes)
        {
            foreach (var additionalScene in additionalScenes)
            {
                scenesToLoad.Add(SceneManager.LoadSceneAsync(additionalScene, LoadSceneMode.Additive));
            }
        }

        private IEnumerator LoadingScreenCoroutine()
		{
			float totalProgress = 0f;
			foreach (var scene in scenesToLoad)
			{
				while(!scene.isDone)
				{
					totalProgress += scene.progress;
                    
                    if (loadingBarImage != null)
                    {
					    loadingBarImage.fillAmount = totalProgress / scenesToLoad.Count;
                    }

					yield return null;
				}
			}

			scenesToLoad = new List<AsyncOperation>();
		}
	}
}