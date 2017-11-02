using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailPanelBehaviour : MonoBehaviour {

	#region Children Names -------------------

	public static string PANEL = "Panel";
	public static string PANEL_IMAGE = "Panel Image";
	public static string PANEL_TITLE = "Title";
	public static string PANEL_WORD_LIST = "Word List";

	#endregion


	#region Properties -----------------------

	public string title {
		get {
			return titleText.text;
		}
		set {
			titleText.text = value;
		}
	}

	public Sprite image {
		get {
			return panelImage.sprite;
		}
		set {
			panelImage.enabled = value != null;
			panelImage.sprite = value;
		}
	}

	#endregion


	#region Private Variables ----------------

	CanvasFadeBehaviour mCanvasFadeBehaviour;
	Text titleText;
	Image panelImage;

	#endregion


	#region MonoBehaviour Functions ----------

	void Awake(){
		panelImage = transform.Find(PANEL).Find(PANEL_IMAGE).GetComponent<Image>();
		titleText = transform.Find(PANEL).Find(PANEL_TITLE).GetComponent<Text>();
		mCanvasFadeBehaviour = GetComponent<CanvasFadeBehaviour>();
	}

	#endregion


	#region Panel Control --------------------

	public void Show(bool animated){
		mCanvasFadeBehaviour.Show(animated);
	}

	public void Hide(bool animated){
		mCanvasFadeBehaviour.Hide(animated);
	}

	#endregion


}
