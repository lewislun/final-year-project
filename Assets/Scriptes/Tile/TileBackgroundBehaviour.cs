using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBackgroundBehaviour : MonoBehaviour {

	#region Children Names --------------------------------------

	static string TOP_LEFT = "Top Left";
	static string TOP_RIGHT = "Top Right";
	static string BOTTOM_LEFT = "Bottom Left";
	static string BOTTOM_RIGHT = "Bottom RIght";

	#endregion


	#region Public Properities ----------------------------------

	Color _backgroundColor;
	public Color backgroundColor {
		set {
			_backgroundColor = value;
		}
		get {
			return _backgroundColor;
		}
	}

	#endregion


	#region Private Variables ---------------------------------- 


	#endregion


	#region MonoBehaviour Functions ----------------------------

	void Start() {

	}

	#endregion

	void ChangeBackgroundColor(Color color) {

	}

}
