using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordChecker : MonoBehaviour {

	#region Public Variables --------------------------------------------

	public List<string> wordList = new List<string>();

	#endregion


	#region Private Variables -------------------------------------------

	private Trie trie = new Trie();

	#endregion


	#region Trie Operations ---------------------------------------------

	public void AddWordList() {
		trie.Clear();
		trie.Addwords(wordList);
	}

	#endregion


	#region CharacreWeights ------------------------------------------------

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

		print(weights);

		TileManager.GetInstance().characterWeights = weights;
	}

	#endregion


	#region Search Word ----------------------------------------------------
	
	public List<List<string>> FindLinkableCharactersForEmptySlots(List<List<GameObject>> tiles) {
		List<List<string>> characters = new List<List<string>>();
		for (int i = 0; i < tiles.Count; i++) {
			characters.Add(new List<string>());
			for (int j = 0; j < tiles[i].Count; j++) {
				if (tiles[i][j] == null)
					characters[i].Add("");
				else
					characters[i].Add(tiles[i][j].GetComponent<TileBehaviour>().character);
			}
		}
		return trie.FillEmptySlotsWithLinkableCharacters(characters);
	}

	#endregion


	public bool IsWordValid(string inputWord) {
		return trie.IsWordExist(inputWord);
	}


	public static WordChecker GetInstance() {
		return GameObject.Find("Manager").GetComponent<WordChecker>();
	}


}
