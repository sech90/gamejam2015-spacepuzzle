using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.Generic;

public enum Joint {FLAT=5, TRIANGLE=1, CIRCLE=2, SQUARE=3, EXAGON=4, HOLE_TRIANGLE=-1, HOLE_CIRCLE=-2, HOLE_SQUARE=-3, HOLE_EXAGON=-4};

public class PuzzlePiece : MonoBehaviour {

	public int HP = 3;
	public float Mass = 1.0f;
	public Joint LeftEdge = Joint.FLAT;
	public Joint FrontEdge = Joint.FLAT;
	public Joint RightEdge = Joint.FLAT;
	public Joint BackEdge = Joint.FLAT;

	private SpriteRenderer _spriteRend;
	private Draggable _drag;
	private BoxCollider2D _coll;
	private PuzzlePiece[] _pieces;
	private Joint[] _joints;
	private Ship _ship;
	private List<PuzzlePiece> _compatibleOverlappingPieces;
	private List<PuzzlePiece> _incompatibleOverlappingPieces;

	//0: left, 1: front, 2: right, 3: back

	void Start () {
		_drag = GetComponent<Draggable>();
		_coll = GetComponent<BoxCollider2D>();
		_spriteRend = GetComponent<SpriteRenderer>();
		_ship = GetComponent<Ship>();


		_pieces = new PuzzlePiece[4];
		_joints = new Joint[]{LeftEdge, FrontEdge, RightEdge, BackEdge};
		_compatibleOverlappingPieces = new List<PuzzlePiece>();
		_incompatibleOverlappingPieces = new List<PuzzlePiece>();
	}


	void Update(){
		if(_drag != null && _drag.IsDragging){

		}
	}

	void OnTriggerEnter2D(Collider2D coll){
		
		PuzzlePiece piece = coll.GetComponent<PuzzlePiece>();
		
		//not a ship piece
		if(piece == null || piece._ship == null)
			return;
		
		//player is dragging this piece towards the ship
		if(_drag != null && _drag.IsDragging == true){
			List<int[]> compatible = CompatibleEdges(piece);
			if(compatible.Count != 0){
				piece._spriteRend.color = Color.green;
				_compatibleOverlappingPieces.Add(piece);
			}
			else{
				piece._spriteRend.color = Color.red;
				_incompatibleOverlappingPieces.Add(piece);
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll){
		
		PuzzlePiece piece = coll.GetComponent<PuzzlePiece>();
		
		//not a ship piece
		if(piece == null || piece._ship == null)
			return;
		
		//player is dragging this piece towards the ship
		if(_drag != null && _drag.IsDragging == true){
			piece._spriteRend.color = Color.white;
			_compatibleOverlappingPieces.Remove(piece);
		}
	}

	public void SetDragging(bool isDrag){
		if(isDrag){
			_coll.isTrigger = true;
			_spriteRend.sortingOrder = _spriteRend.sortingOrder+1;
		}
		else{
			foreach(PuzzlePiece p in _compatibleOverlappingPieces)
				p._spriteRend.color = Color.white;
			foreach(PuzzlePiece p in _incompatibleOverlappingPieces)
				p._spriteRend.color = Color.white;

			_coll.isTrigger = false;
			_spriteRend.sortingOrder = _spriteRend.sortingOrder-1;

			if(_compatibleOverlappingPieces.Count == 1)
				Attach(_compatibleOverlappingPieces[0]);
		}
	}
	
	public List<int[]> CompatibleEdges(PuzzlePiece piece){
		List<int[]> edges = new List<int[]>();
		for(int i=0;i<4;i++){
			//only if free slot
			if(_pieces[i] == null){
				for(int j=0;j<4;j++){

					if(piece._pieces[j] == null && ((int)piece._joints[j] + (int)_joints[i]) == 0){
					//	Debug.Log("Compatible "+i+" "+j+": "+((int)_joints[i])+" "+((int)piece._joints[j]));
						edges.Add(new int[]{i,j});
					}
				}
			}
		}
		return edges;
	}

	public void Attach(PuzzlePiece piece){
		List<int[]> matches = CompatibleEdges(piece);
		if(matches.Count != 0){

			transform.parent = piece.transform;
			float newRotation = 0;
			Vector2 newPos = Vector2.zero;

			for(int i=0;i<matches.Count;i++)
				Debug.Log("Matches: "+matches[i][0]+" "+matches[i][1]);

			//MATCH 1
			if(matches.Count == 1){
				int[] info = matches[0];

				newPos = CalcNewPosition(info[1]);
				newRotation = CalcNewRotation(info, piece);
				_ship = piece._ship;
				_pieces[info[0]] = piece;
				piece._pieces[info[1]] = this;
			}

			transform.localPosition = newPos;
			transform.localRotation = Quaternion.Euler(0,0,newRotation);

			rigidbody2D.isKinematic = true;
			Destroy(_drag);


		}
	}

	private float CalcNewRotation(int[] info, PuzzlePiece piece){

		if(Mathf.Abs(info[0]-info[1]) == 2)	//no need to rotate because faces are opposite
			return 0;

		//same face, rotate 180 degrees
		if(info[0] == info[1])
			return 180;

		if(info[0] - info[1] == -1)
			return 90;

		return 270;
	}

	private Vector2 CalcNewPosition(int direction){
		float x=0,y=0;

		//place left
		if(direction == 0) x = -0.44f; 

		//place front
		else if(direction == 1) y = 0.44f;

		//place right
		else if(direction == 2) x = 0.44f;

		//place bottom
		else if(direction == 3) y = -0.44f;
		
		return new Vector2(x,y);
	}


}



















