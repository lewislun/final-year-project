using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCharacterBehaviour : MonoBehaviour {

	#region Public Variables -------------------

	public float moveDuration = 0.3f;
	public AnimationCurve moveCurve;

	#endregion

	#region Private Variables ------------------

	private Vector3 originalAnchoredPos;
	private Coroutine ongingMovement = null;
	private float originalZ;

	#endregion

	#region MonoBehaviour Functions ------------

	void Start() {
		originalZ = transform.position.z;
		originalAnchoredPos = GetComponent<RectTransform>().anchoredPosition;
	}

	#endregion


	#region Merging ----------------------------

	public void MoveTo(Vector2 targetPos) {
		ongingMovement = StartCoroutine(MoveCoroutine(targetPos));
	}

	public void MoveToOriginalPos() {
		ongingMovement = StartCoroutine(MoveToOriginalPosCoroutine());
	}

	IEnumerator MoveCoroutine(Vector2 targetPos) {
		Vector2 startPos = transform.position;
		float timePassed = 0;

		while (timePassed < moveDuration) {
			timePassed += Time.deltaTime;
			Vector2 tempV2 = Vector2.Lerp(startPos, targetPos, moveCurve.Evaluate(timePassed / moveDuration));
			transform.position = new Vector3(tempV2.x, tempV2.y, originalZ);
			yield return new WaitForFixedUpdate();
		}

		transform.position = new Vector3(targetPos.x, targetPos.y, originalZ);
		ongingMovement = null;
	}

	IEnumerator MoveToOriginalPosCoroutine() {

		RectTransform rectTransform = GetComponent<RectTransform>();

		Vector2 startPos = rectTransform.anchoredPosition;
		float timePassed = 0;

		while (timePassed < moveDuration) {
			timePassed += Time.deltaTime;
			Vector2 tempV2 = Vector2.Lerp(startPos, originalAnchoredPos, moveCurve.Evaluate(timePassed / moveDuration));
			rectTransform.anchoredPosition = new Vector3(tempV2.x, tempV2.y, originalZ);
			yield return new WaitForFixedUpdate();
		}

		rectTransform.anchoredPosition = new Vector3(originalAnchoredPos.x, originalAnchoredPos.y, originalZ);
		ongingMovement = null;
	}

	#endregion

}
