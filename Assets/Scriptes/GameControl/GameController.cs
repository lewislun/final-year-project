using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameController : MonoBehaviour {

	#region Public Variables -----------------------------------------

	public Text topText;
	public GameObject retryButton;
	public GameObject detailButton;
	public DetailPanelBehaviour detailPanel;
	public RemainingWordlistBehaviour remainingWordlist;

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

	LevelInfo[] curLevelSeries;
	int curLevelIndex = -1;
	Dictionary<string, int> requiredWords = new Dictionary<string, int>();
	bool hasWordTarget = false;

	#endregion


	#region MonoBehaviour Functions ----------------------------------

	void Awake() {
		if (retryButton == null)
			Debug.Log("retryButton == null");
		if (detailButton == null)
			Debug.Log("detailButton == null");
		if (detailPanel == null)
			Debug.Log("detailPanel == null");
		if (remainingWordlist == null)
			Debug.Log("remainingWOrdlist == null");
	}

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

		//init tileManager and wordChecker
		wordChecker.showHints = levelInfo.showHints;
		List<string> wordList = new List<string>(levelInfo.words);
		wordChecker.SetWordList(wordList);
		if (levelInfo.weights.Length == 0)
			tileManager.AutoAdjustCharacterWeight(wordList);
		else
			tileManager.characterWeights = (new List<TileManager.CharacterWeight>(levelInfo.weights));
		tileManager.rowCount = levelInfo.rowCount;
		tileManager.colCount = levelInfo.colCount;
		tileManager.GenerateTiles(levelInfo.tileSetup);

		//UI related
		if (topText == null)
			Debug.Log("topText == null");
		else {
			remainingWordlist.ClearList();
			topText.text = levelInfo.topText;
			if (levelInfo.topText == "") {
				remainingWordlist.SetWordList(levelInfo.requiredWords);
			}
		}
			
		retryButton.SetActive(levelInfo.canRetry);
		detailButton.SetActive(levelInfo.detailPanel.visible);

		if (levelInfo.detailPanel.visible){
			detailPanel.Show(true);
			detailPanel.title = levelInfo.detailPanel.title;
			detailPanel.image = Resources.Load<Sprite>(levelInfo.detailPanel.imagePath);
			if (detailPanel.image == null) {
				detailPanel.SetWordList(new List<RequiredWord>(levelInfo.requiredWords));
			} else {
				detailPanel.ClearWordList();
			}
		} else {
			detailPanel.Hide(false);
		}


		//init abilities
		AbilityBehaviour.StopAllCooldown();
		AbilityBehaviour.SetAbilityEnabled(AbilityBehaviour.AbilityName.Chainify, levelInfo.enableChainify);
		AbilityBehaviour.SetAbilityEnabled(AbilityBehaviour.AbilityName.Exchange, levelInfo.enableExchange);

		//level required words
		if (levelInfo.requiredWords.Length == 0)
			hasWordTarget = false;
		else{
			hasWordTarget = true;
			for(int i = 0; i < levelInfo.requiredWords.Length; i++){
				RequiredWord rw = levelInfo.requiredWords[i];
				requiredWords[rw.word.ToUpper()] = rw.count;
			}
		}
	}

	public bool FinishWord(string word){
		if (!hasWordTarget)
			return false;
		
		word = word.ToUpper();
		if (requiredWords.ContainsKey(word)){
			requiredWords[word]--;
			remainingWordlist.UpdateCount(word, requiredWords[word]);
			if (requiredWords[word] <= 0){
				requiredWords.Remove(word);
				if (requiredWords.Count <= 0){
					StartNextLevel();
					return true;
				}
			}
		}

		return false;
	}

	public void StartNextLevel(){
		curLevelIndex++;
		if (curLevelIndex >= curLevelSeries.Length){
			curLevelIndex = -1;
			tileManager.InitTiles();
			PageNavigationManager.GetInstance().ChangePage("tab");
		}
		else{
			Debug.Log("Level: " + curLevelIndex + "/" + curLevelSeries.Length);
			StartGame(curLevelSeries[curLevelIndex]);
		}
	}

	public void StartLevelSeries(LevelInfo[] levelSeries){
		curLevelSeries = levelSeries;
		curLevelIndex = -1;
		StartNextLevel();
	}

	public void RestartLevel(){
		StartGame(curLevelSeries[curLevelIndex]);
	}

	#endregion


	#region Tutorial Levels ------------------------------------------

	public void StartTutorial(){
		//tutorialLevelIndex = -1;
		//StartNextTutorialLevel();
		StartLevelSeries(tutorialLevels);
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
