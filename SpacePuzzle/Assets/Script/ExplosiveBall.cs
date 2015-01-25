using UnityEngine;
using System.Collections;

public class ExplosiveBall : MonoBehaviour {
	
	public int Damage;
	private Animator _anim;
	public AudioClip CollisionSound; 

	void Start(){
		_anim = GetComponent<Animator>();
	}

	void OnCollisionEnter2D(Collision2D coll){


		PuzzlePiece collidedPiece = coll.collider.GetComponent<PuzzlePiece>();
		if(collidedPiece != null && collidedPiece._ship != null){
			

			collidedPiece.HP -= Damage;
			AudioSource.PlayClipAtPoint(CollisionSound,transform.position);
			if(collidedPiece.HP <= 0){
				Ship ship = collidedPiece._ship;
				collidedPiece.Explode();
				if(ship != null)
					ship.UpdatePieces();
			}
			Debug.Log("non esplodi!!!");
			_anim.SetTrigger("Explode");
			Destroy(gameObject,4.0f);
		}
	}
}
