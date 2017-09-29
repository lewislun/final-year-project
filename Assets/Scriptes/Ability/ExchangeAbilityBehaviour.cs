using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeAbilityBehaviour : AbilityBehaviour {


	#region Public Variables ---------------------------------

	public float animationDuration = 1f;
	public AnimationCurve diagonalPointCurve;
	public AnimationCurve animationCurve;

	#endregion

	#region Private Variables --------------------------------

	private GameObject firstTile = null;
	private GameObject secondTile = null;

	private TouchManager touchManager;

	#endregion


	#region MonoBehaviour ------------------------------------

	void Start() {
		touchManager = TouchManager.GetInstance();
	}

	void FixedUpdate() {
		if (!isActivating)
			return;

		GameObject touchingTile = TouchManager.GetTouchingTile();

		if (touchingTile != null && !touchingTile.GetComponent<TileBehaviour>().isMerged) {
			if (firstTile == null) {
				firstTile = touchingTile;
				firstTile.GetComponent<TileBehaviour>().isLinked = true;
			}
			else if (secondTile == null && touchingTile != firstTile) {
				secondTile = touchingTile;
				secondTile.GetComponent<TileBehaviour>().isLinked = true;
				StartCoroutine(Exchange());
			}
		}

	}

	#endregion


	IEnumerator Exchange() {

		TileBehaviour firstTileBehaviour = firstTile.GetComponent<TileBehaviour>();
		TileBehaviour secondTileBehaviour = secondTile.GetComponent<TileBehaviour>();

		Vector3 startPos1 = firstTile.transform.position;
		Vector3 startPos2 = secondTile.transform.position;

		Vector2 centerPos = (startPos1 + startPos2) / 2;
		Vector2 relativePos1 = new Vector2(startPos1.x, startPos1.y) - centerPos;
		Vector2 relativePos2 = new Vector2(startPos2.x, startPos2.y) - centerPos;

		Vector2 diagonalPoint1 = new Vector2(relativePos1.y, relativePos2.x);
		Vector2 diagonalPoint2 = new Vector2(relativePos2.y, relativePos1.x);

		float timePassed = 0;
		while (timePassed < animationDuration) {
			timePassed += Time.deltaTime;

			Vector3 tempPos1 = Vector2.Lerp(startPos1, startPos2, animationCurve.Evaluate(timePassed / animationDuration)) + diagonalPoint1 * diagonalPointCurve.Evaluate(timePassed / animationDuration);
			Vector3 tempPos2 = Vector2.Lerp(startPos2, startPos1, animationCurve.Evaluate(timePassed / animationDuration)) + diagonalPoint2 * diagonalPointCurve.Evaluate(timePassed / animationDuration);

			tempPos1.z = -5;
			tempPos2.z = -5;

			firstTile.transform.position = tempPos1;
			secondTile.transform.position = tempPos2;

			yield return new WaitForFixedUpdate();
		}


		firstTileBehaviour.isLinked = false;
		secondTileBehaviour.isLinked = false;

		firstTile.transform.position = startPos2;
		secondTile.transform.position = startPos1;

		TileManager.GetInstance().ExchangeTiles(firstTileBehaviour.row, firstTileBehaviour.col, secondTileBehaviour.row, secondTileBehaviour.col);

		touchManager.touchPriority = 0;
		Deactivate();
	}


	protected override void InitAbility() {
		firstTile = null;
		secondTile = null;
	}

	public override void Activate() {

		base.Activate();


		
		//todo: darken all UI

		touchManager.touchPriority = 1;
	}

}
