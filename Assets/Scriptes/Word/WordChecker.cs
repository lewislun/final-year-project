﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordChecker : MonoBehaviour {

	#region Public Variables --------------------------------------------

	public List<string> wordList = new List<string>();
	public bool showHints = true;

	#endregion


	#region Private Variables -------------------------------------------

	private Trie trie = new Trie();

	#endregion


	#region Trie Operations ---------------------------------------------

	public void SetWordList(List<string> newList) {
		wordList = newList;
		for(int i = 0; i < wordList.Count; i++)
			wordList[i] = wordList[i].ToUpper();
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

	public void FindAllLinkableCharacter(List<List<GameObject>> tiles) {

		if (!showHints)
			return;

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

		ThreadedJob job = new ThreadedJob();
		job.threadFunctions.Add(() => {
			job.args["list_of_linkables"] = trie.FindAllLinkableCharacters(characters);
		});
		job.onFinish.Add(() => {
			print("finished");
			TileManager.GetInstance().ClearHints();
			foreach (LinkableCharacter linkable in (List<LinkableCharacter>)job.args["list_of_linkables"]) {
				print(linkable.row + " " + linkable.col + ": " + linkable.word);
				tiles[linkable.row][linkable.col].GetComponent<TileBehaviour>().isHint = true;
			}
		});

		job.Start();
		StartCoroutine(job.WaitFor());

	}

	#endregion


	public bool IsWordValid(string inputWord) {
		return trie.IsWordExist(inputWord);
	}


	public static WordChecker GetInstance() {
		return GameObject.Find("Manager").GetComponent<WordChecker>();
	}


}
