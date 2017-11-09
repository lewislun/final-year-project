using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelProgress {

	static string PROGRESS_KEY = "levelProgress";

	public static int GetProgress(){
		if (!PlayerPrefs.HasKey(PROGRESS_KEY)){
			return 0;
		} else {
			return PlayerPrefs.GetInt(PROGRESS_KEY);
		}
	}

	public static void SetProgress(int level){
		PlayerPrefs.SetInt(PROGRESS_KEY, level);
	}

}