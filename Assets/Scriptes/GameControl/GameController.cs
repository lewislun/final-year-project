using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameController : MonoBehaviour {

	#region Public Variables -----------------------------------------

	public Text topText;

	public TimerBehaviour timerBehaviour;
	public TextAsset tutorialLevelsJson;

	#endregion


	#region Properties -----------------------------------------------

	public bool isTutorial{
		get{
			return tutorialLevelIndex != -1;
		}
	}

	#endregion


	#region Private Variables ----------------------------------------

	WordChecker wordChecker;
	TileManager tileManager;
	LevelInfo[] tutorialLevels = {};

	int tutorialLevelIndex = -1;

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

		if (topText == null)
			Debug.Log("topText == null");
		else
			topText.text = levelInfo.topText;

		AbilityBehaviour.StopAllCooldown();
	}

	#endregion


	#region Tutorial Levels ------------------------------------------

	public void StartTutorial(){
		tutorialLevelIndex = -1;
		StartNextTutorialLevel();
	}

	public void StartNextTutorialLevel(){
		tutorialLevelIndex++;
		if (tutorialLevelIndex >= tutorialLevels.Length){
			tutorialLevelIndex = -1;
			tileManager.InitTiles();
			PageNavigationManager.GetInstance().ChangePage("tab");
		}
		else{
			print("tutorial level: " + tutorialLevelIndex + "/" + tutorialLevels.Length);
			StartGame(tutorialLevels[tutorialLevelIndex]);
		}
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
