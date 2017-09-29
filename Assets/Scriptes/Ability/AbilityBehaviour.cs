using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour {

	#region Children Names -----------------------------------

	public static string ACTIVATING_VISUAL_EFFECTS = "Activating Visual Effects";

	#endregion


	#region Public Variables ---------------------------------

	public float cooldown = 10f;

	#endregion


	#region Properties ---------------------------------------

	private bool _isActivating = false;
	public bool isActivating {
		get {
			return _isActivating;
		}
		private set {
			transform.FindChild(ACTIVATING_VISUAL_EFFECTS).gameObject.SetActive(value);
			_isActivating = value;
		}
	}

	#endregion

	protected abstract void InitAbility();

	public virtual void Activate() {
		isActivating = true;
		InitAbility();

	}

	public virtual void Deactivate() {
		print("deacitivated");
		isActivating = false;
	}

}
