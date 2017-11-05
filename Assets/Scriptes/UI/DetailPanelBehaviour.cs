using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailPanelBehaviour : MonoBehaviour {

	#region Children Names -------------------

	public static string PANEL = "Panel";
	public static string PANEL_IMAGE = "Panel Image";
	public static string PANEL_TITLE = "Title";
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

	#endregion


	#region Private Variables ----------------

	CanvasFadeBehaviour mCanvasFadeBehaviour;
	Text titleText;
	Image panelImage;
	GameObject wordListItemContainer;

	#endregion


	#region MonoBehaviour Functions ----------

	void Awake(){
		panelImage = transform.Find(PANEL).Find(PANEL_IMAGE).GetComponent<Image>();
		titleText = transform.Find(PANEL).Find(PANEL_TITLE).GetComponent<Text>();
		mCanvasFadeBehaviour = GetComponent<CanvasFadeBehaviour>();
		wordListItemContainer = transform.Find(PANEL).Find(PANEL_WORD_LIST).Find("Viewport").Find("Content").gameObject;
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

	public void setWordList(List<RequiredWord> wordList) {
		ClearWordList();
		Dictionary<string, WordDefinition> wordDefinitions = WordDictionary.GetInstance().wordDefinitions;
		foreach(RequiredWord word in wordList){
			string upperWord = word.word.ToUpper();
			if (wordDefinitions.ContainsKey(upperWord)){
				AddWordListItem(wordDefinitions[upperWord], word.count);
			} else {
				WordDefinition tempDefinition = new WordDefinition();
				tempDefinition.word = word.word;
				AddWordListItem(tempDefinition, word.count);
				Debug.Log("DetailPanelDehavbiour: word (" + word + ") not found in dictionary.");
			}
		}
	}

	void ClearWordList(){
		int itemCount = wordListItemContainer.transform.childCount;
		for (int i = itemCount - 1; i >= 0; i--){
			Destroy(wordListItemContainer.transform.GetChild(i).gameObject);
		}
	}

	void AddWordListItem(WordDefinition definition, int count) {
		if (wordListItemPrefab == null){
			Debug.Log("DetailPanelDehaviour: missin wordListItemPrefab");
			return;
		}

		string displayWord = StrToFirstCap(definition.word) + " (" + definition.partOfSpeech + ".)";

		GameObject newItem = Instantiate(wordListItemPrefab, wordListItemContainer.transform);
		newItem.transform.localScale = Vector3.one;
		newItem.transform.Find(PANEL_WORD_LIST_ITEM_DEFINITION).GetComponent<Text>().text = definition.definition;
		Transform upperPart = newItem.transform.Find(PANEL_WORD_LIST_ITEM_UPPER_PART);
		upperPart.Find(PANEL_WORD_LIST_ITEM_UPPER_PART_WORD).GetComponent<Text>().text = displayWord;
		upperPart.Find(PANEL_WORD_LIST_ITEM_UPPER_PART_COUNT).GetComponent<Text>().text = "x" + count;
	}

	string StrToFirstCap(string str){
		if (str == null)
			return "";
		string result = "";
		if (str.Length > 0)
			result += str.Substring(0,1).ToUpper();
		if (str.Length > 1)
			result += str.Substring(1,str.Length - 1);
		
		return result;
	}

	#endregion

}
