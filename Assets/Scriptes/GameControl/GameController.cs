using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameController : MonoBehaviour {

	#region Public Variables -----------------------------------------

	public TimerBehaviour timerBehaviour;
	public TextAsset tutorialLevelsJson;

	#endregion


	#region Private Variables ----------------------------------------

	WordChecker wordChecker;
	TileManager tileManager;
	LevelInfo[] tutorialLevels = {};

	#endregion


	#region MonoBehaviour Functions ----------------------------------

	void Start () {
		tileManager = TileManager.GetInstance();
		wordChecker = WordChecker.GetInstance();
		tutorialLevels = ReadLevels(tutorialLevelsJson);
	}

	#endregion


	#region Game Control ---------------------------------------------

	public void StartGame(List<string> wordList) {
		PageNavigationManager.GetInstance().ChangePage("game");
		wordChecker.SetWordList(wordList);
		tileManager.AutoAdjustCharacterWeight(wordList);
		tileManager.GenerateTiles();
	}

	public void StartGame(LevelInfo levelInfo) {
		PageNavigationManager.GetInstance().ChangePage("game");
		List<string> wordList = new List<string>(levelInfo.words);
		wordChecker.SetWordList(wordList);
		if (levelInfo.weights.Length == 0)
			tileManager.AutoAdjustCharacterWeight(wordList);
		else
			tileManager.characterWeights = (new List<TileManager.CharacterWeight>(levelInfo.weights));
		tileManager.rowCount = levelInfo.rowCount;
		tileManager.colCount = levelInfo.colCount;
		tileManager.GenerateTiles(levelInfo.tileSetup);
	}

	//testing
	public void StartTutorial(){
		StartGame(tutorialLevels[1]);
	}

	#endregion


	#region Read Levels JSON -----------------------------------------

	LevelInfo[] ReadLevels(TextAsset jsonFile){
		Levels levels = JsonUtility.FromJson<Levels>(jsonFile.text);
		LevelInfo[] results = levels.levels;
		foreach(LevelInfo level in results)
			level.ParseTilesPreset();
		
		return results;
	}

	#endregion


	public static GameController GetInstance() {
		return GameObject.Find("Manager").GetComponent<GameController>();
	}
}
