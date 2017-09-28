using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeAbilityBehaviour : AbilityBehaviour {


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
				Exchange();
			}
		}

	}

	#endregion


	void Exchange() {
		//secondTile.GetComponent<TileBehaviour>().

		TileBehaviour firstTileBehaviour = firstTile.GetComponent<TileBehaviour>();
		TileBehaviour secondTileBehaviour = secondTile.GetComponent<TileBehaviour>();

		firstTileBehaviour.isLinked = false;
		secondTileBehaviour.isLinked = false;

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
