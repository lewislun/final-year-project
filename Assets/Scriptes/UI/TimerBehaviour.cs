using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBehaviour : MonoBehaviour {

	#region Public Variables ---------------

	public float duration;

	#endregion

	public void StartTimer() {
		StartCoroutine(TimerCoroutine());
	}

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

	void changeTimeVisual(float timeRemain){
		GetComponent<Text>().text = secondToTimeStr(timeRemain);
	}

	IEnumerator TimerCoroutine() {
		float timeRemain = duration;
		while (timeRemain <= 0) {
			changeTimeVisual(timeRemain);
			yield return new WaitForFixedUpdate();
			timeRemain -= Time.deltaTime;
		}
		changeTimeVisual(timeRemain);
	}

}
