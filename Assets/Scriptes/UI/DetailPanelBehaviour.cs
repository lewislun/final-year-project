using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailPanelBehaviour : MonoBehaviour {

	#region Children Names -------------------

	public static string PANEL = "Panel";
	public static string PANEL_IMAGE = "Panel Image";
	public static string PANEL_TITLE = "Title";
	public static string PANEL_FINISH_WITHIN = "Finish Within";
	public static string PANEL_FINISH_WITHIN_DURATION = "duration";
	public static string PANEL_WORD_LIST = "Word List";

	public static string PANEL_WORD_LIST_ITEM_UPPER_PART = "Upper Part";
	public static string PANEL_WORD_LIST_ITEM_UPPER_PART_WORD = "word";
	public static string PANEL_WORD_LIST_ITEM_UPPER_PART_COUNT = "count";
	public static string PANEL_WORD_LIST_ITEM_DEFINITION = "definition";

	#endregion


	#region Public Variables -----------------

	public GameObject wordListItemPrefab;

	#endregion


	#region Properties -----------------------

	public string title {
		get {
			return titleText.text;
		}
		set {
			titleText.text = value;
		}
	}

	public Sprite image {
		get {
			return panelImage.sprite;
		}
		set {
			panelImage.enabled = value != null;
			panelImage.sprite = value;
		}
	}

	public float duration {
		set {
			durationContainer.SetActive(value >= 0);
			durationText.text = value + "s";
		}
	}

	#endregion


	#region Private Variables ----------------

	CanvasFadeBehaviour mCanvasFadeBehaviour;
	Text titleText;
	Image panelImage;
	GameObject durationContainer;
	Text durationText;
	GameObject wordListItemContainer;

	#endregion


	#region MonoBehaviour Functions ----------

	void Awake(){
		panelImage = transform.Find(PANEL).Find(PANEL_IMAGE).GetComponent<Image>();
		titleText = transform.Find(PANEL).Find(PANEL_TITLE).GetComponent<Text>();
		mCanvasFadeBehaviour = GetComponent<CanvasFadeBehaviour>();
		wordListItemContainer = transform.Find(PANEL).Find(PANEL_WORD_LIST).Find("Viewport").Find("Content").gameObject;
		durationContainer = transform.Find(PANEL).Find(PANEL_FINISH_WITHIN).gameObject;
		durationText = durationContainer.transform.Find(PANEL_FINISH_WITHIN_DURATION).GetComponent<Text>();
	}

	#endregion


	#region Panel Control --------------------

	public void Show(bool animated){
		mCanvasFadeBehaviour.Show(animated);
	}

	public void Hide(bool animated){
		mCanvasFadeBehaviour.Hide(animated);
	}

	#endregion


	#region Word List ------------------------

	public void SetWordList(List<RequiredWord> wordList, Dictionary<string, string> filteredWords) {
		ClearWordList();
		Dictionary<string, WordDefinition> wordDefinitions = WordDictionary.GetInstance().wordDefinitions;
		foreach(RequiredWord word in wordList){
			string upperWord = word.word.ToUpper();
			string filteredWord = FilterWord(upperWord, filteredWords);
			if (wordDefinitions.ContainsKey(upperWord)){
				AddWordListItem(wordDefinitions[upperWord], filteredWord, word.count);
			} else {
				WordDefinition tempDefinition = new WordDefinition();
				AddWordListItem(tempDefinition, filteredWord, word.count);
				Debug.Log("DetailPanelDehavbiour: word (" + word + ") not found in dictionary.");
			}
		}
	}

	string FilterWord(string upperWord, Dictionary<string, string> filteredWords){
		if (filteredWords.ContainsKey(upperWord)){
			return filteredWords[upperWord];
		} else {
			return upperWord;
		}
	}

	public void ClearWordList(){
		int itemCount = wordListItemContainer.transform.childCount;
		for (int i = itemCount - 1; i >= 0; i--){
			Destroy(wordListItemContainer.transform.GetChild(i).gameObject);
		}
	}

	void AddWordListItem(WordDefinition definition, string displayWord, int count) {
		if (wordListItemPrefab == null){
			Debug.Log("DetailPanelDehaviour: missin wordListItemPrefab");
			return;
		}

		displayWord = StringOperation.ToFirstUpper(displayWord) + " (" + definition.partOfSpeech + ".)";

		Debug.Log(displayWord);

		GameObject newItem = Instantiate(wordListItemPrefab, wordListItemContainer.transform);
		newItem.transform.localScale = Vector3.one;
		newItem.transform.Find(PANEL_WORD_LIST_ITEM_DEFINITION).GetComponent<Text>().text = definition.definition;
		Transform upperPart = newItem.transform.Find(PANEL_WORD_LIST_ITEM_UPPER_PART);
		upperPart.Find(PANEL_WORD_LIST_ITEM_UPPER_PART_WORD).GetComponent<Text>().text = displayWord;
		upperPart.Find(PANEL_WORD_LIST_ITEM_UPPER_PART_COUNT).GetComponent<Text>().text = "x" + count;
	}

	#endregion

}
