using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour {

	#region Children Names ---------------------------------------

	public static string LINKED_GROWING_BORDER = "Linked Growing Border";
	public static string CHARACTER = "Character";
	public static string TILE_BACKGROUND = "Tile Background";

	#endregion

	#region Public Variables ------------------------------------

	public float dropAcceleration;
	public int dropSection;

	#endregion


	#region Private Variable ------------------------------------

	private TileManager tileManager;
	private Vector2 currentSpeed = Vector2.zero;
	private Vector2 expectedPos;

	GameObject tileBackground;

	TileBackgroundBehaviour mTileBackgroundBehaviour;
	RectTransform mRectTransform;
	Animator mAnimator;

	#endregion


	#region Public Properties -----------------------------------

	Vector2 _mergedCenterPos = new Vector2(0, 0);
	public Vector2 mergedCenterPos {
		get {
			return _mergedCenterPos;
		}
		private set {
			_mergedCenterPos = value;
		}
	}

	bool _isLinked = false;
	public bool isLinked {
		get {
			return _isLinked;
		}
		set {
			transform.FindChild(LINKED_GROWING_BORDER).gameObject.SetActive(value);
			_isLinked = value;
		}
	}

	bool _isMerged = false;
	public bool isMerged {
		get {
			return _isMerged;
		}
		private set {
			_isMerged = value;
		}
	}

	string _character = "R";
	public string character {
		get {
			return _character;
		}
		set {
			_character = value;
		}
	}

	int _row;
	public int row {
		get {
			return _row;
		}
		set {
			_row = value;
			expectedPos = transform.parent.GetComponent<TileManager>().GetTilePos(row, col);
		}
	}

	int _col;
	public int col {
		get {
			return _col;
		}
		set {
			_col = value;
			expectedPos = transform.parent.GetComponent<TileManager>().GetTilePos(row, col);
		}
	}

	public Color backgroundColor {
		get {
			if (tileBackground == null)
				tileBackground = transform.FindChild(TILE_BACKGROUND).gameObject;
			return tileBackground.GetComponent<TileBackgroundBehaviour>().backgroundColor;
		}
		set {
			if (tileBackground == null)
				tileBackground = transform.FindChild(TILE_BACKGROUND).gameObject;
			tileBackground.GetComponent<TileBackgroundBehaviour>().backgroundColor = value;
		}
	}

	#endregion


	#region MonoBehaviour Functions -----------------------------

	void Start() {
		tileManager = transform.parent.GetComponent<TileManager>();
		if (!tileManager)
			Debug.Log("TileManager not found. (TileBehaviour)");

		tileBackground = transform.FindChild(TILE_BACKGROUND).gameObject;

		mRectTransform = GetComponent<RectTransform>();
		mAnimator = GetComponent<Animator>();
		mTileBackgroundBehaviour = tileBackground.GetComponent<TileBackgroundBehaviour>();
	}

	void FixedUpdate() {
		MoveToExpectedPos();
	}

	#endregion


	#region Movement --------------------------------------------

	void MoveToExpectedPos() {

		Vector2 distanceDiff = expectedPos - mRectTransform.anchoredPosition;


		if (distanceDiff.y == 0)
			currentSpeed.y = 0; 
		else
			currentSpeed.y += dropAcceleration * Mathf.Sign(distanceDiff.y) * Time.deltaTime;

		if (distanceDiff.x == 0) 
			currentSpeed.x = 0;
		else
			currentSpeed.x += dropAcceleration * Mathf.Sign(distanceDiff.x) * Time.deltaTime;

		Vector2 tempPos = mRectTransform.anchoredPosition + currentSpeed;

		if (
			Mathf.Sign(distanceDiff.y) > 0 && tempPos.y > expectedPos.y ||
			Mathf.Sign(distanceDiff.y) < 0 && tempPos.y < expectedPos.y
			) {
			tempPos.y = expectedPos.y;
		}

		if (
			Mathf.Sign(distanceDiff.x) > 0 && tempPos.x > expectedPos.x ||
			Mathf.Sign(distanceDiff.x) < 0 && tempPos.x < expectedPos.x
			) {
			tempPos.x = expectedPos.x;
		}


		mRectTransform.anchoredPosition = tempPos;

	}

	public void MergeAndDestroy(GameObject masterTile) {

		PlaySlaveMergeAnimation();

		StartCoroutine(Merge(masterTile));
	}

	IEnumerator Merge(GameObject masterTile) {
		while (true) {
			//print(mRectTransform.localPosition);
			expectedPos = masterTile.GetComponent<RectTransform>().anchoredPosition;
			if (mRectTransform.anchoredPosition == masterTile.GetComponent<RectTransform>().anchoredPosition) {
				Destroy(gameObject);
				yield break;
			}
			yield return null;
		}

	}

	#endregion


	#region Animation -------------------------------------------

	public void PlayMergeAnimation() {
		mAnimator.Play("merging");
	}

	void PlaySlaveMergeAnimation() {
		mAnimator.Play("slaveMerging");
	}

	public void Merge(Vector2 cnterPos, bool mergeLeft, bool mergeRight, bool mergeTop, bool mergeBottom) {
		isMerged = true;
		mergedCenterPos = centerPos;
		mTileBackgroundBehaviour.Merge(mergeLeft, mergeRight, mergeTop, mergeBottom);
	}

	#endregion

}
