using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	#region Public Variables -----------------------------------------

	public TimerBehaviour timerBehaviour;

	#endregion


	#region Private Variables ----------------------------------------

	WordChecker wordChecker;
	TileManager tileManager;

	#endregion


	#region MonoBehaviour Functions ----------------------------------

	void Start () {
		tileManager = TileManager.GetInstance();
		wordChecker = WordChecker.GetInstance();
	}

	#endregion


	#region Game Control ---------------------------------------------

	public void StartGame(List<string> wordList) {
		PageNavigationManager.GetInstance().ChangePage("game");
		wordChecker.wordList = wordList;
		wordChecker.AddWordList();
		tileManager.AutoAdjustCharacterWeight(wordList);
		tileManager.GenerateTiles();

	}

	#endregion

	public static GameController GetInstance() {
		return GameObject.Find("Manager").GetComponent<GameController>();
	}
}
