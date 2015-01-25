using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

	void OnCollisionExit2D(Collision2D coll){

		Debug.Log(coll.collider.name+" collided");


		Destroy(coll.gameObject);
		TestScript._actualNumber--;

	}
}
