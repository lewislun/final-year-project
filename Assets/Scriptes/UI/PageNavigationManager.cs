using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PageInfo {
	public string name;
	public GameObject pageCanvas;
	public UnityEvent pageConstructor;
	public UnityEvent pageDestructor;
}

public class PageNavigationManager : MonoBehaviour {

	#region Public Variables -------------------------------------------------

	public int homePage = 0;

	public List<PageInfo> pageInfos = new List<PageInfo>();
	public AnimationCurve fadeCurve;
	public float fadeDuration = 0.3f;

	#endregion


	#region Priavte Variables ------------------------------------------------

	private Stack<int> pageStack = new Stack<int>();
	private int curPage = -1;

	#endregion


	#region MonoBehaviour Functions ------------------------------------------

	void Awake() {
		DisableAllPage();
	}

	void Start() {
		ChangePage(homePage);
	}

	#endregion


	#region Page Navigation Functions ----------------------------------------

	public void ChangePage(string pageName) {
		for (int i = 0; i < pageInfos.Count; i++)
			if (pageInfos[i].name == pageName) {
				ChangePage(i);
				return;
			}
	}

	public void ChangePage(int pageIdx) {
		if (pageIdx < 0 || pageIdx >= pageInfos.Count) {
			Debug.Log("Page " + pageIdx + " does not exist");
			return;
		}

		UnmountPage(curPage);
		MountPage(pageIdx);

		if (curPage != -1)
			pageStack.Push(curPage);
		curPage = pageIdx;
	}

	void MountPage(int pageIdx) {

		pageInfos[pageIdx].pageCanvas.GetComponent<CanvasGroup>().interactable = true;
		pageInfos[pageIdx].pageCanvas.SetActive(true);
		FadeInPage(pageIdx);
		pageInfos[pageIdx].pageConstructor.Invoke();

	}

	void UnmountPage(int pageIdx) {
		if (pageIdx != -1) {
			pageInfos[pageIdx].pageDestructor.Invoke();
			if (pageInfos[pageIdx].pageCanvas) {
				FadeOutPage(pageIdx);
				pageInfos[pageIdx].pageCanvas.GetComponent<CanvasGroup>().interactable = false;
			}
		}
	}

	void DisableAllPage() {
		foreach (PageInfo pageInfo in pageInfos) {
			pageInfo.pageCanvas.SetActive(false);
		}
	}

	#endregion
	

	#region Page Navigation Visual Effect ------------------------------------

	void FadeInPage(int pageIdx) {
		if (pageIdx < 0 || pageIdx >= pageInfos.Count) {
			Debug.Log("Page " + pageIdx + " does not exist");
			return;
		}

		StartCoroutine(FadeCanvasGroup(pageInfos[pageIdx].pageCanvas, 0, 1, fadeDuration, true));
	}

	void FadeOutPage(int pageIdx) {
		if (pageIdx < 0 || pageIdx >= pageInfos.Count) {
			Debug.Log("Page " + pageIdx + " does not exist");
			return;
		}

		StartCoroutine(FadeCanvasGroup(pageInfos[pageIdx].pageCanvas, 1, 0, fadeDuration, false));
	}

	IEnumerator FadeCanvasGroup(GameObject go, float startAlpha, float endAlpha, float duration, bool endEnable) {
		CanvasGroup cg = go.GetComponent<CanvasGroup>();
		if (cg == null)
			yield break;
		float curTime = 0;
		while (curTime < duration) {
			float curAlpha = startAlpha + (endAlpha - startAlpha) * fadeCurve.Evaluate(curTime / duration);
			cg.alpha = curAlpha;
			yield return null;
			curTime += Time.deltaTime;
		}
		cg.alpha = endAlpha;
		go.SetActive(endEnable);
	}

	#endregion

	public static PageNavigationManager GetInstance() {
		return GameObject.Find("Manager").GetComponent<PageNavigationManager>();
	}
}
