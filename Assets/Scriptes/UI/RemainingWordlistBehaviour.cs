using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingWordlistBehaviour : MonoBehaviour {

	#region Children Names --------------------------

	public static string LEFT_CONTAINER = "Left";
	public static string RIGHT_CONTAINER = "Right";

	public static string REMAINING_WORD_ITEM_WORD = "Word";
	public static string REMAINING_WORD_ITEM_COUNT = "Count";
	public static string REMAINING_WORD_ITEM_STRIKETHROUGH = "Strikethrough";

	#endregion

	#region Public Varialbes ------------------------

	public GameObject remainingWordItemPrefab;

	#endregion


	#region Private Variables -----------------------

	GameObject leftContainer = null;
	GameObject rightContainer = null;
	Dictionary<string, GameObject> remainingWords = new Dictionary<string, GameObject>();
	Dictionary<string, string> filteredWords = new Dictionary<string, string>();

	#endregion


	#region MonoBehaviour Functions -----------------

	void Awake() {
		if (remainingWordItemPrefab == null)
			Debug.Log("RemainingWOrdlistBehaviour: remainingWordItemPrefab == null");
		leftContainer = transform.Find(LEFT_CONTAINER).gameObject;
		rightContainer = transform.Find(RIGHT_CONTAINER).gameObject;
	}

	#endregion



	#region Wordlist Operation ----------------------

	public void SetWordList(RequiredWord[] requiredWords, Dictionary<string, string> filteredWords) {
		ClearList();
		this.filteredWords = filteredWords;
		for(int i = 0; i < requiredWords.Length; i++){
			string filteredWord = FilterWord(requiredWords[i].word, filteredWords);
			if (i%2 == 0){
				AddWordItem(filteredWord, requiredWords[i].count, leftContainer);
			} else {
				AddWordItem(filteredWord, requiredWords[i].count, rightContainer);
			}
		}
	}

	string FilterWord(string word, Dictionary<string, string> filteredWords){
		string upperWord = word.ToUpper();
		if (filteredWords.ContainsKey(upperWord))
			return filteredWords[upperWord];
		else
			return word;
	}

	public void ClearList(){
		for (int i = leftContainer.transform.childCount - 1; i >= 0; i--)
			Destroy(leftContainer.transform.GetChild(i).gameObject);
		for (int i = rightContainer.transform.childCount - 1; i >= 0; i--)
			Destroy(rightContainer.transform.GetChild(i).gameObject);
	}

	void AddWordItem(RequiredWord requiredWord, GameObject parent){
		AddWordItem(requiredWord.word, requiredWord.count, parent);
	}

	void AddWordItem(string word, int count, GameObject parent) {
		GameObject newItem = Instantiate(remainingWordItemPrefab, parent.transform);
		newItem.transform.Find(REMAINING_WORD_ITEM_WORD).GetComponent<Text>().text = StringOperation.ToFirstUpper(word);
		newItem.transform.Find(REMAINING_WORD_ITEM_COUNT).GetComponent<Text>().text = "x" + count;
		newItem.transform.localScale = Vector3.one;

		remainingWords[word.ToUpper()] = newItem;
	}

	public void UpdateCount(string word, int newCount) {
		string upperWord = word.ToUpper();
		if (filteredWords.ContainsKey(upperWord)){
			upperWord = filteredWords[upperWord].ToUpper();
		}
		if (!remainingWords.ContainsKey(upperWord)){
			Debug.Log("RemainingWordlistBehaviour.UpdateCount(): word not found (" + word + ")");
			return;
		}

		GameObject go = remainingWords[upperWord];
		go.transform.Find(REMAINING_WORD_ITEM_COUNT).GetComponent<Text>().text = "x" + newCount;
		CanvasFadeBehaviour strikethoughFader = go.transform.Find(REMAINING_WORD_ITEM_STRIKETHROUGH).GetComponent<CanvasFadeBehaviour>();
		CanvasFadeBehaviour goFader = go.GetComponent<CanvasFadeBehaviour>();
		if (newCount == 0){
			strikethoughFader.Show(true);
			goFader.Hide(true);
		} else {
			strikethoughFader.Hide(true);
			goFader.Show(true);
		}
	}

	#endregion

}
