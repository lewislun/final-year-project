using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelContainerBehaviour : MonoBehaviour {

	public static LevelContainerBehaviour instance = null;


	#region Public Variables ----------------

	public GameObject levelItemPrefab = null;

	public Color tutorialLevelColor;
	public Color normalLevelColor;
	public Color timedLevelColor;
	public Color quizLevelColor;

	#endregion


	#region Private Variables ---------------

	List<GameObject> levelItems = new List<GameObject>();

	#endregion


	#region MonoBehaviour -------------------

	void Awake(){
		if (levelItemPrefab == null)
			Debug.Log("levelItemPrefab == null");
		instance = this;
	}

	#endregion


	#region Generate Level Items ------------

	void GenerateLevelItem(LevelInfo levelInfo, int levelIndex){
		GameObject newItem = Instantiate(levelItemPrefab, transform);

		newItem.transform.localScale = Vector3.one;
		newItem.transform.FindChild("Level Index").GetComponent<Text>().text = (levelIndex + 1) + "";
		ChangeLevelItemColor(newItem, levelInfo);
		AddLevelItemOnClick(newItem, levelIndex);

		levelItems.Add(newItem);
	}


	void ChangeLevelItemColor(GameObject levelItem, LevelInfo levelInfo){
		Image image = levelItem.GetComponent<Image>();
		if (levelInfo.isTutorial)
			image.color = tutorialLevelColor;
		else if (levelInfo.filteredWords.Length > 0) 
			image.color = quizLevelColor;
		else if (levelInfo.duration >= 0)
			image.color = timedLevelColor;
		else
			image.color = normalLevelColor;
	}

	void AddLevelItemOnClick(GameObject levelItem, int levelIndex){
		levelItem.GetComponent<Button>().onClick.AddListener(() => {
			GameController.GetInstance().StartLevel(levelIndex);
		});
	}

	#endregion


	#region Level Items Control --------------	

	public void InitContainer(){
		levelItems.Clear();
		for (int i = transform.childCount - 1; i >= 0; i--){
			Destroy(transform.GetChild(i).gameObject);
		}
	}

	public void GenerateAllLevels(){
		GenerateAllLevels(new List<LevelInfo>(GameController.GetInstance().levelSeries));
	}

	public void GenerateAllLevels(List<LevelInfo> levelInfos){
		InitContainer();
		for(int i = 0; i < levelInfos.Count; i++){
			GenerateLevelItem(levelInfos[i], i);
		}
		UpdateLevelProgress();
	}

	public void UpdateLevelProgress(){
		int progress = LevelProgress.GetProgress();
		for(int i = 0; i < levelItems.Count; i++){
			if (i <= progress) {
				levelItems[i].GetComponent<CanvasFadeBehaviour>().Show(false);
			} else {
				levelItems[i].GetComponent<CanvasFadeBehaviour>().Hide(false);
			}
		}
	}

	#endregion

	

}
