using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCharacterBehaviour : MonoBehaviour {

	#region Public Variables -------------------

	public float moveDuration = 0.3f;
	public AnimationCurve moveCurve;

	#endregion

	#region Private Variables ------------------

	private Vector3 originalPos;
	private Coroutine ongingMovement = null;

	#endregion

	#region MonoBehaviour Functions ------------

	void Start() {
		originalPos = transform.position;
	}

	#endregion


	#region Merging ----------------------------

	public void MoveTo(Vector2 targetPos) {
		ongingMovement = StartCoroutine(MoveCoroutine(targetPos));
	}

	IEnumerator MoveCoroutine(Vector2 targetPos) {
		Vector2 startPos = transform.position;
		float timePassed = 0;

		while (timePassed < moveDuration) {
			timePassed += Time.deltaTime;
			Vector2 tempV2 = Vector2.Lerp(startPos, targetPos, moveCurve.Evaluate(timePassed / moveDuration));
			transform.position = new Vector3(tempV2.x, tempV2.y, originalPos.z);
			yield return new WaitForFixedUpdate();
		}

		transform.position = new Vector3(targetPos.x, targetPos.y, originalPos.z);
		ongingMovement = null;
	}

	#endregion

}
