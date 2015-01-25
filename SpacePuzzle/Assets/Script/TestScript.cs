using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {


	public GameObject[] PuzzlePieces;
	public GameObject[] Asteroids;
	public GameObject[] FuelTanks;
	public GameObject[] Propellers;


	public int MaxNumber = 200;
	public CircleCollider2D SpawnerRing;
	public CircleCollider2D DeathRing;

	public static int _actualNumber = 0;

	void Start () {
		_actualNumber = 0;
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ShipPiece"), LayerMask.NameToLayer("ShipPiece"));
	}

	void Update(){
		if(_actualNumber < MaxNumber){
			Spawn();
		}
	}

	public void Spawn(){
		int rand = (int)Random.Range(0,3);
		GameObject[] category;
		if(rand == 0)
			category = PuzzlePieces;
		else if(rand == 1)
			category = Asteroids;
		else if(rand == 2)
			category = FuelTanks;
		else
			category = Propellers;

		int index = (int)Random.Range(0,category.Length);
		Vector3 pos = Random.insideUnitCircle * (SpawnerRing.radius);

		if(Mathf.Abs(pos.x) < 1.0f || Mathf.Abs(pos.y) < 1.0f)
			return;

		Instantiate(category[index], pos,Quaternion.identity);
		_actualNumber++;
	}
	


}
