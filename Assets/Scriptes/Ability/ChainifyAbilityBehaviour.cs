using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainifyAbilityBehaviour : AbilityBehaviour {


	#region Properties ----------------------------------

	public override AbilityName abilityName {
		get {
			return AbilityName.Chainify;
		}
	}

	#endregion

	#region Private Variables ---------------------------------

	private TouchManager touchManager;

	#endregion


	#region MonoBehaviour -------------------------------------

	void FixedUpdate() {
		if (!activating)
			return;

		GameObject touchingTile = TouchManager.GetTouchingTile();
		if (touchingTile == null)
			return;

		TileManager.GetInstance().TransformTile(touchingTile, "");
		Deactivate(true);
	}

	#endregion


	#region Ability Behaviour ---------------------------------

	protected override void InheritedStart(){
		touchManager = TouchManager.GetInstance();
	}

	protected override void InitAbility() {

	}

	protected override void AbilityActivated() {
		touchManager.touchPriority = 1;
	}

	protected override void AbilityDeactivated() {
		touchManager.touchPriority = 0;
	}

	#endregion
}
