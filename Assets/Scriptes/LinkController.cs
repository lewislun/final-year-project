using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkController : MonoBehaviour {

	#region Children Names ---------------------------------------

	static string LINK_FLARE = "Link Flare";

	#endregion


	#region Public Variables ---------------------------------------------

	public float linkZPos = -1;

	#endregion


	#region Private Variables --------------------------------------------

	private List<GameObject> linkedTiles = new List<GameObject>();
	private bool prevTouching = false;

	private GameObject linkFlare;

	private LineRenderer mLineRenderer;

	#endregion


	#region MonoBehaviour Functions --------------------------------------

	void Start() {

		linkFlare = transform.FindChild(LinkController.LINK_FLARE).gameObject;
		mLineRenderer = GetComponent<LineRenderer>();

		mLineRenderer.enabled = false;
	}

	void Update() {
		UpdateLink();
	}

	#endregion


	#region Link Mechanics -----------------------------------------------

	Vector2 GetTouchPos() {

		Vector2 touchPos = Vector2.zero;
		bool isTouching = true;

		if (Input.touchCount > 0)
			touchPos = Input.GetTouch(0).position;
		else if (Input.GetMouseButton(0))
			touchPos = Input.mousePosition;
		else
			isTouching = false;

		if (isTouching)
			return Camera.main.ScreenToWorldPoint(touchPos);
		else
			return Vector2.zero;
	}

	void SetLinkTailFollowTouch(Vector2 touchPos) {
		mLineRenderer.SetPosition(mLineRenderer.numPositions -1, new Vector3(touchPos.x, touchPos.y, linkZPos));

		//change the link flare's position
		linkFlare.transform.position = new Vector3(touchPos.x, touchPos.y, linkZPos);
	}

	void DestroyLink(bool shouldDestroyLinkedTiles) {

		if (shouldDestroyLinkedTiles) {
			
			foreach (GameObject go in linkedTiles) {
				TileBehaviour TileBehaviour = go.GetComponent<TileBehaviour>();
				TileManager.GetInstance().DestroyTile(TileBehaviour.row, TileBehaviour.col);
			}
			TileManager.GetInstance().Drop();
			
		}

		mLineRenderer.enabled = false;
		linkedTiles.Clear();
	}

	bool IsTileLinkable(GameObject go) {
		if (linkedTiles.Count == 0)
			return true;

		TileBehaviour tileBehaviour = go.GetComponent<TileBehaviour>();

		int rowDiff = 0;
		int colDiff = 0;

		if (linkedTiles.Count > 0) {
			TileBehaviour lastTileInfo = linkedTiles[linkedTiles.Count - 1].GetComponent<TileBehaviour>();
			rowDiff = tileBehaviour.row - lastTileInfo.row;
			colDiff = tileBehaviour.col - lastTileInfo.col;
		}

		return (Mathf.Abs(rowDiff) <= 1 && Mathf.Abs(colDiff) <= 1);

	}

	void LinkTile(GameObject tile) {

		if (linkedTiles.Count == 0) {
			mLineRenderer.enabled = true;
			mLineRenderer.numPositions = 1;
		}

		TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour>();

		linkedTiles.Add(tile);
		tileBehaviour.isLinked = true;

		mLineRenderer.SetPosition(mLineRenderer.numPositions-1, new Vector3(tile.transform.position.x, tile.transform.position.y, linkZPos));
		mLineRenderer.numPositions++;
	}

	void UpdateLink() {
		Vector2 touchPos = GetTouchPos();
		bool isTouching = touchPos != Vector2.zero;

		SetLinkFlareActive(isTouching);

		//linking
		if (isTouching) {

			if (!prevTouching || linkedTiles.Count > 0) {

				RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
				if (hit && hit.collider.gameObject.tag == TagsManager.TILE && !hit.collider.transform.parent.GetComponent<TileBehaviour>().isLinked) {

					GameObject tile = hit.collider.transform.parent.gameObject;
					if (IsTileLinkable(tile)) 
						LinkTile(tile);
				}
			}
			SetLinkTailFollowTouch(touchPos);
		}
		//end linking
		else if (prevTouching) {
			string str = getLinkedWord();
			print(str);
			bool isWordValid = WordChecker.GetInstance().IsWordValid(str);
			if (!isWordValid && AreAllCharactersSame(str) && IsLinkRectangle())
				MergeLinkedTiles();
			DestroyLink(isWordValid);
		}

		prevTouching = isTouching;
	}

	#endregion


	#region Link Visual Effect ---------------------------------------------

	void SetLinkFlareActive(bool active) {
		ParticleSystem.EmissionModule emission = linkFlare.GetComponent<ParticleSystem>().emission;
		emission.enabled = active;
	}

	#endregion


	#region Merging -------------------------------------------------------

	bool AreAllCharactersSame(string str) {
		if (str.Length < 2)
			return false;
		for (int i = 1; i < str.Length; i++)
			if (str[i] != str[0])
				return false;
		return true;
	}

	bool IsLinkRectangle() {

		if (linkedTiles.Count < 2)
			return false;

		int minRow = TileManager.GetInstance().rowCount;
		int minCol = TileManager.GetInstance().colCount;
		int maxRow = 0;
		int maxCol = 0;

		foreach (GameObject go in linkedTiles) {
			TileBehaviour tileBehaviour = go.GetComponent<TileBehaviour>();

			if (tileBehaviour.isMerged)
				return false;

			if (tileBehaviour.row < minRow)
				minRow = tileBehaviour.row;
			if (tileBehaviour.row > maxRow)
				maxRow = tileBehaviour.row;
			if (tileBehaviour.col < minCol)
				minCol = tileBehaviour.col;
			if (tileBehaviour.col > maxCol)
				maxCol = tileBehaviour.col;
		}

		return ((maxRow - minRow + 1) * (maxCol - minCol + 1) == linkedTiles.Count);
	}

	void MergeLinkedTilesOld() {

		TileBehaviour masterTileBehaviour = linkedTiles[linkedTiles.Count - 1].GetComponent<TileBehaviour>();
		masterTileBehaviour.PlayMergeAnimation();

		for (int i = 0; i < linkedTiles.Count - 1; i++) {
			TileBehaviour tileBehaviour = linkedTiles[i].GetComponent<TileBehaviour>();
			TileManager.GetInstance().MergeTiles(
				masterTileBehaviour.row,
				masterTileBehaviour.col,
				tileBehaviour.row,
				tileBehaviour.col
			);
		}
		TileManager.GetInstance().Drop();
	}

	void MergeLinkedTiles() { //should move this func to TileManager?

		int minRow = TileManager.GetInstance().rowCount;
		int minCol = TileManager.GetInstance().colCount;
		int maxRow = 0;
		int maxCol = 0;

		foreach (GameObject go in linkedTiles) {
			TileBehaviour tileBehaviour = go.GetComponent<TileBehaviour>();

			if (tileBehaviour.row < minRow)
				minRow = tileBehaviour.row;
			if (tileBehaviour.row > maxRow)
				maxRow = tileBehaviour.row;
			if (tileBehaviour.col < minCol)
				minCol = tileBehaviour.col;
			if (tileBehaviour.col > maxCol)
				maxCol = tileBehaviour.col;
		}

		for (int i = 0; i < linkedTiles.Count; i++) {
			TileBehaviour tileBehaviour = linkedTiles[i].GetComponent<TileBehaviour>();
			bool mergeRight = tileBehaviour.col + 1 <= maxCol;
			bool mergeLeft = tileBehaviour.col - 1 >= minCol;
			bool mergeBottom = tileBehaviour.row + 1 <= maxRow;
			bool mergeTop = tileBehaviour.row - 1 >= minRow;
			tileBehaviour.Merge(calculateMergedCenterPos(), mergeLeft, mergeRight, mergeTop, mergeBottom);
		}
	}

	Vector2 calculateMergedCenterPos() {

		Vector2 sum = new Vector2(0, 0);

		foreach (GameObject go in linkedTiles) {
			TileBehaviour tileBehaviour = go.GetComponent<TileBehaviour>();
			sum += tileBehaviour.GetComponent<RectTransform>().anchoredPosition;
		}

		return sum / linkedTiles.Count;
	}

	#endregion

	#region Post-Link Actions ----------------------------------------------

	public string getLinkedWord() {
		string str = "";
		foreach (GameObject go in linkedTiles) {
			TileBehaviour TileBehaviour = go.GetComponent<TileBehaviour>();
			TileBehaviour.isLinked = false;
			str += TileBehaviour.character.ToUpper();
		}
		return str;
	}

	#endregion


}
