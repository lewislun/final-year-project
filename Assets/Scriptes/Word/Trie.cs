using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Node {
	public string value { get; set; }
	public Dictionary<string, Node> children { get; private set;}
	public Node parent { get; set; }

	public Node(Node parent, string value) {
		children = new Dictionary<string, Node>();
		this.parent = parent;
		this.value = value;
	}

	public Node AttachChild(Node child, string value) {
		if (children.ContainsKey(value))
			return children[value];

		return children[value] = child;
	}

	public Node AttachChild(string value) {
		return AttachChild(new Node(this, value), value);
	}
}

public class Trie {

	public const string WORD_END = "$";
	public const string ROOT = "^";

	private Node root = new Node(null, ROOT);

	public void Clear() {
		root = new Node(null, ROOT);
	}

	#region Add Words ------------------------------------------

	public void Addwords(List<string> words) {
		foreach(string word in words) {
			AddWord(word.ToUpper());
		}
	}

	public void AddWord(string word) {
		AddWordRecur(word, root);
	}

	void AddWordRecur(string word, Node curNode) {
		if (word.Length == 0)
			curNode.AttachChild(WORD_END);
		else {
			string remainder = word.Substring(1);
			AddWordRecur(remainder, curNode.AttachChild(word[0] + ""));
		}
	}

	#endregion


	#region Checking --------------------------------------------

	public bool IsWordExist(string word) {
		return IsWordExistRecur(word, root);
	}

	bool IsWordExistRecur(string word, Node curNode) {
		if (word == "")
			return curNode.children.ContainsKey(WORD_END);
		else {
			Node nextNode;
			if (!curNode.children.TryGetValue(word[0]+ "", out nextNode))
				return false;
			return IsWordExistRecur(word.Substring(1), nextNode);
		}
	}

	public List<List<string>> FillEmptySlotsWithLinkableCharacters(List<List<string>> characters) {
		for (int i = 0; i < characters.Count; i++) {
			for (int j = 0; j < characters[i].Count; j++) {
				SearchForLinkableCharacters(characters, i, j, root, new Dictionary<string, string>());
			}
		}
		return characters;
	}

	void SearchForLinkableCharacters(List<List<string>> characters, int row, int col, Node curNode, Dictionary<string, string> emptySlotDict) {
	
		string curCharacter = characters[row][col];
		int rowCount = TileManager.GetInstance().rowCount;
		int colCount = TileManager.GetInstance().colCount;

		if (curCharacter == "") {
			foreach(KeyValuePair<string, Node> pair in curNode.children) {
				Node nextNode = pair.Value;
				Dictionary<string, string> newDict = new Dictionary<string, string>(emptySlotDict);
				newDict[row + "/" + col] = pair.Key;

				if (nextNode.children.ContainsKey(WORD_END)) {
					FillEmptySlots(characters, emptySlotDict);
					return;
				}

				for (int i = -1; i <= 1; i++) {
					for (int j = -1; j <= 1; j++) {
						int nextRow = row + i;
						int nextCol = col + j;
						if (!(i == 0 && j == 0) && nextRow >= 0 && nextRow < rowCount && nextCol >= 0 && nextCol < colCount)
							SearchForLinkableCharacters(characters, nextRow, nextCol, nextNode, emptySlotDict);
					}
				}
			}
		}
		else if (curNode.children.ContainsKey(curCharacter)) {
			Node nextNode = curNode.children[curCharacter];
			if (nextNode.children.ContainsKey(WORD_END)) {
				FillEmptySlots(characters, emptySlotDict);
				return;
			}
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					int nextRow = row + i;
					int nextCol = col + j;
					if (!(i == 0 && j == 0) && nextRow >= 0 && nextRow < rowCount && nextCol >= 0 && nextCol < colCount)
						SearchForLinkableCharacters(characters, nextRow, nextCol, nextNode, emptySlotDict);
				}
			}
		}
	}

	void FillEmptySlots(List<List<string>> characters, Dictionary<string, string> emptySlotDict) {
		foreach (KeyValuePair<string, string> emptySlot in emptySlotDict) {
			string[] rowCol = emptySlot.Key.Split('/');
			int emptyRow = int.Parse(rowCol[0]);
			int emptyCol = int.Parse(rowCol[1]	);
			characters[emptyRow][emptyCol] = emptySlot.Value;
			Debug.Log("hihi");
		}
	}

	#endregion


	}
