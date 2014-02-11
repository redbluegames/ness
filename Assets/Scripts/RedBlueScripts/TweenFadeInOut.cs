using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Renderer))]
public class TweenFadeInOut : MonoBehaviour
{
	public float fadeTime = 0.5f;
	public bool isFadingOut { get; private set; }
	public bool isFadingIn { get; private set; }

	public void FadeOut ()
	{
		FadeOut (fadeTime);
	}

	public void FadeOut (float time)
	{
		// If fading in, stop it and fade out, if already fading out, do nothing
		if (isFadingOut) {
			return;
		}
		if (isFadingIn) {
			StopCoroutine ("Fade");
			isFadingIn = false;
		}
		isFadingOut = true;
		fadeTime = time;
		StartCoroutine ("Fade", 0);
	}

	public void FadeIn ()
	{
		FadeIn (fadeTime);
	}

	public void FadeIn (float time)
	{
		// If fading out, stop it and fade in, if already fading in, do nothing
		if (isFadingIn) {
			return;
		}
		if (isFadingOut) {
			StopCoroutine ("Fade");
			isFadingOut = false;
		}
		isFadingIn = true;
		fadeTime = time;
		StartCoroutine ("Fade", 1);
	}

	IEnumerator Fade (float alphaValue)
	{
		Color currentColor = gameObject.renderer.material.color;
		Color targetColor = gameObject.renderer.material.color;
		targetColor.a = alphaValue;

		// Fast-forward if the fade is midway through
		float currentFadeProgress = Mathf.Abs (alphaValue - currentColor.a);
		float elapsed = Mathf.Max (0, fadeTime - (currentFadeProgress * fadeTime));
		while (elapsed < fadeTime) {
			elapsed += Time.deltaTime;
			gameObject.renderer.material.color = Color.Lerp (currentColor, targetColor, elapsed / fadeTime);
			yield return null;
		}
	}

}
