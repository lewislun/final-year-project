using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBackgroundBehaviour : MonoBehaviour {

	#region Children Names --------------------------------------

	static string TOP_LEFT = "Top Left";
	static string TOP_RIGHT = "Top Right";
	static string BOTTOM_LEFT = "Bottom Left";
	static string BOTTOM_RIGHT = "Bottom Right";

	#endregion


	#region Public Properities ----------------------------------

	Color _backgroundColor;
	public Color backgroundColor {
		set {
			ChangeBackgroundColor(value);
			_backgroundColor = value;
		}
		get {
			return _backgroundColor;
		}
	}

	#endregion


	void ChangeBackgroundColor(Color color) {

		GetComponent<SpriteRenderer>().color = color;

		transform.FindChild(TOP_LEFT).GetComponent<SpriteRenderer>().color = color;
		transform.FindChild(TOP_RIGHT).GetComponent<SpriteRenderer>().color = color;
		transform.FindChild(BOTTOM_LEFT).GetComponent<SpriteRenderer>().color = color;
		transform.FindChild(BOTTOM_RIGHT).GetComponent<SpriteRenderer>().color = color;
	}

}
