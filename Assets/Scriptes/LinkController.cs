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
	private TileManager tileManager;
	private TouchManager touchManager;

	private GameObject linkFlare;

	private LineRenderer mLineRenderer;

	private string linkedStr = "";

	#endregion


	#region MonoBehaviour Functions --------------------------------------

	void Start() {

		tileManager = TileManager.GetInstance();
		touchManager = TouchManager.GetInstance();

		linkFlare = transform.FindChild(LinkController.LINK_FLARE).gameObject;
		mLineRenderer = GetComponent<LineRenderer>();

		mLineRenderer.enabled = false;
	}

	void FixedUpdate() {
		UpdateLink();
	}

	#endregion


	#region Link Mechanics -----------------------------------------------

	void SetLinkTailFollowTouch(Vector2 touchPos) {
		mLineRenderer.SetPosition(mLineRenderer.numPositions -1, new Vector3(touchPos.x, touchPos.y, linkZPos));

		//change the link flare's position
		linkFlare.transform.position = new Vector3(touchPos.x, touchPos.y, linkZPos);
	}

	void DestroyLink(bool shouldDestroyLinkedTiles) {

		foreach (GameObject go in linkedTiles) {
			go.GetComponent<TileBehaviour>().isLinked = false;
		}

		if (shouldDestroyLinkedTiles) {
			
			foreach (GameObject go in linkedTiles) {
				TileBehaviour TileBehaviour = go.GetComponent<TileBehaviour>();
				TileManager.GetInstance().DestroyTile(TileBehaviour.row, TileBehaviour.col);
			}
			tileManager.Drop();
			
		}

		mLineRenderer.enabled = false;
		linkedTiles.Clear();
	}

	void LinkTile(GameObject tile) {

		if (linkedTiles.Count == 0) {
			mLineRenderer.enabled = true;
			mLineRenderer.numPositions = 1;
		}

		TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour>();
		Vector2 linkAnchor;

		if (tileBehaviour.isMerged) {
			foreach (GameObject peerTile in tileBehaviour.mergedPeerTiles) {
				linkedTiles.Add(peerTile);
				peerTile.GetComponent<TileBehaviour>().isLinked = true;
			}
			linkAnchor = tileBehaviour.mergedCenterPos;
		}
		else {
			linkedTiles.Add(tile);
			tileBehaviour.isLinked = true;
			linkAnchor = new Vector2(tile.transform.position.x, tile.transform.position.y);
		}

		linkedStr += tileBehaviour.character;

		mLineRenderer.SetPosition(mLineRenderer.numPositions-1, new Vector3(linkAnchor.x, linkAnchor.y, linkZPos));
		mLineRenderer.numPositions++;
	}

	void UpdateLink() {
		bool isTouching = touchManager.isTouching && touchManager.touchPriority <= 0;
		Vector2 touchPos = touchManager.touchPos;

		SetLinkFlareActive(isTouching);

		//linking
		if (isTouching) {

			if (!prevTouching || linkedTiles.Count > 0) {

				GameObject tile = TouchManager.GetTouchingTile();

				if (tile != null && !tile.GetComponent<TileBehaviour>().isLinked) 
					if (IsTileAdjacentToLastTile(tile)) 
						LinkTile(tile);
				
			}
			SetLinkTailFollowTouch(touchPos);
		}

		//end linking
		else if (prevTouching) {
			//string str = getLinkedWord();
			print(linkedStr);

			bool isWordValid = WordChecker.GetInstance().IsWordValid(linkedStr);
			if (!isWordValid && AreAllCharactersSame(linkedStr) && IsLinkRectangle())
				tileManager.MergeTiles(linkedTiles);
			DestroyLink(isWordValid);

			linkedStr = "";
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


	#region Checking -------------------------------------------------------

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

	bool IsTileAdjacentToLastTile(GameObject tile) {
		if (linkedTiles.Count == 0)
			return true;

		TileBehaviour tileBehaviour = tile.GetComponent<TileBehaviour>();
		TileBehaviour lastTileBehaviour = linkedTiles[linkedTiles.Count - 1].GetComponent<TileBehaviour>();

		if (lastTileBehaviour.isMerged) {
			Vector2 topLeftCorner = new Vector2(lastTileBehaviour.mergedMinPos.x - 1, lastTileBehaviour.mergedMinPos.y - 1);
			Vector2 bottomRightCorner = new Vector2(lastTileBehaviour.mergedMaxPos.x + 1, lastTileBehaviour.mergedMaxPos.y + 1);

			return tileBehaviour.row >= topLeftCorner.x && tileBehaviour.row <= bottomRightCorner.x && tileBehaviour.col >= topLeftCorner.y && tileBehaviour.col <= bottomRightCorner.y;
		}
		else {
			int rowDiff = tileBehaviour.row - lastTileBehaviour.row;
			int colDiff = tileBehaviour.col - lastTileBehaviour.col;

			return (Mathf.Abs(rowDiff) <= 1 && Mathf.Abs(colDiff) <= 1);
		}
	}

	#endregion


}
