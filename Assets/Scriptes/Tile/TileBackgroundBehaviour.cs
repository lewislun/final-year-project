using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBackgroundBehaviour : MonoBehaviour {

	#region Children Names --------------------------------------

	static string TOP_LEFT = "Top Left";
	static string TOP_RIGHT = "Top Right";
	static string BOTTOM_LEFT = "Bottom Left";
	static string BOTTOM_RIGHT = "Bottom Right";

	#endregion


	#region Public Properities ----------------------------------

	Color _backgroundColor;
	public Color backgroundColor {
		set {
			ChangeBackgroundColor(value);
			_backgroundColor = value;
		}
		get {
			return _backgroundColor;
		}
	}

	#endregion


	#region Public Variables -----------------------------------

	public float cornerMoveDistance = 1f;
	public float cornerMoveDuration = 0.5f;

	#endregion


	#region Private Variables ----------------------------------

	Coroutine ongoingMerge = null;
	List<GameObject> tileCorners = new List<GameObject>();
	List<Vector2> cornerOriginalPos = new List<Vector2>();

	#endregion


	#region MonoBehaviour Functions ----------------------------

	void Start() {

		tileCorners.Add(transform.FindChild(TOP_LEFT).gameObject);
		tileCorners.Add(transform.FindChild(TOP_RIGHT).gameObject);
		tileCorners.Add(transform.FindChild(BOTTOM_LEFT).gameObject);
		tileCorners.Add(transform.FindChild(BOTTOM_RIGHT).gameObject);

		for (int i = 0; i < 4; i++)
			cornerOriginalPos.Add(tileCorners[i].GetComponent<RectTransform>().anchoredPosition);
	}

	#endregion



	#region Color ----------------------------------------------

	void ChangeBackgroundColor(Color color) {

		GetComponent<SpriteRenderer>().color = color;

		transform.FindChild(TOP_LEFT).GetComponent<SpriteRenderer>().color = color;
		transform.FindChild(TOP_RIGHT).GetComponent<SpriteRenderer>().color = color;
		transform.FindChild(BOTTOM_LEFT).GetComponent<SpriteRenderer>().color = color;
		transform.FindChild(BOTTOM_RIGHT).GetComponent<SpriteRenderer>().color = color;
	}

	#endregion


	#region Merging --------------------------------------------

	public void Merge(bool shouldMergeLeft, bool shouldMergeRight, bool shouldMergeTop, bool shouldMergeBottom) {

		if (ongoingMerge != null)
			StopCoroutine(ongoingMerge);

		Vector2[] cornerDisplacement = {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) };

		if (shouldMergeLeft) {
			cornerDisplacement[0].x = -cornerMoveDistance;
			cornerDisplacement[2].x = -cornerMoveDistance;
		}
		if (shouldMergeRight) {
			cornerDisplacement[1].x = cornerMoveDistance;
			cornerDisplacement[3].x = cornerMoveDistance;
		}
		if (shouldMergeTop) {
			cornerDisplacement[0].y = cornerMoveDistance;
			cornerDisplacement[1].y = cornerMoveDistance;
		}
		if (shouldMergeBottom) {
			cornerDisplacement[2].y = -cornerMoveDistance;
			cornerDisplacement[3].y = -cornerMoveDistance;
		}

		BoxCollider2D collider = GetComponent<BoxCollider2D>();
		float xSizeExpand = 0.75f;
		float ySizeExpand = 0.75f;

		collider.size = new Vector2(
				4 + (shouldMergeLeft ? xSizeExpand : 0) + (shouldMergeRight ? xSizeExpand : 0),
				4 + (shouldMergeTop ? ySizeExpand : 0) + (shouldMergeBottom ? ySizeExpand : 0)
			);
		collider.offset = new Vector2(
				(shouldMergeLeft ? -xSizeExpand/2 : 0) + (shouldMergeRight ? xSizeExpand/2 : 0),
				(shouldMergeTop ? ySizeExpand/2 : 0) + (shouldMergeBottom ? -ySizeExpand/2 : 0)
			);

		ongoingMerge = StartCoroutine(MergeCoroutine(cornerDisplacement));
	}

	public void Unmerge() {

		if (ongoingMerge != null)
			StopCoroutine(ongoingMerge);

		Vector2[] cornerDisplacement = {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) };

		ongoingMerge = StartCoroutine(MergeCoroutine(cornerDisplacement));
	}

	IEnumerator MergeCoroutine(Vector2[] cornerDisplacement) {

		for (int i = 0; i < 4; i++)
			if (cornerDisplacement[i] != Vector2.zero)
				tileCorners[i].SetActive(true);

		List<Vector2> expectedPos = new List<Vector2>();
		List<Vector2> startPos = new List<Vector2>();
		for (int i = 0; i < 4; i++) {
			expectedPos.Add(cornerOriginalPos[i] + cornerDisplacement[i]);
			startPos.Add(tileCorners[i].GetComponent<RectTransform>().anchoredPosition);
		}

		float timePassed = 0;
		while (timePassed < cornerMoveDuration) {
			timePassed += Time.deltaTime;
			for (int i = 0; i < 4; i++) {
				tileCorners[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos[i], expectedPos[i], timePassed / cornerMoveDuration);
			}
			yield return new WaitForFixedUpdate();
		}

		for (int i = 0; i < 4; i++) {
			tileCorners[i].GetComponent<RectTransform>().anchoredPosition = expectedPos[i];
			if (tileCorners[i].GetComponent<RectTransform>().anchoredPosition == cornerOriginalPos[i])
				tileCorners[i].SetActive(false);
		}

		ongoingMerge = null;
		yield break;
	}

	#endregion

}
