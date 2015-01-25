using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Draggable : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;
	private PuzzlePiece _puzzle;
	public bool IsDragging = false;


	void Start(){
		_puzzle = GetComponent<PuzzlePiece>();
	}

	void OnMouseDown() {
		IsDragging = true;
		if(_puzzle != null)
			_puzzle.SetDragging(true);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
	
	void OnMouseDrag()
	{
		Debug.Log("Dragging "+name);
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;

	}

	void OnMouseUp(){
		IsDragging = false;
		if(_puzzle != null)
			_puzzle.SetDragging(false);

	}
}
