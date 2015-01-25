using UnityEngine;
using System;
using System.Collections.Generic;

public enum Joint {FLAT=5, TRIANGLE=1, CIRCLE=2, SQUARE=3, EXAGON=4, HOLE_TRIANGLE=-1, HOLE_CIRCLE=-2, HOLE_SQUARE=-3, HOLE_EXAGON=-4,NULL=0};
public enum Side {LEFT =0, FRONT=1, RIGHT=2, BOTTOM=3};
public class PuzzlePiece : MonoBehaviour {

	public AudioClip PuzzleAttached;

	public int HP = 3;
	public float Mass = 1.0f;
	public Joint LeftEdge = Joint.FLAT;
	public Joint FrontEdge = Joint.FLAT;
	public Joint RightEdge = Joint.FLAT;
	public Joint BackEdge = Joint.FLAT;

	[HideInInspector]
	public SpriteRenderer _spriteRend;
	private Draggable _drag;
	[HideInInspector]
	public BoxCollider2D _coll;
	[HideInInspector]
	public PuzzlePiece[] _pieces;
	[HideInInspector]
	public Joint[] _joints;
	public Ship _ship;
	private List<PuzzlePiece> _compatibleOverlappingPieces;
	private List<PuzzlePiece> _incompatibleOverlappingPieces;
	[HideInInspector]
	public float _rotationRelativeToShip = -1;
	[HideInInspector]
	public Animator _anim;

	private List<PhantomPiece> _phantoms;

	//0: left, 1: front, 2: right, 3: back

	void Start () {
		_drag = GetComponent<Draggable>();
		_coll = GetComponent<BoxCollider2D>();
		_spriteRend = GetComponent<SpriteRenderer>();
		_anim = GetComponent<Animator>();
		_ship = GetComponent<Ship>();

		_pieces = new PuzzlePiece[4];
		_joints = new Joint[]{LeftEdge, FrontEdge, RightEdge, BackEdge};
		_compatibleOverlappingPieces = new List<PuzzlePiece>();
		_incompatibleOverlappingPieces = new List<PuzzlePiece>();
		_phantoms = new List<PhantomPiece>();

	}


	public void Explode(){

		_anim.SetTrigger("Explode");

		PuzzlePiece[] children = GetComponentsInChildren<PuzzlePiece>();
		transform.parent = null;



		foreach(PuzzlePiece p in children){
			if(p != this)
				p.Explode();



		}
		if(GetComponent<Ship>() != null)
			Invoke("playAudioDeath",1.5f);
		else
			Destroy(this.gameObject,2.0f);

	}

	private void playAudioDeath(){
		Debug.Log("Game Over");
		_ship.GameOverScreen.alpha = 1;
		AudioSource.PlayClipAtPoint(_ship.GameOver,transform.position);
	}

	public void RemovePiece(PuzzlePiece piece){
		for(int i=0;i<4;i++){
			if(_pieces[i] == piece){
				_pieces[i] = null;
			}
		}
	}

	void Update(){
		if(_drag != null && _drag.IsDragging){
			Collider2D[] points = Physics2D.OverlapCircleAll(transform.position,0.2f,LayerMask.GetMask("AttachPoint"));
			if(points.Length == 0)
				_spriteRend.color = Color.white;

			else if(Fits(points) != null)
				_spriteRend.color = Color.green;
			else
				_spriteRend.color = Color.red;
		}
	}

	/*
	void OnTriggerEnter2D(Collider2D coll){
		
		OnTrigger(coll);
	}*/
	/*
	void OnTriggerExit2D(Collider2D coll){
		
		PuzzlePiece piece = coll.GetComponent<PuzzlePiece>();

		//not a ship piece
		if(piece == null || piece._ship == null)
			return;
		
		//player is dragging this piece
		if(_drag != null && _drag.IsDragging == true){
			piece._spriteRend.color = Color.white;
			_compatibleOverlappingPieces.Remove(piece);
		}
	}*/

