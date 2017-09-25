using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionBehaviour : MonoBehaviour {


	#region Children Names ----------------------------------------------

	static string TITLE_TEXT_NAME = "Mission Title";

	#endregion


	#region Public Variables --------------------------------------------

	public string missionTitle = "default mission";
	public float missionTimeLimit = 120f;
	public List<string> wordList = new List<string>();

	#endregion


	#region Private Variables -------------------------------------------

	private GameObject titleText;
	private Button mButton;

	#endregion


	#region MonoBehaviour Functions -------------------------------------

	void Start () {
		titleText = transform.FindChild(MissionBehaviour.TITLE_TEXT_NAME).gameObject;
		mButton = GetComponent<Button>();

		titleText.GetComponent<Text>().text = missionTitle;
		mButton.onClick.AddListener(() => StartGame());
	}
	

	#endregion

	void StartGame() {
		GameController.GetInstance().StartGame(this.wordList);
		
	}

}
