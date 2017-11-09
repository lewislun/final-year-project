using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;

public class GameController : MonoBehaviour {

	#region Public Variables -----------------------------------------

	public Text topText;
	public GameObject pauseButton;
	public GameObject retryButton;
	public GameObject detailButton;
	public DetailPanelBehaviour detailPanel;
	public RemainingWordlistBehaviour remainingWordlist;
	public TimerBehaviour timer;

	public UnityEvent onInitLevel = new UnityEvent();

	public TextAsset levelJson;

	#endregion


	#region Properties -----------------------------------------------

	LevelInfo[] _levelSeries = {};
	public LevelInfo[] levelSeries {
		get {
			return _levelSeries;
		}
		private set {
			_levelSeries = value;
		}
	}

	#endregion


	#region Private Variables ----------------------------------------

	WordChecker wordChecker;
	TileManager tileManager;
	LinkController linkController;

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
		if (pauseButton == null)
			Debug.Log("pauseButton == null");
		if (remainingWordlist == null)
			Debug.Log("remainingWOrdlist == null");
		if (timer == null)
			Debug.Log("timer == null");
		tileManager = TileManager.GetInstance();
		wordChecker = WordChecker.GetInstance();
		linkController = LinkController.GetInstance();
	}

	void Start () {
		levelSeries = ReadLevels(levelJson);
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

		onInitLevel.Invoke();

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

		//Filtered words for quiz level
		Dictionary<string, string> filteredWords = new Dictionary<string, string>();
		for (int i = 0; i < levelInfo.filteredWords.Length; i++){
			filteredWords[levelInfo.filteredWords[i].originalWord.ToUpper()] = levelInfo.filteredWords[i].filteredWord;
		}

		//UI related
		if (topText != null) {
			remainingWordlist.ClearList();
			topText.text = levelInfo.topText;
			if (levelInfo.topText == "") {
				remainingWordlist.SetWordList(levelInfo.requiredWords, filteredWords);
			}
		}
			
		retryButton.SetActive(levelInfo.canRetry);
		detailButton.SetActive(levelInfo.detailPanel.visible);

		if (levelInfo.detailPanel.visible){
			detailPanel.Show(true);
			detailPanel.title = levelInfo.detailPanel.title;
			detailPanel.image = Resources.Load<Sprite>(levelInfo.detailPanel.imagePath);
			if (detailPanel.image == null) {
				detailPanel.SetWordList(new List<RequiredWord>(levelInfo.requiredWords), filteredWords);
			} else {
				detailPanel.ClearWordList();
			}
		} else {
			detailPanel.Hide(false);
		}

		//Timer related
		timer.StopTimer();
		timer.paused = false;
		timer.duration = levelInfo.duration;
		pauseButton.SetActive(levelInfo.duration >= 0);

		//abilities
		AbilityBehaviour.ResetAllAbilities();
		for(int i = 0; i < levelInfo.abilities.Length; i++)
			AbilityBehaviour.ConfigAbility(levelInfo.abilities[i]);

		//link
		linkController.shouldGenerateNewTile = levelInfo.shouldGenerateNewTile;
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
			//curLevelIndex = -1;
			tileManager.InitTiles();
			PageNavigationManager.GetInstance().ChangePage("level");
		}
		else{
			Debug.Log("Level: " + curLevelIndex + "/" + curLevelSeries.Length);
			if (LevelProgress.GetProgress() < curLevelIndex)
				LevelProgress.SetProgress(curLevelIndex);
			StartGame(curLevelSeries[curLevelIndex]);
		}
	}

	public void StartLevelSeries(LevelInfo[] levelSeries){
		curLevelSeries = levelSeries;
		curLevelIndex = -1;
		StartNextLevel();
	}

	public void StartLevel(int levelIndex){
		curLevelSeries = levelSeries;
		curLevelIndex = levelIndex - 1;
		StartNextLevel();
	}

	public void RestartLevel(){
		StartGame(curLevelSeries[curLevelIndex]);
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


	public void ResetLevelProgress(){
		LevelProgress.SetProgress(0);
	}

	public static GameController GetInstance() {
		return GameObject.Find("Manager").GetComponent<GameController>();
	}
}
