using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour {

	#region Public Variables ------------------------------------

	public int touchPriority = 0;

	#endregion


	#region Public Properties ------------------------------------

	private bool _isTouching = false;
	public bool isTouching {
		get {
			return _isTouching;
		}
		private set {
			_isTouching = value;
		}
	}

	private bool _isTouchingUI = false;
	public bool isTouchingUI {
		get {
			return _isTouchingUI;
		}
		private set {
			_isTouchingUI = value;
		}
	}

	private Vector2 _touchPos = Vector2.zero;
	public Vector2 touchPos {
		get {
			return _touchPos;
		}
		private set {
			_touchPos = value;
		}
	}

	/*
	private GameObject _touchingTile = null;
	public GameObject touchingTile {
		get {
			return _touchingTile;
		}
		private set {
			_touchingTile = value;
		}
	}*/

	#endregion


	#region MonoBehaviour Functions ------------------------------

	void FixedUpdate() {
		GetTouchPos();
	}

	#endregion


	#region Touch Positions --------------------------------------

	void GetTouchPos() {

		isTouching = true;
		Vector2 tempPos = Vector2.zero;

		if (Input.touchCount > 0)
			tempPos = Input.GetTouch(0).position;
		else if (Input.GetMouseButton(0))
			tempPos = Input.mousePosition;
		else
			isTouching = false;

		if (isTouching)
			touchPos = Camera.main.ScreenToWorldPoint(tempPos);
		else
			touchPos = Vector2.zero;

		isTouchingUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0);
	}

	
	public static GameObject GetTouchingTile() {

		if (!GetInstance().isTouching || GetInstance().isTouchingUI) 
			return null;

		Vector2 touchPos = GetInstance().touchPos;

		RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
		if (hit && hit.collider.transform.parent.gameObject.tag == TagsManager.TILE)
			return hit.collider.transform.parent.gameObject;

		return null;
	}


	#endregion


	public static TouchManager GetInstance() {
		return GameObject.Find("Manager").GetComponent<TouchManager>();
	}
}
