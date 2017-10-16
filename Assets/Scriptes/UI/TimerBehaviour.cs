using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBehaviour : MonoBehaviour {

	#region Public Variables ---------------

	public float duration;

	#endregion

	public void StartTimer() {
		StartCoroutine(TimerCoroutine());
	}

	void changeTimerVisual(float timePassed) {

	}

	IEnumerator TimerCoroutine() {
		float timePassed = 0;
		while (timePassed > duration) {
			changeTimerVisual(timePassed);
			yield return new WaitForFixedUpdate();
			timePassed += Time.deltaTime;

		}

	}

}
