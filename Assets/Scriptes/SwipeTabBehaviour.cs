using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeTabBehaviour : MonoBehaviour {

	[System.Serializable]
	public class TabInfo {
		public string tabName;
		//public int sortIndex;
		public GameObject tabContent;
		public GameObject tabButton;
		public float underlineLength;
		[HideInInspector]
		public int index;
	}


	#region Public Variables 

	public float speedBase = 10f;
	public float speedFactor = 10f;
	public float velocityToSwipe = 1.0f;
	public GameObject tabButtonUnderline;
	public List<TabInfo> tabInfo = new List<TabInfo>();

	#endregion


	#region Private Variables 

	private bool isSetParent = false;
	private GameObject content;
	private float currentMoveTarget = 0f;
	private bool isPrevTouching = false;
	private bool isPrevTouchingTab = false;
	private bool isTouchingTab = false;
	private bool isTouching = false;
	private int curTabIndex = 0;
	private Vector2 velocity;
	private Vector2 prevViewportPos;
	private int tabCount;

	#endregion


	#region MonoBehaviour Functions -------------------------------------------------------------------

	void Start() {
		content = transform.FindChild("Viewport").FindChild("Content").gameObject;
		if (!tabButtonUnderline)
			Debug.Log("Missing tabButtonUnderline (SwipeTabBehaviour)");
		tabCount = transform.FindChild("Tabs").childCount;
	}

	void LateUpdate () {

		//the reason of not initializing the tabs in Start() is that Unity cannot get the scretched RectTransform size of a disabled object
		if (!isSetParent) {

			int index = 0;

			foreach (TabInfo tab in tabInfo) { 
			
				//put in the scrollview content
				RectTransform rt = tab.tabContent.GetComponent<RectTransform>();
				Rect tempRect = rt.rect;
				tab.tabContent.transform.SetParent(content.transform);
				tab.tabContent.SetActive(true);
				tab.tabButton.SetActive(true);

				//set size
				Vector2 tempSize;
				tempSize.x = tempRect.width;
				tempSize.y = tempRect.height;
				rt.sizeDelta = tempSize;

				//set tab button text
				tab.tabButton.transform.FindChild("Text").GetComponent<Text>().text = tab.tabName;

				//Set index
				tab.index = index++;

				//set button on click
				tab.tabButton.GetComponent<Button>().onClick.AddListener( () => ChangeTab(tab.index) );
			}

			ChangeTab(curTabIndex);
			isSetParent = true;
		}
	}

	void Update() {
		//print(scrollRect.velocity);
		DetermineSlideDirection();
		//print(curTabIndex);
		//print(velocity);
		UnderlineFollowContent();
	}

	#endregion

	public void ChangeTab(int index) {
		curTabIndex = index;
		//currentMoveTarget = 0 - (transform.GetComponent<RectTransform>().rect.width * index);
	}

	void UnderlineFollowContent() {
		Vector3 tempPos = tabButtonUnderline.transform.position;
		float width = transform.GetComponent<RectTransform>().rect.width;

		tempPos.x = tabInfo[curTabIndex].tabButton.transform.position.x + ((0 - content.transform.localPosition.x) - (curTabIndex * width)) / width * tabInfo[curTabIndex].tabButton.GetComponent<RectTransform>().sizeDelta.x;

		print(tempPos.x);

		tabButtonUnderline.transform.position = tempPos;
	}

	void ContentMoveToX(float x) {

		RectTransform rt = content.GetComponent<RectTransform>();
		float distance = x - rt.localPosition.x;
		float sign;

		if (distance == 0)
			sign = 0;
		else
			sign = (distance / Mathf.Abs(distance));

		float moveDistance = (distance * speedFactor + speedBase * sign) * Time.deltaTime;
		if (rt.localPosition.x < x && rt.localPosition.x + moveDistance > x || rt.localPosition.x > x && rt.localPosition.x + moveDistance < x)
			moveDistance = x - rt.localPosition.x;

		Vector2 tempPos = rt.localPosition;
		tempPos.x += moveDistance;
		rt.localPosition = tempPos;
	}

	void DetermineSlideDirection() {
		//check if the user is touching the tab area
		isTouching = Input.GetMouseButton(0) || (Input.touchCount > 0);
		Vector2 touchPos = Vector2.zero;
		if (isTouching) {

			if (!isPrevTouching) {
				if (Input.GetMouseButton(0))
					touchPos = Input.mousePosition;
				else
					touchPos = Input.GetTouch(0).position;

				isTouchingTab = RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), touchPos);
			}

		}
		else {
			isTouchingTab = false;
			prevViewportPos = new Vector2(-999, -999);
		}

		//calculate the velocity
		if (isTouchingTab) {
			if (Input.GetMouseButton(0))
				touchPos = Input.mousePosition;
			else
				touchPos = Input.GetTouch(0).position;

			Vector2 viewportPos = Camera.main.ScreenToViewportPoint(touchPos);

			if (prevViewportPos.x != -999) 
				velocity = (viewportPos - prevViewportPos) / Time.deltaTime;
			else
				velocity = Vector2.zero;

			prevViewportPos = viewportPos;
		}

		//decide which way should it slide if there is no destination yet
		if (!isTouchingTab && isPrevTouchingTab) {

			if (velocity.x < 0 - velocityToSwipe && curTabIndex < tabCount - 1)
				curTabIndex++;
			else if (velocity.x > velocityToSwipe && curTabIndex > 0)
				curTabIndex--;
			else {
				RectTransform rt = content.GetComponent<RectTransform>();
				float width = transform.GetComponent<RectTransform>().rect.width;

				float curX = 0 - rt.localPosition.x;
				float temp = curX % width;

				if (temp < width / 2)
					curTabIndex = (int)(curX / width);
				else
					curTabIndex = (int)(curX / width) + 1;
			}

		}

		if (!isTouchingTab) {
			//ContentMoveToX(currentMoveTarget);
			ContentMoveToX(0 - (transform.GetComponent<RectTransform>().rect.width * curTabIndex));
		}

		isPrevTouching = isTouching;
		isPrevTouchingTab = isTouchingTab;

	}

}
