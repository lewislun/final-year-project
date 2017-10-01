using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbilityBehaviour : MonoBehaviour {

	#region Children Names -----------------------------------

	public static string ACTIVATING_VISUAL_EFFECTS = "Activating Visual Effects";
	public static string COOLDOWN_FILTER = "Cooldown Filter";

	#endregion


	#region Static Variabels ---------------------------------

	public static bool hasActivatingAbility = false;
	public static float cooldownYOffset = 0.35f;
	public static float cooldownAnimationDuration = 0.2f;

	#endregion


	#region Public Variables ---------------------------------

	public float cooldownDuration = 10f;

	#endregion


	#region Private Variables --------------------------------

	private Vector3 originalPos = Vector3.zero;
	private Coroutine cooldownAnimation = null;

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

	private bool _isCoolingDown = false;
	public bool isCoolingDown {
		get {
			return _isCoolingDown;
		}
		private set {
			_isCoolingDown = value;
		}
	}

	#endregion


	#region Ability Lifecycle --------------------------------

	protected abstract void InitAbility();

	public virtual void Activate() {
		if (hasActivatingAbility || isCoolingDown)
			return;
		isActivating = true;
		hasActivatingAbility = true;
		InitAbility();
	}

	public virtual void Deactivate() {
		print("deacitivated");
		isActivating = false;
		hasActivatingAbility = false;
	}

	#endregion


	#region Cooldown -----------------------------------------

	protected void StartCooldown() {
		if (cooldownAnimation != null)
			StopCoroutine(cooldownAnimation);
		cooldownAnimation = StartCoroutine(CooldownAnimation(true));
		StartCoroutine(CooldownCounter());
	}

	IEnumerator CooldownAnimation(bool isStartCooldown) {

		if (originalPos == Vector3.zero)
			originalPos = transform.position;

		float timePassed = 0;
		AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		Vector3 startPos = transform.position;
		Vector3 targetPos = new Vector3(
			originalPos.x, 
			isStartCooldown? (originalPos.y - cooldownYOffset): originalPos.y,
			originalPos.z
		);

		print(startPos);
		print(targetPos);
		
		while (timePassed < cooldownAnimationDuration) {
			yield return new WaitForFixedUpdate();
			timePassed += Time.deltaTime;
			transform.position = Vector3.Lerp(startPos, targetPos, curve.Evaluate(timePassed / cooldownAnimationDuration));
		}

		transform.position = targetPos;
		cooldownAnimation = null;
	}

	IEnumerator CooldownCounter() {
		isCoolingDown = true;
		float timePassed = 0;
		Image cooldownFilterImage = transform.FindChild(COOLDOWN_FILTER).GetComponent<Image>();

		while (timePassed < cooldownDuration) {
			timePassed += Time.deltaTime;
			cooldownFilterImage.fillAmount = 1 - timePassed / cooldownDuration;
			yield return new WaitForFixedUpdate();
		}

		cooldownFilterImage.fillAmount = 0;

		isCoolingDown = false;
		if (cooldownAnimation != null)
			StopCoroutine(cooldownAnimation);
		cooldownAnimation = StartCoroutine(CooldownAnimation(false));

	}

	#endregion
}
