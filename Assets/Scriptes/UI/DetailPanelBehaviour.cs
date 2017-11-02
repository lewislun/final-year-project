using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailPanelBehaviour : MonoBehaviour {

	#region Children Names -------------------

	public static string PANEL = "Panel";
	public static string PANEL_IMAGE = "Panel Image";
	public static string TITLE = "Title";

	#endregion


	#region Properties -----------------------

	public string title {
		get {
			if (titleText == null)
				titleText = transform.Find(PANEL).Find(TITLE).GetComponent<Text>();
			return titleText.text;
		}
		set {
			if (titleText == null)
				titleText = transform.Find(PANEL).Find(TITLE).GetComponent<Text>();
			titleText.text = value;
		}
	}

	public Sprite image {
		get {
			if (panelImage == null)
				panelImage = transform.Find(PANEL).Find(PANEL_IMAGE).GetComponent<Image>();
			return panelImage.sprite;
		}
		set {
			if (panelImage == null)
				panelImage = transform.Find(PANEL).Find(PANEL_IMAGE).GetComponent<Image>();
			panelImage.sprite = value;
		}
	}

	#endregion


	#region Private Variables ----------------

	CanvasFadeBehaviour mCanvasFadeBehaviour;
	Text titleText;
	Image panelImage;

	#endregion


	#region Panel Control --------------------

	public void Show(bool animated){
		if (mCanvasFadeBehaviour == null)
			mCanvasFadeBehaviour = GetComponent<CanvasFadeBehaviour>();
		mCanvasFadeBehaviour.Show(animated);
	}

	public void Hide(bool animated){
		if (mCanvasFadeBehaviour == null)
			mCanvasFadeBehaviour = GetComponent<CanvasFadeBehaviour>();
		mCanvasFadeBehaviour.Hide(animated);
	}

	#endregion


}
