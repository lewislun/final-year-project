using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainifyAbilityBehaviour : AbilityBehaviour {


	#region Public Variables ----------------------------------



	#endregion

	#region Private Variables ---------------------------------

	private TouchManager touchManager;

	#endregion


	#region MonoBehaviour -------------------------------------

	void Start() {
		touchManager = TouchManager.GetInstance();
	}

	void FixedUpdate() {
		if (!isActivating)
			return;

		GameObject touchingTile = TouchManager.GetTouchingTile();
		if (touchingTile == null)
			return;

		TileManager.GetInstance().TransformTile(touchingTile, "");
		Deactivate();
	}

	#endregion


	#region Ability Behaviour ---------------------------------

	protected override void InitAbility() {

	}

	public override void Activate() {
		base.Activate();
		touchManager.touchPriority = 1;
	}

	public override void Deactivate() {
		base.Deactivate();
		touchManager.touchPriority = 0;
	}

	#endregion
}
