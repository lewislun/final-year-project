using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]

public class TimerBehaviour : MonoBehaviour {

	#region Public Variables ---------------

	public Color startColor = Color.white;
	public Color endColor = Color.white;
	public AnimationCurve colorCurve = AnimationCurve.Linear(0,0,1,1);
	public UnityEvent onTimeUp = new UnityEvent();

	#endregion


	#region Private Variables -------------

	Coroutine runningTimer = null;
	Image mImage;

	#endregion


	#region Properties --------------------

	private float _duration = -1f;
	public float duration {
		get {
			return _duration;
		}
		set {
			_duration = value;
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
			if (value < 0){
				canvasGroup.alpha = 0;
			} else {
				canvasGroup.alpha = 1;
			}
		}
	}

	#endregion


	#region MonoBehaviour -----------------

	void Awake(){
		mImage = GetComponent<Image>();
	}

	#endregion


	#region Timer Control -----------------

	public void StartTimer() {
		Debug.Log(duration);
		StopTimer();
		if (duration >= 0)
			runningTimer = StartCoroutine(TimerCoroutine());
	}

	public void StopTimer(){
		if (runningTimer != null)
			StopCoroutine(runningTimer);
	}

	#endregion


	#region Visual ------------------------

	void ChangeTimerLineScaleX(float timeRemain){
		Vector3 tempV3 = transform.localScale;
		tempV3.x = timeRemain/duration;
		transform.localScale = tempV3;
	}

	void ChangeTimerLineColor(float timeRemain){
		mImage.color = Color.Lerp(startColor, endColor, colorCurve.Evaluate(1 - (timeRemain / duration)));
	}

	void ChangeTimeVisual(float timeRemain){
		ChangeTimerLineScaleX(timeRemain);
		ChangeTimerLineColor(timeRemain);
	}

	IEnumerator TimerCoroutine() {
		float timeRemain = duration;
		while (timeRemain > 0) {
			ChangeTimeVisual(timeRemain);
			yield return new WaitForFixedUpdate();
			timeRemain -= Time.deltaTime;
		}
		ChangeTimeVisual(timeRemain);
		runningTimer = null;
		if (onTimeUp != null)
			onTimeUp.Invoke();
	}

	#endregion


	#region utils ------------------------

	string secondToTimeStr(float time) {
		int minute = (int)(time / 60);
		float second = time % 60;
		string str = "";

		if (minute < 10)
			str += "0";
		str += minute + ":";
		str += second.ToString("00.00");

		return str;
	}

	#endregion

}
