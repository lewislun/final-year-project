using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]

public class CanvasFadeBehaviour : MonoBehaviour {

	#region Public Variables ---------------------------------

	public float fadeDuration = 0.3f;
	public float showAlpha = 1f;
	public float hideAlpha = 0f;
	public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0,0,1,1);
	public UnityEvent onShown = new UnityEvent();
	public UnityEvent onHidden = new UnityEvent();

	#endregion


	#region Private Variables --------------------------------

	Coroutine ongoingFade = null;

	#endregion


	#region Fade ---------------------------------------------

	public void Show(bool animated){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

		if (animated){
			if (ongoingFade != null)
				StopCoroutine(ongoingFade);
			ongoingFade = StartCoroutine(Fade(showAlpha, onShown));
		}
		else
			canvasGroup.alpha = showAlpha;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

	public void Hide(bool animated){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

		if (animated){
			if (ongoingFade != null)
				StopCoroutine(ongoingFade);
			ongoingFade = StartCoroutine(Fade(hideAlpha, onHidden));
		}
		else
			canvasGroup.alpha = hideAlpha;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	IEnumerator Fade(float endAlpha, UnityEvent onFaded){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		float timePassed = 0;
		float startAlpha = canvasGroup.alpha;
		float alphaDiff = endAlpha - startAlpha;
		while (timePassed < fadeDuration){
			timePassed += Time.deltaTime;
			canvasGroup.alpha = alphaDiff * fadeCurve.Evaluate(timePassed / fadeDuration) + startAlpha;
			yield return new WaitForFixedUpdate();
		}
		
		canvasGroup.alpha = endAlpha;
		ongoingFade = null;
		if (onFaded != null)
			onFaded.Invoke();
	}

	#endregion
}
