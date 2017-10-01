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
	TileCharacterBehaviour mTileCharacterBehaviour;
	TileGlowingBorderBehaviour mTileGlowingBorderBehaviour;
	RectTransform mRectTransform;
	Animator mAnimator;

	#endregion


	#region Properties -----------------------------------

	List<GameObject> _mergedPeerTiles = null;
	public List<GameObject> mergedPeerTiles {
		get {
			return _mergedPeerTiles;
		}
		private set {
			_mergedPeerTiles = value;
		}
	}

	Vector2 _mergedCenterPos = new Vector2(0, 0);
	public Vector2 mergedCenterPos {
		get {
			return _mergedCenterPos;
		}
		private set {
			_mergedCenterPos = value;
		}
	}

	Vector2 _mergedMinPos = new Vector2(0, 0);
	public Vector2 mergedMinPos {
		get {
			return _mergedMinPos;
		}
		private set {
			_mergedMinPos = value;
		}
	}

	Vector2 _mergedMaxPos = new Vector2(0, 0);
	public Vector2 mergedMaxPos {
		get {
			return _mergedMaxPos;
		}
		private set {
			_mergedMaxPos = value;
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

	int _row = -1;
	public int row {
		get {
			return _row;
		}
		set {
			_row = value;
			isStable = false;
			expectedPos = transform.parent.GetComponent<TileManager>().GetTilePos(row, col);
		}
	}

	int _col = -1;
	public int col {
		get {
			return _col;
		}
		set {
			_col = value;
			isStable = false;
			expectedPos = transform.parent.GetComponent<TileManager>().GetTilePos(row, col);
		}
	}

	bool _isStable = false;
	public bool isStable {
		get {
			return _isStable;
		}
		private set {
			_isStable = value;
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

	public Color glowingBorderColor {
		get {
			return transform.FindChild(LINKED_GROWING_BORDER).GetComponent<TileGlowingBorderBehaviour>().color;
		}
		set {
			transform.FindChild(LINKED_GROWING_BORDER).GetComponent<TileGlowingBorderBehaviour>().color = value;
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
		mTileCharacterBehaviour = transform.FindChild(CHARACTER).GetComponent<TileCharacterBehaviour>();
		mTileGlowingBorderBehaviour = transform.FindChild(LINKED_GROWING_BORDER).GetComponent<TileGlowingBorderBehaviour>();
	}

	void FixedUpdate() {
		if (!isStable)
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

		if (distanceDiff == Vector2.zero) {
			isStable = true;
			return;
		}

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

	#endregion


	#region Merge -----------------------------------------------

	public void Merge(Vector2 centerPos, Vector2 minPos, Vector2 maxPos, List<GameObject> peerTiles, bool mergeLeft, bool mergeRight, bool mergeTop, bool mergeBottom) {

		isMerged = true;
		mTileCharacterBehaviour.MoveTo(centerPos);
		mTileBackgroundBehaviour.Merge(mergeLeft, mergeRight, mergeTop, mergeBottom);
		mTileGlowingBorderBehaviour.MergeBorder(mergeLeft, mergeRight, mergeTop, mergeBottom);

		mergedPeerTiles = new List<GameObject>(peerTiles);
		mergedCenterPos = centerPos;
		mergedMinPos = minPos;
		mergedMaxPos = maxPos;
	}

	public void Unmerge() {
		isMerged = false;
		mTileBackgroundBehaviour.Unmerge();
		mTileCharacterBehaviour.MoveToOriginalPos();
		mTileGlowingBorderBehaviour.MergeBorder(false, false, false, false);
	}

	#endregion

}
