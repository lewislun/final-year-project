using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Levels {
	public LevelInfo[] levels;
}

[System.Serializable]
public class RequiredWord {
	public string word;
	public int count;
}

[System.Serializable]
public class LevelDetailPanel {
	public bool visible = false;
	public string imagePath = "";
	public string title = "";
}

[System.Serializable]
public class LevelInfo {
	public string name = "";
	public string topText = "";
	public float duration = -1f;	//-1 = infinite
	public string[] words = {};
	public string[] tiles = {};
	public string[][] tileSetup = {};	//Unity JsonParser does not support nested array
	public int rowCount = 8;
	public int colCount = 8;
	public bool enableChainify = true;
	public bool enableExchange = true;
	public bool canRetry = false;
	public bool showHints = true;
	public TileManager.CharacterWeight[] weights = {};
	public RequiredWord[] requiredWords = {};
	public LevelDetailPanel detailPanel = null;

	public void ParseTilesPreset(){
		tileSetup = new string[tiles.Length][];
		for (int i = 0; i < tiles.Length; i++){
			tileSetup[i] = tiles[i].Split(',');
			for(int j = 0; j < tileSetup[i].Length; j++)
				tileSetup[i][j] = tileSetup[i][j].ToUpper();
		}
	}

}
