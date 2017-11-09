using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FontColor {
	public Color primary = new Color32(0xCA, 0xCA, 0xCA, 0xFF);
}

public class Config: MonoBehaviour{
	public FontColor fontColor = new FontColor();

	public static Config GetInstance(){
		return GameObject.Find("Manager").GetComponent<Config>();
	}
}


