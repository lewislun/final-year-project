using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGlowingBorderBehaviour : MonoBehaviour {

	#region Children Names ------------------

	public static string TOP_LEFT_CORNER = "Top Left Corner";
	public static string TOP_RIGHT_CORNER = "Top Right Corner";
	public static string BOTTOM_LEFT_CORNER = "Bottom Left Corner";
	public static string BOTTOM_RIGHT_CORNER = "Bottom Right Corner";
	public static string TOP_EXTENSION = "Top Extension";
	public static string BOTTOM_EXTENSION = "Bottom Extension";
	public static string LEFT_EXTENSION = "Left Extension";
	public static string RIGHT_EXTENSION = "Right Extension";

	#endregion


	#region Public Variables ----------------

	public Color defaultColor;

	#endregion


	#region Private Variables ---------------

	private List<GameObject> borderParts = null;

	#endregion


	#region Properties ----------------------

	public Color color {
		get {
			if (borderParts == null)
				FindChildren();
			return borderParts[0].GetComponent<SpriteRenderer>().color;
		}
		set {
			if (borderParts == null)
				FindChildren();
			foreach (GameObject borderPart in borderParts) {
				borderPart.GetComponent<SpriteRenderer>().color = value;
			}
		}
	}

	#endregion


	#region MonoBehaviour -------------------

	void Awake () {
		if (borderParts == null)
			FindChildren();
		color = defaultColor;
	}

	#endregion


	void FindChildren() {
		borderParts = new List<GameObject>();

		borderParts.Add(transform.FindChild(TOP_LEFT_CORNER).gameObject);
		borderParts.Add(transform.FindChild(TOP_RIGHT_CORNER).gameObject);
		borderParts.Add(transform.FindChild(BOTTOM_LEFT_CORNER).gameObject);
		borderParts.Add(transform.FindChild(BOTTOM_RIGHT_CORNER).gameObject);
		borderParts.Add(transform.FindChild(TOP_EXTENSION).gameObject);
		borderParts.Add(transform.FindChild(BOTTOM_EXTENSION).gameObject);
		borderParts.Add(transform.FindChild(LEFT_EXTENSION).gameObject);
		borderParts.Add(transform.FindChild(RIGHT_EXTENSION).gameObject);
	}

	public void MergeBorder(bool shouldMergeLeft, bool shouldMergeRight, bool shouldMergeTop, bool shouldMergeBottom) {

		int[] flags = { 1, 1, 1, 1, 0, 0, 0, 0 };

		if (shouldMergeRight) {
			flags[4] += 1;
			flags[5] += 1;
			flags[1] -= 10;
			flags[3] -= 10;
			flags[7] -= 10;
		}
		if (shouldMergeBottom) {
			flags[6] += 1;
			flags[7] += 1;
			flags[2] -= 10;
			flags[3] -= 10;
			flags[5] -= 10;
		}
		if (shouldMergeTop) {
			flags[0] -= 10;
			flags[1] -= 10;
			flags[4] -= 10;
		}
		if (shouldMergeLeft) {
			flags[0] -= 10;
			flags[2] -= 10;
			flags[6] -= 10;
		}

		for (int i = 0; i < 8; i++) 
			borderParts[i].SetActive(flags[i] > 0);
		
	}

}
