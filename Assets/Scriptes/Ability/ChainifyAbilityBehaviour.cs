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
		Deactivate();
	}

	#endregion


	#region Ability Behaviour ---------------------------------

	protected override void InheritedStart(){
		touchManager = TouchManager.GetInstance();
	}

	protected override void InitAbility() {

	}

	public override void Activate() {
		base.Activate();

		if (activating) { 
			touchManager.touchPriority = 1;
		}
	}

	public override void Deactivate() {
		base.Deactivate();
		touchManager.touchPriority = 0;
		StartCooldown();
	}

	#endregion
}
