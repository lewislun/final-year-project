using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Words {
	public WordDefinition[] words;
}

[System.Serializable]
public class WordDefinition {
	public string word = "";
	public string partOfSpeech = "";
	public string definition = "";
}

public class WordDictionary: MonoBehaviour {

	#region Public Variables ---------------

	public TextAsset dictionaryJson = null;
	public List<WordDefinition> wordDefinitions = new List<WordDefinition>();

	#endregion


	#region MonoBehaviour Functions --------

	void Start(){
		if (dictionaryJson == null) {
			Debug.Log("WordDictionary: dictionaryJson == null");
		} else {
			ReadDictionaryJson();
		}
	}

	#endregion

	void ReadDictionaryJson() {
		Words temp = JsonUtility.FromJson<Words>(dictionaryJson.text);
		wordDefinitions = new List<WordDefinition>(temp.words);

		Debug.Log(GetInstance().wordDefinitions);
	}

	public static WordDictionary GetInstance(){
		return GameObject.Find("Manager").GetComponent<WordDictionary>();
	}

}
