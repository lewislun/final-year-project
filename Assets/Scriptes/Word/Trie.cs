using System.Collections;
using System.Collections.Generic;

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

	#endregion


}
