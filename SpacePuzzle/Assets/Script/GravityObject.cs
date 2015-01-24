using UnityEngine;
using System.Collections;

public class GravityObject : MonoBehaviour {

	public float Mass;

	private GravityAttractor _attractor;


	void Start () {
		_attractor = FindObjectOfType<GravityAttractor>();
	}

	void Update () {
		/*
		 * this function calculates the circular motion of the object depending by the gravity of the attractor;
 		for semplicity we can forget about gravity and use a fixed radius instead, and all the objects move in circular
 		motion around the attractor
		 */
	}
}
