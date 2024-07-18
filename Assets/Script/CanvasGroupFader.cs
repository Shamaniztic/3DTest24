using System.Collections;
using UnityEngine;

namespace Lucerna.UI
{
	public class CanvasGroupFader : MonoBehaviour 
	{
		// VARIABLES
		[SerializeField] private CanvasGroup group;

		private Coroutine currentFadeCoroutine;
	
		// METHODS
		public void FadeOut(float transitionDuration, bool disableOnEnd = true)
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			float initialValue = group.alpha;
			float finalValue = 0f;

			FadeTo(initialValue, finalValue, transitionDuration);
		}

		public void FadeIn(float transitionDuration)
		{
            gameObject.SetActive(true);

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            float initialValue = 0f;
            float finalValue = 1f;

            FadeTo(initialValue, finalValue, transitionDuration);
        }

		public void FadeTo(float initialValue, float finalValue, float transitionDuration)
		{
			if (currentFadeCoroutine != null)
			{
				StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = null;
			}

            currentFadeCoroutine = StartCoroutine(FadeCoroutine(initialValue, finalValue, transitionDuration));

        }

		private IEnumerator FadeCoroutine(float initialValue, float finalValue, float transitionDuration)
		{
			float t = 0f;

			while (t < 1f)
			{
				t += Time.unscaledDeltaTime * (1 / transitionDuration);
				group.alpha = Mathf.Lerp(initialValue, finalValue, t);
				yield return null;
			}

			group.alpha = finalValue;

			if (finalValue <= 0f)
			{
				gameObject.SetActive(false);
			}
		}
	}
}