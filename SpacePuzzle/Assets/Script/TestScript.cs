using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {


	public PuzzlePiece p1,p2;
	// Use this for initialization

	void Start () {
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ShipPiece"), LayerMask.NameToLayer("ShipPiece"));
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Return))
			p1.Attach(p2);
	}
}
