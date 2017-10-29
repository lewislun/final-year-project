using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbilityBehaviour : MonoBehaviour {

	public enum AbilityName {
		Chainify,
		Exchange
	}


	#region Children Names -----------------------------------

	public static string ACTIVATING_VISUAL_EFFECTS = "Activating Visual Effects";
	public static string COOLDOWN_FILTER = "Cooldown Filter";

	#endregion


	#region Static Variables ---------------------------------

	public static Dictionary<AbilityName, AbilityBehaviour> abilities = new Dictionary<AbilityName, AbilityBehaviour>();
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
	private Coroutine cooldownCounter = null;

	#endregion


	#region Properties ---------------------------------------

	public abstract AbilityName abilityName{ get; }

	private bool _activating = false;
	public bool activating {
		get {
			return _activating;
		}
		private set {
			transform.FindChild(ACTIVATING_VISUAL_EFFECTS).gameObject.SetActive(value);
			_activating = value;
		}
	}

	private bool _coolingDown = false;
	public bool coolingDown {
		get {
			return _coolingDown;
		}
		private set {
			_coolingDown = value;
		}
	}

	public bool abilityEnabled {
		set {
			CanvasGroup cg = GetComponent<CanvasGroup>();
			cg.alpha = value? 1: 0;
			cg.interactable = value;
			cg.blocksRaycasts = value;
		}
		get {
			return GetComponent<CanvasGroup>().interactable;
		}
	}

	#endregion


	#region MonoBehaviour Functions --------------------------

	protected void Awake(){
		abilities[abilityName] = this;
	}

	protected void Start(){
		InheritedStart();
	}

	void OnDisable(){
		StopCooldown(false);
	}

	#endregion


	#region Ability Lifecycle --------------------------------

	protected abstract void InheritedStart();

	protected abstract void InitAbility();

	public virtual void Activate() {
		if (hasActivatingAbility || coolingDown)
			return;
		activating = true;
		hasActivatingAbility = true;
		InitAbility();
	}

	public virtual void Deactivate() {
		print("deacitivated");
		activating = false;
		hasActivatingAbility = false;
	}

	#endregion


	#region Abilities Control --------------------------------

	public static void StopAllCooldown(){
		foreach(KeyValuePair<AbilityName, AbilityBehaviour> ability in abilities)
			ability.Value.StopCooldown(true);
	}

	public static void SetAbilityEnabled(AbilityName abilityName, bool enabled){
		if (!abilities.ContainsKey(abilityName)){
			Debug.Log("ability " + abilityName + " does not exist");
			return;
		}

		abilities[abilityName].abilityEnabled = enabled;
	}

	#endregion


	#region Cooldown -----------------------------------------

	protected void StartCooldown() {
		if (cooldownAnimation != null)
			StopCoroutine(cooldownAnimation);
		cooldownAnimation = StartCoroutine(CooldownAnimation(true));
		cooldownCounter = StartCoroutine(CooldownCounter());
	}

	public void StopCooldown(bool hasAnimation){
		if (!coolingDown)
			return;

		if (cooldownCounter != null){
			StopCoroutine(cooldownCounter);
			cooldownCounter = null;
		}
		if (cooldownAnimation != null){
			StopCoroutine(cooldownAnimation);
			cooldownAnimation = null;
		}

		if (hasAnimation)
			cooldownAnimation = StartCoroutine(CooldownAnimation(false));
		else
			transform.position = originalPos;

		coolingDown = false;
		transform.FindChild(COOLDOWN_FILTER).GetComponent<Image>().fillAmount = 0;
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
		
		while (timePassed < cooldownAnimationDuration) {
			yield return new WaitForFixedUpdate();
			timePassed += Time.deltaTime;
			transform.position = Vector3.Lerp(startPos, targetPos, curve.Evaluate(timePassed / cooldownAnimationDuration));
		}

		transform.position = targetPos;
		cooldownAnimation = null;
	}

	IEnumerator CooldownCounter() {
		coolingDown = true;
		float timePassed = 0;
		Image cooldownFilterImage = transform.FindChild(COOLDOWN_FILTER).GetComponent<Image>();

		while (timePassed < cooldownDuration) {
			timePassed += Time.deltaTime;
			cooldownFilterImage.fillAmount = 1 - timePassed / cooldownDuration;
			yield return new WaitForFixedUpdate();
		}

		cooldownFilterImage.fillAmount = 0;

		coolingDown = false;
		if (cooldownAnimation != null)
			StopCoroutine(cooldownAnimation);
		cooldownAnimation = StartCoroutine(CooldownAnimation(false));
		cooldownCounter = null;
	}

	#endregion
}
