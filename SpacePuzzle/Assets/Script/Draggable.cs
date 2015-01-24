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
		if(_puzzle != null)
			_puzzle.SetDragging(true);
		IsDragging = true;
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
	
	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;

	}

	void OnMouseUp(){
		if(_puzzle != null)
			_puzzle.SetDragging(false);
		IsDragging = false;
	}
}
