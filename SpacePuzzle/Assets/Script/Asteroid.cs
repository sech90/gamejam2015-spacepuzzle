using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	public AudioClip ImpactSound;
	public int Damage;
	public GameObject Sparkles;

	void OnCollisionEnter2D(Collision2D coll){

		PuzzlePiece collidedPiece = coll.collider.GetComponent<PuzzlePiece>();
		if(collidedPiece != null && collidedPiece._ship != null){
			ContactPoint2D cp = coll.contacts[0];

			AudioSource.PlayClipAtPoint(ImpactSound,transform.position);

			Destroy(Instantiate(Sparkles,(Vector3)cp.point,coll.transform.rotation),1.0f);
			collidedPiece.HP -= Damage;
			Ship s = collidedPiece._ship;
			if(s != null){
				int random = Random.Range(0,7);
				AudioSource.PlayClipAtPoint(s.Screams[random],transform.position);
			}

			if(collidedPiece.HP <= 0){
				Ship ship = collidedPiece._ship;
				collidedPiece.Explode();
				if(ship != null)
					ship.UpdatePieces();
			}
		}
	}
}
