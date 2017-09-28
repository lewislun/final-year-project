using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour {

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
