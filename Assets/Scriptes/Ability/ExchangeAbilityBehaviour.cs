using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeAbilityBehaviour : AbilityBehaviour {

	#region Children Names -----------------------------------

	public static string VORTEX_PARTICLE = "Vortex Particle";

	#endregion


	#region Public Variables ---------------------------------

	public float animationDuration = 1f;
	public AnimationCurve diagonalPointCurve;
	public AnimationCurve animationCurve;
	public AnimationCurve darkenCurve;
	public Color selectedTileBorderColor;
	public GameObject darkenFilter;

	#endregion


	#region Properties ----------------------------------

	public override AbilityName abilityName {
		get {
			return AbilityName.Exchange;
		}
	}

	#endregion


	#region Private Variables --------------------------------

	private GameObject firstTile = null;
	private GameObject secondTile = null;
	private Color firstTileBorderColor;
	private Color secondTileBorderColor;

	private TouchManager touchManager;

	#endregion


	#region MonoBehaviour ------------------------------------

	void FixedUpdate() {
		if (!activating)
			return;

		GameObject touchingTile = TouchManager.GetTouchingTile();
		if (touchingTile == null)
			return;

		TileBehaviour tileBehaviour = touchingTile.GetComponent<TileBehaviour>();

		if (!tileBehaviour.isMerged) {
			if (firstTile == null) {
				firstTile = touchingTile;
				tileBehaviour.isLinked = true;
				firstTileBorderColor = tileBehaviour.glowingBorderColor;
				tileBehaviour.glowingBorderColor = selectedTileBorderColor;

			}
			else if (secondTile == null && touchingTile != firstTile) {
				secondTile = touchingTile;
				secondTile.GetComponent<TileBehaviour>().isLinked = true;
				secondTileBorderColor = tileBehaviour.glowingBorderColor;
				tileBehaviour.glowingBorderColor = selectedTileBorderColor;
				StartCoroutine(Exchange());
			}
		}

	}

	#endregion


	#region Visual Effect -------------------------------------

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

		if (darkenFilter != null)
			darkenFilter.SetActive(true);

		float timePassed = 0;
		while (timePassed < animationDuration) {
			timePassed += Time.deltaTime;

			float curveValue = animationCurve.Evaluate(timePassed / animationDuration);
			float diagonalCurveValue = diagonalPointCurve.Evaluate(timePassed / animationDuration);
			Vector3 tempPos1 = Vector2.Lerp(startPos1, startPos2, curveValue) + diagonalPoint1 * diagonalCurveValue;
			Vector3 tempPos2 = Vector2.Lerp(startPos2, startPos1, curveValue) + diagonalPoint2 * diagonalCurveValue;

			tempPos1.z = -5;
			tempPos2.z = -8;
			firstTile.transform.position = tempPos1;
			secondTile.transform.position = tempPos2;

			if (darkenFilter != null) {
				Color tempDarkenColor = darkenFilter.GetComponent<SpriteRenderer>().color;
				tempDarkenColor.a = darkenCurve.Evaluate(timePassed / animationDuration);
				darkenFilter.GetComponent<SpriteRenderer>().color = tempDarkenColor;
			}

			yield return new WaitForFixedUpdate();
		}

		if (darkenFilter != null)
			darkenFilter.SetActive(false);

		firstTileBehaviour.isLinked = false;
		secondTileBehaviour.isLinked = false;
		firstTileBehaviour.glowingBorderColor = firstTileBorderColor;
		secondTileBehaviour.glowingBorderColor = secondTileBorderColor;

		firstTile.transform.position = startPos2;
		secondTile.transform.position = startPos1;

		TileManager.GetInstance().ExchangeTiles(firstTileBehaviour.row, firstTileBehaviour.col, secondTileBehaviour.row, secondTileBehaviour.col);

		Deactivate(true);
	}

	void SetVortexParticleActive(bool active) {
		ParticleSystem.EmissionModule emission = transform.FindChild(VORTEX_PARTICLE).GetComponent<ParticleSystem>().emission;
		emission.enabled = active;
	}

	#endregion


	#region Ability Behaviour ---------------------------------

	protected override void InheritedStart(){
		touchManager = TouchManager.GetInstance();
	}

	protected override void InitAbility() {
		firstTile = null;
		secondTile = null;
	}

	protected override void AbilityActivated() {
		if (activating) { 
			SetVortexParticleActive(true);
			touchManager.touchPriority = 1;
		}
	}

	protected override void AbilityDeactivated() {
		SetVortexParticleActive(false);
		touchManager.touchPriority = 0;
	}

	#endregion
}
