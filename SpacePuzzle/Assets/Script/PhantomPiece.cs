using UnityEngine;
using System.Collections.Generic;

public class PhantomPiece : PuzzlePiece {



	void Start () {

	}

	public void Setup(PuzzlePiece ego, PuzzlePiece toTest, int[] info){

		Debug.Log("phantom start "+name);
		gameObject.layer = LayerMask.NameToLayer("ShipPiece");
		_coll = GetComponent<BoxCollider2D>();
		_spriteRend = GetComponent<SpriteRenderer>();
		
		_pieces = new PuzzlePiece[4];
		_joints = new Joint[]{LeftEdge, FrontEdge, RightEdge, BackEdge};


		_spriteRend.sprite = ego._spriteRend.sprite;
		_spriteRend.color = new Color(0,1,0,0.5f);
		_joints = ego._joints;
		TryAttach(toTest,info);
	}

	public new List<int[]> CompatibleEdges(PuzzlePiece piece){
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
	
	public void TryAttach(PuzzlePiece piece, int[] info){
			
			transform.parent = piece.transform;
			float newRotation = 0;
			Vector2 newPos = Vector2.zero;

			newPos = CalcNewPosition(info[1]);
			newRotation = CalcNewRotation(info);
			_pieces[info[0]] = piece;
			
			transform.localPosition = newPos;
			transform.localRotation = Quaternion.Euler(0,0,newRotation);

	}
}