	public void SetDragging(bool isDrag){
		if(isDrag){
			_coll.isTrigger = true;
			_spriteRend.sortingOrder = _spriteRend.sortingOrder+1;
			/*Vector2 c1 = new Vector2(transform.position.x + 0.34f, transform.position.y + 0.34f);
			Vector2 c2 = new Vector2(transform.position.x - 0.34f, transform.position.y - 0.34f);

			Collider2D[] colls = Physics2D.OverlapAreaAll(c1,c2, LayerMask.GetMask("ShipPiece"));
			for(int i=0;i<colls.Length;i++)
				OnTrigger(colls[i]);*/

		}
		else{
		/*	foreach(PuzzlePiece p in _compatibleOverlappingPieces)
				p._spriteRend.color = Color.white;
			foreach(PuzzlePiece p in _incompatibleOverlappingPieces)
				p._spriteRend.color = Color.white;
*/
			_spriteRend.color = Color.white;
			_coll.isTrigger = false;
			_spriteRend.sortingOrder = _spriteRend.sortingOrder-1;


			Collider2D[] points = Physics2D.OverlapCircleAll(transform.position,0.2f,LayerMask.GetMask("AttachPoint"));

			if(points != null && points.Length > 0){
				List<int[]> res = Fits(points);
				if(res != null)
					Attach (points,res);
			}


			/*if(_compatibleOverlappingPieces.Count == 1)
				Attach(_compatibleOverlappingPieces[0]);*/
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

	public void Attach(Collider2D[] colls, List<int[]> info){



		List<AttachPoint> points = new List<AttachPoint>();
		AttachPoint a;
		
		//get points out from collider
		foreach(Collider2D c in colls){
			a = c.GetComponent<AttachPoint>();
			points.Add(a);
			//_pieces[(int)a.ParentSideRelativeToShip] = a.ParentPiece;
		}
		

		float newRotation = CalcNewRotation(info[0]);
		transform.parent = points[0].ParentPiece.transform;
		Vector2 newPos = CalcNewPosition((int)points[0].ParentSideRelativeToShip);


			
		_ship = points[0].ParentPiece._ship;
		for(int i=0;i<points.Count;i++){
			//Debug.Log("Attach "+name+" to "+info[i][0]);
			_pieces[info[i][0]] = points[i].ParentPiece;
			points[i].ParentPiece._pieces[info[i][1]] = this;
		}

		
		transform.localPosition = newPos;
		transform.localRotation = Quaternion.Euler(0,0,newRotation);
		
		//calculate rotation offset relative to main ship
		CalcRotationRelativeToShip();
		
		rigidbody2D.mass = 0;
		rigidbody2D.isKinematic = true;
		gameObject.layer = LayerMask.NameToLayer("ShipPiece");
		Destroy(_drag);
		Destroy(rigidbody2D);

		AudioSource.PlayClipAtPoint(PuzzleAttached,transform.position);

		_ship.UpdatePieces();
		MakeAttachPoints();
		
		
		
	}

	public List<int[]> Fits(Collider2D[] colls){
		List<AttachPoint> points = new List<AttachPoint>();
		AttachPoint a;

		//get points out from collider
		foreach(Collider2D c in colls){
			a = c.GetComponent<AttachPoint>();
			points.Add(a);
		}

		Joint[] sequence = new Joint[]{Joint.NULL,Joint.NULL,Joint.NULL,Joint.NULL
		};

		for(int i=0;i<points.Count;i++){
			sequence[(int)points[i].ParentSideRelativeToShip] = points[i].RequiredJoint;
			bool hasIt = false;
			for(int j=0;j<4;j++)
				if(_joints[j] == points[i].RequiredJoint)
					hasIt = true;
			if(!hasIt)
				return null;
		}
		List<int[]> info = new List<int[]>() ;
		int degrees = 0;
		//per la lunghezza della sequenza
		for(int i=0;i<4;i++){
			int fitting = 0;
			//confronta con la sequenza del pezzo corrente
			for(int j=0;j<4;j++){
				//Debug.Log("sequence ["+i+","+j+"]: "+sequence[j]+" "+_joints[(j+i)%4]);
				//NULL SIGNIFICA che non c'e' attachpoint per quella positions
				if(sequence[j] == Joint.NULL || sequence[j] == _joints[(j+i)%4]){
					fitting++;
					if(sequence[j] != Joint.NULL)
						info.Add(new int[]{(j+i)%4,j});

				}
			}
			if(fitting == 4){
			//	Debug.Log("fitting dopo "+i+" iterazioni");
				return info;
			}
		}

		return null;

	}

	/*

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
				newRotation = CalcNewRotation(info);
				_ship = piece._ship;
				_pieces[info[0]] = piece;
				piece._pieces[info[1]] = this;
			}

			transform.localPosition = newPos;
			transform.localRotation = Quaternion.Euler(0,0,newRotation);

			//calculate rotation offset relative to main ship
			CalcRotationRelativeToShip();

			rigidbody2D.mass = 0;
			rigidbody2D.isKinematic = true;
			gameObject.layer = LayerMask.NameToLayer("ShipPiece");
			Destroy(_drag);
			Destroy(rigidbody2D);

			_ship.UpdatePieces();


		}
	}
*/
	public void MakeAttachPoints(){
	

		for(int i=0;i<_pieces.Length;i++){

			if(_pieces[i] != null)
				Debug.Log("Make points " +i+" "+_pieces[i].name);
			if(_pieces[i] == null && _joints[i] != Joint.FLAT){



				Vector2 pointCenter = Vector2.zero;
				//left
				if(i == 0) pointCenter.x = -0.64f;
				//front
				else if(i==1) pointCenter.y = 0.64f;
				//right
				else if(i==2) pointCenter.x = 0.64f;
				//bottom
				else pointCenter.y = -0.64f;

				GameObject inst = Instantiate(Resources.Load("AttachPoint", typeof(GameObject))) as GameObject;
				AttachPoint attach = inst.GetComponent<AttachPoint>();
				attach.transform.parent = transform;
				attach.transform.localPosition = pointCenter;
				attach.ParentPiece = this;
				attach.RequiredJoint = (Joint)((int)(_joints[i])*(-1));

				int side = (i*90) - (int)_rotationRelativeToShip; 
				if(side < 0 )
					side += 360;

				if(side == 0)
					attach.ParentSideRelativeToShip = Side.LEFT;
				else if(side == 90)
					attach.ParentSideRelativeToShip = Side.FRONT;
				else if(side == 180)
					attach.ParentSideRelativeToShip = Side.RIGHT;
				else if(side == 270)
					attach.ParentSideRelativeToShip = Side.BOTTOM;

			}
		}
	}

/*	private void OnTrigger(Collider2D coll){
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
				for(int i = 0;i<compatible.Count; i++){
					GameObject inst = Instantiate(Resources.Load("PhantomPiece", typeof(GameObject))) as GameObject;
					PhantomPiece phantom = inst.GetComponent<PhantomPiece>();
					phantom.Setup(this,piece,compatible[i]);

					_phantoms.Add(phantom);
				}
			}
			else{
				piece._spriteRend.color = Color.red;
				_incompatibleOverlappingPieces.Add(piece);
			}
		}
	}*/

	protected void CalcRotationRelativeToShip(){
		float pieceRotation = transform.rotation.eulerAngles.z;
		float shipRotation = _ship.transform.rotation.eulerAngles.z;

		_rotationRelativeToShip = pieceRotation - shipRotation;
	}

	protected float CalcNewRotation(int[] info){

		if(Mathf.Abs(info[0]-info[1]) == 2)	//no need to rotate because faces are opposite
			return 0;

		//same face, rotate 180 degrees
		if(info[0] == info[1])
			return 180;

		if(info[0] - info[1] == -1)
			return 90;

		return 270;
	}

	protected Vector2 CalcNewPosition(int direction){
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



















