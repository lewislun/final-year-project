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

	#endregion

	#region Public Varialbes ------------------------

	public GameObject remainingWordItemPrefab;

	#endregion


	#region Private Variables -----------------------

	GameObject leftContainer = null;
	GameObject rightContainer = null;
	Dictionary<string, GameObject> remainingWords = new Dictionary<string, GameObject>();

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

	public void SetWordList(RequiredWord[] requiredWords) {
		ClearList();
		for(int i = 0; i < requiredWords.Length; i++){
			if (i%2 == 0){
				AddWordItem(requiredWords[i], leftContainer);
			} else {
				AddWordItem(requiredWords[i], rightContainer);
			}
		}
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

	#endregion

}
