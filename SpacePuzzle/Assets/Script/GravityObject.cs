using UnityEngine;
using System.Collections;

public class GravityObject : MonoBehaviour {

	private GravityAttractor _attractor;


	void Start () {
		_attractor = FindObjectOfType<GravityAttractor>();
	}

	void Update () {
		float gravity = _attractor.Gravity;

		Vector2 distance = (Vector2)(_attractor.transform.position - transform.position);
		rigidbody2D.velocity = rigidbody2D.velocity + (gravity * Time.fixedDeltaTime * distance);
//		rigidbody2D.AddForceAtPosition (-Vector2.up, new Vector2(0,0));
		/*
		 * this function calculates the circular motion of the object depending by the gravity of the attractor;
 		for semplicity we can forget about gravity and use a fixed radius instead, and all the objects move in circular
 		motion around the attractor
		 */
	}
}
