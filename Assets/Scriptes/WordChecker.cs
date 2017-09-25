using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordChecker : MonoBehaviour {

	#region Public Variables -------------------------------------------------------

	public List<string> wordList = new List<string>();

	#endregion


	public void ChangeWordListAutoWeight(List<string> words) {
		wordList = words;

		string str = "";
		foreach (string word in wordList) 
			str += word;

		List<TileManager.CharacterWeight> weights = new List<TileManager.CharacterWeight>();
		for (int i = 0; i < str.Length; i++) {
			bool exist = false;
			foreach (TileManager.CharacterWeight weight in weights) {
				if (weight.character == str[i] + "") {
					weight.weight += 1;
					exist = true;
					break;
				}
			}

			if (!exist) {
				TileManager.CharacterWeight newWeight = new TileManager.CharacterWeight();
				newWeight.character = str[i] + "";
				newWeight.weight = 1;
				weights.Add(newWeight);
			}

		}

		TileManager.GetInstance().characterWeights = weights;
	}

	public bool IsWordValid(string inputWord) {

		foreach(string word in wordList) {
			if (inputWord.ToUpper() == word.ToUpper())
				return true;
		}

		return false;
	}


	public static WordChecker GetInstance() {
		return GameObject.Find("Manager").GetComponent<WordChecker>();
	}


}
