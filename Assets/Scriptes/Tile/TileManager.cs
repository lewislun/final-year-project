using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

	[System.Serializable]
	public class TileSpriteInfo {
		public string character;
		public Sprite characterSprite;
		public Color characterColor = Color.black;
		public Color tileColor = Color.white;
	}

	[System.Serializable]
	public class CharacterWeight {
		public string character;
		public float weight;
	}

	#region Public Variables --------------------------------

	public GameObject tilePrefab;
	public int rowCount;
	public int colCount;
	public List<TileSpriteInfo> tileSpritesInfo = new List<TileSpriteInfo>();
	public List<CharacterWeight> characterWeights = new List<CharacterWeight>();

	#endregion


	#region Privavte Variables --------------------------------------------------

	private Dictionary<string, TileSpriteInfo> tileSpritesDictionary = new Dictionary<string, TileSpriteInfo>();
	private List<List<Vector2>> tilePos = new List<List<Vector2>>();
	private List<List<GameObject>> tiles = new List<List<GameObject>>();

	#endregion


	#region MonoBehaviour Functions ---------------------------------------------

	public void Start() {

		if (!tilePrefab)
			Debug.Log("missing tilePrefab (TileManager)");

		foreach (TileSpriteInfo ts in tileSpritesInfo)
			tileSpritesDictionary.Add(ts.character, ts);

		InitTilePos();
	}

	#endregion


	#region tilePos -------------------------------------------------------------

	public Vector2 GetTilePos(int row, int col) {
		if (row >= 0 && row < rowCount && col >= 0 && col < colCount)
			return tilePos[row][col];
		else
			return CalculateTilePos(row, col);
	}

	void InitTilePos() {
		tilePos.Clear();

		RectTransform tileRt = tilePrefab.GetComponent<RectTransform>();

		for (int i = 0; i < rowCount; i++) {
			tilePos.Add(new List<Vector2>());
			for (int j = 0; j < colCount; j++) 
				tilePos[i].Add(CalculateTilePos(i, j));
		}
	}

	Vector2 CalculateTilePos(int row, int col) {
		RectTransform rt = GetComponent<RectTransform>();
		RectTransform tileRt = tilePrefab.GetComponent<RectTransform>();

		float totalWidth = tileRt.sizeDelta.x * colCount;
		float totalHeight = tileRt.sizeDelta.y * rowCount;
		float spacingX = (rt.sizeDelta.x - totalWidth) / (colCount - 1);
		float spacingY = (rt.sizeDelta.y - totalHeight) / (rowCount - 1);

		return new Vector2(
			(tileRt.sizeDelta.x + spacingX) * col + tileRt.sizeDelta.x/2,
			0 - (tileRt.sizeDelta.y + spacingY) * row - tileRt.sizeDelta.y/2
		);

	}

	#endregion


	#region Tile Generation -----------------------------------------------------

	public void GenerateTiles() {
		InitTileList();
		Drop();
	}

	void InitTileList() {
		tiles.Clear();
		for (int i = 0; i < rowCount; i++) {
			tiles.Add(new List<GameObject>());
			for (int j = 0; j < colCount; j++)
				tiles[i].Add(null);
		}
	}

	public TileSpriteInfo GetTileSpriteInfo(string str) {

		TileSpriteInfo tempTS;
		if (tileSpritesDictionary.TryGetValue(str, out tempTS)) 
			return tempTS;
		else {
			Debug.Log("TileSprite not found (character: " + str + ") (TileManager.GetTileSprite)");
			return null;
		}
	}

	public void CreateTile(string tileCharacter, int row, int col, int startPosOffset) {

		TileSpriteInfo tileSpriteInfo = GetTileSpriteInfo(tileCharacter);

		if (tileSpriteInfo != null && tilePrefab) {
			GameObject newTile = Instantiate(tilePrefab, transform);
			newTile.name = row + "" + col + " Tile";

			SpriteRenderer characterSpriteRenderer = newTile.transform.FindChild(TileBehaviour.CHARACTER).GetComponent<SpriteRenderer>();
			characterSpriteRenderer.color = tileSpriteInfo.characterColor;
			characterSpriteRenderer.sprite = tileSpriteInfo.characterSprite;

			TileBehaviour newTileBehaviour = newTile.GetComponent<TileBehaviour>();
			newTileBehaviour.backgroundColor = tileSpriteInfo.tileColor;
			newTileBehaviour.character = tileCharacter;
			newTileBehaviour.row = row;
			newTileBehaviour.col = col;

			RectTransform newRT = newTile.GetComponent<RectTransform>();
			switch (GetDropSection(row, col)) {
				case 1:
					newRT.anchoredPosition = GetTilePos(startPosOffset, col);
					break;
				case 2:
					newRT.anchoredPosition = GetTilePos(row, colCount - 1 - startPosOffset);
					break;
				case 3:
					newRT.anchoredPosition = GetTilePos(rowCount - 1 - startPosOffset, col);
					break;
				case 4:
					newRT.anchoredPosition = GetTilePos(row, startPosOffset);
					break;
			}

			tiles[row][col] = newTile;
		}
	}

	public void CreateTile(string tileCharacter, int row, int col) {
		CreateTile(tileCharacter, row, col, -1);
	}

	public string GetRandomCharacter() {
		float totalWeight = 0;
		for (int i = 0; i < characterWeights.Count; i++)
			totalWeight += characterWeights[i].weight;
		float randomNum = Random.Range(0, totalWeight);
		for (int i = 0; i < characterWeights.Count; i++) {
			randomNum -= characterWeights[i].weight;
			if (randomNum <= 0)
				return characterWeights[i].character;
		}

		return "error";
	}
	
	public void AutoAdjustCharacterWeight(List<string> words) {

		Dictionary<char, int> weights = new Dictionary<char, int>();

		foreach (string word in words) {

			string upperWord = word.ToUpper();

			for (int i = 0; i < word.Length; i++) {

				if (weights.ContainsKey(upperWord[i]))
					weights[upperWord[i]]++;
				else
					weights[upperWord[i]] = 1;
			}
		}

		print("auto");

		characterWeights.Clear();
		foreach (KeyValuePair<char, int> weight in weights) {
			CharacterWeight tempWeight = new CharacterWeight();
			tempWeight.character = weight.Key + "";
			tempWeight.weight = weight.Value;
			characterWeights.Add(tempWeight);
		}
	}

	#endregion


	#region Drop and Destroy Tiles ---------------------------------------

	//1 = down, 2 = left, 3 = up, 4 = right
	public int GetDropSection(int row, int col) {
		if (col >= row && col < rowCount - 1 - row)
			return 1;
		else if (row >= colCount - col - 1 && row < col)
			return 2;
		else if (col >= rowCount - row && col <= row)
			return 3;
		else
			return 4;
	}

	public void DestroyTile(int row, int col) {
		if (tiles[row][col] == null)
			return;
		Destroy(tiles[row][col]);
		tiles[row][col] = null;
	}

	public void Drop() {

		int[] tilePosOffset = new int[colCount];

		//section 1
		for (int i = 0; i < colCount; i++)
			tilePosOffset[i] = -2;
		for (int i = rowCount - 1; i >= 0; i--) {
			for (int j = 0; j < colCount; j++) {
				if (GetDropSection(i, j) != 1)
					continue;
				if (tiles[i][j] == null) {
					int row = i - 1;
					while (row >= 0) {
						if (tiles[row][j] != null) {
							tiles[row][j].GetComponent<TileBehaviour>().row = i;
							tiles[i][j] = tiles[row][j];
							tiles[row][j] = null;
							break;
						}
						row--;
					}
					if (row == -1)
						CreateTile(GetRandomCharacter(), i, j, --tilePosOffset[j]);
				}
			}
		}

		//section 2
		for (int i = 0; i < rowCount; i++)
			tilePosOffset[i] = -2;
		for (int j = 0; j < colCount; j++) {
			for (int i = 0; i < rowCount; i++) {
				if (GetDropSection(i, j) != 2) 
					continue;
				if (tiles[i][j] == null) {
					int col = j + 1;
					while (col < colCount) {
						if (tiles[i][col] != null) {
							tiles[i][col].GetComponent<TileBehaviour>().col = j;
							tiles[i][j] = tiles[i][col];
							tiles[i][col] = null;
							break;
						}
						col++;
					}
					if (col == colCount)
						CreateTile(GetRandomCharacter(), i, j, --tilePosOffset[i]);
				}
			}
		}

		//3
		for (int i = 0; i < colCount; i++)
			tilePosOffset[i] = -2;
		for (int i = 0; i < rowCount; i++) {
			for (int j = 0; j < colCount; j++) {
				if (GetDropSection(i, j) != 3)
					continue;
				if (tiles[i][j] == null) {
					int row = i + 1;
					while (row < rowCount) {
						if (tiles[row][j] != null) {
							tiles[row][j].GetComponent<TileBehaviour>().row = i;
							tiles[i][j] = tiles[row][j];
							tiles[row][j] = null;
							break;
						}
						row++;
					}
					if (row == rowCount)
						CreateTile(GetRandomCharacter(), i, j, --tilePosOffset[j]);
				}
			}
		}

		//4
		for (int i = 0; i < rowCount; i++)
			tilePosOffset[i] = -2;
		for (int j = colCount - 1; j >= 0; j--) {
			for (int i = 0; i < rowCount; i++) {
				if (GetDropSection(i, j) != 4)
					continue;
				if (tiles[i][j] == null) {
					int col = j - 1;
					while (col >= 0) {
						if (tiles[i][col] != null) {
							tiles[i][col].GetComponent<TileBehaviour>().col = j;
							tiles[i][j] = tiles[i][col];
							tiles[i][col] = null;
							break;
						}
						col--;
					}
					if (col == -1)
						CreateTile(GetRandomCharacter(), i, j, --tilePosOffset[i]);
				}
			}
		}

	}

	#endregion


	#region Merge -------------------------------------------------------

	public void MergeTiles(List<GameObject> linkedTiles) { 

		int minRow = rowCount;
		int minCol = colCount;
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

		Vector2 centerPos = calculateMergedCenterPos(linkedTiles);
		Vector2 mergedMinPos = new Vector2(minRow, minCol);
		Vector2 mergedMaxPos = new Vector2(maxRow, maxCol);

		for (int i = 0; i < linkedTiles.Count; i++) {
			TileBehaviour tileBehaviour = linkedTiles[i].GetComponent<TileBehaviour>();
			bool mergeRight = tileBehaviour.col + 1 <= maxCol;
			bool mergeLeft = tileBehaviour.col - 1 >= minCol;
			bool mergeBottom = tileBehaviour.row + 1 <= maxRow;
			bool mergeTop = tileBehaviour.row - 1 >= minRow;
			tileBehaviour.Merge(centerPos, mergedMinPos, mergedMaxPos, linkedTiles, mergeLeft, mergeRight, mergeTop, mergeBottom);
		}
	}

	Vector2 calculateMergedCenterPos(List<GameObject> linkedTiles) {

		Vector2 sum = new Vector2(0, 0);

		foreach (GameObject go in linkedTiles) {
			sum += new Vector2(go.transform.position.x, go.transform.position.y);
		}

		return sum / linkedTiles.Count;
	}

	#endregion

	public static TileManager GetInstance() {
		return GameObject.Find("Tiles Container").GetComponent<TileManager>();
	}


}
