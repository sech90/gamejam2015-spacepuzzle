using UnityEngine;
using System.Collections;

public class Propeller : MonoBehaviour {

	public float ForcePower = 5.0f;
	public static float ConsumptionPerSec = 10.0f;


	private PuzzlePiece _thisPiece;
	// Use this for initialization
	void Start () {
		_thisPiece = GetComponent<PuzzlePiece>();
	}

	public void Animate(){
		_thisPiece._anim.SetBool("Propelling",true);
	}

	public void stopAnimation(){
		_thisPiece._anim.SetBool("Propelling",false);
	}
}
