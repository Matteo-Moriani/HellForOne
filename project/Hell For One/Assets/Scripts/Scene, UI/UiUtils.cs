using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public static class UiUtils
    {
        public static void FadeIn(MonoBehaviour caller, CanvasGroup canvasGroup,float duration)
        {
            caller.StartCoroutine(Fade(canvasGroup, duration, canvasGroup.alpha, 1f));
        }

        public static void FadeOut(MonoBehaviour caller, CanvasGroup canvasGroup,float duration)
        {
            caller.StartCoroutine(Fade(canvasGroup, duration, canvasGroup.alpha, 0f));    
        }

        private static IEnumerator Fade(CanvasGroup canvasGroup, float duration, float startingAlpha, float endingAlpha)
        {
            float counter = 0f;

            while(counter < duration)
            {
                counter += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startingAlpha, endingAlpha, counter / duration);

                yield return null;
            }
        }
    }
}