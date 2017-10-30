using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFadeBehaviour : MonoBehaviour {

	#region Public Variables ---------------------------------

	public float fadeDuration = 0.3f;
	public AnimationCurve fadeCurve;

	#endregion


	#region Private Variables --------------------------------

	Coroutine ongoingFade = null;

	#endregion


	#region Fade ---------------------------------------------

	public void Show(){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null){
			Debug.Log(gameObject.name + ".CanvasFadeBehaviour: no CanvasGroup attached");
			return;
		}

		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1;
	}

	public void Hide(){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null){
			Debug.Log(gameObject.name + ".CanvasFadeBehaviour: no CanvasGroup attached");
			return;
		}

		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0;
	}

	public void FadeIn(){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null){
			Debug.Log(gameObject.name + ".CanvasFadeBehaviour: no CanvasGroup attached");
			return;
		}

		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		if (ongoingFade != null)
			StopCoroutine(ongoingFade);
		ongoingFade = StartCoroutine(Fade(0,1));
	}

	public void FadeOut(){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null){
			Debug.Log(gameObject.name + ".CanvasFadeBehaviour: no CanvasGroup attached");
			return;
		}

		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		if (ongoingFade != null)
			StopCoroutine(ongoingFade);
		ongoingFade = StartCoroutine(Fade(1,0));
	}

	IEnumerator Fade(float startAlpha, float endAlpha){
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		float timePassed = 0;
		float alphaDiff = endAlpha - startAlpha;
		while (timePassed < fadeDuration){
			timePassed += Time.deltaTime;
			canvasGroup.alpha = alphaDiff * fadeCurve.Evaluate(timePassed / fadeDuration) + startAlpha;
			yield return new WaitForFixedUpdate();
		}
		
		canvasGroup.alpha = endAlpha;
		ongoingFade = null;
	}

	#endregion
}
