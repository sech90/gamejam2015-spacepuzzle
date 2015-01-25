using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Ship : MonoBehaviour {

	public AudioClip GameOver;
	public AudioClip[] Screams;
	public CanvasGroup GameOverScreen;

	private AudioSource _audios;

	public float RotationSpeed = 1.0f;
	public PuzzlePiece Bottom;
	public Image FuelBar;

	public int Hp = 5;
	public float MaxFuel = 100.0f;
	
	private List<Propeller> _propellers;
	private List<FuelTank> _fuelTanks;
	private float _totalMass;
	private PuzzlePiece _thisPiece;
	private float _fuel;
	private float[] _propellerForce;
	private int[] _propellerNumber;
	private List<List<Propeller>> haha;

	void Start () {
		_propellers = new List<Propeller>();
		_fuelTanks = new List<FuelTank>();
		_thisPiece = GetComponent<PuzzlePiece>();
		_thisPiece._rotationRelativeToShip = 0;
		_fuel = MaxFuel;
		_thisPiece.MakeAttachPoints();

		haha = new List<List<Propeller>>();
		haha.Add(new List<Propeller>());
		haha.Add(new List<Propeller>());
		haha.Add(new List<Propeller>());
		haha.Add(new List<Propeller>());

		Collider2D[] points = Physics2D.OverlapCircleAll(Bottom.transform.position,0.2f,LayerMask.GetMask("AttachPoint"));

		List<int[]> res = Bottom.Fits(points);
		Bottom.Attach(points,res);
		_audios = GetComponent<AudioSource>();


		UpdatePieces();

	}
	

	void Update () {
		//steer left
		if(Input.GetKey(KeyCode.A))
			//rigidbody2D.rotation = rigidbody2D.rotation + RotationSpeed*Time.deltaTime;
			transform.Rotate(Vector3.forward * RotationSpeed);

		//steer right
		if(Input.GetKey(KeyCode.D))
			//rigidbody2D.rotation = rigidbody2D.rotation - RotationSpeed*Time.deltaTime;
			transform.Rotate(Vector3.back * RotationSpeed);

		int isFiring = 0;

		if(Input.GetKey(KeyCode.Escape)){
			Application.LoadLevel("GUI");
		}

		//go forward
		if(Input.GetKey(KeyCode.W)){
			isFiring++;
			ApplyPropellerForce(1, Vector2.up);
		}
		//go backwards
		if(Input.GetKey(KeyCode.S)){
			isFiring++;
			ApplyPropellerForce(3, -Vector2.up);
		}
		//Steer left
		if(Input.GetKey(KeyCode.Q)){
			isFiring++;
			ApplyPropellerForce(0, -Vector2.right);
		}

		//Steer right
		if(Input.GetKey(KeyCode.E)){
			isFiring++;
			ApplyPropellerForce(2, Vector2.right);
		}


		if(Input.GetKeyUp(KeyCode.W)){

			foreach(Propeller p in haha[1])
				p.stopAnimation();
		}

		if(Input.GetKeyUp(KeyCode.S)){

			foreach(Propeller p in haha[3])
				p.stopAnimation();
		}

		if(Input.GetKeyUp(KeyCode.Q)){

			foreach(Propeller p in haha[0])
				p.stopAnimation();
		}

		if(Input.GetKeyUp(KeyCode.E)){

			foreach(Propeller p in haha[2])
				p.stopAnimation();
		}

		if(isFiring == 0 || _fuel <= 0)
			if(_audios.isPlaying)
				_audios.Stop();

		FuelBar.transform.localScale = new Vector3( Mathf.Clamp(_fuel/MaxFuel,0,100),1,1);

	}

	public void UpdatePieces(){
		_propellers.Clear();
		_fuelTanks.Clear();
		_propellerForce = new float[]{0,0,0,0};
		_propellerNumber = new int[]{0,0,0,0};
		haha[0].Clear();
		haha[1].Clear();
		haha[2].Clear();
		haha[3].Clear();

		InspectPiece(_thisPiece);

	}

	private void ApplyPropellerForce(int propNumber, Vector2 dir){
		if(_fuelTanks.Count > 0){
			_fuelTanks[0].Fuel -= _propellerNumber[propNumber] * Propeller.ConsumptionPerSec * Time.deltaTime;
			if(_fuelTanks[0].Fuel <= 0)
				_fuelTanks.RemoveAt(0);
		}
		
		else if(_fuel > 0){
			_fuel -= ((float)_propellerNumber[propNumber]) * Propeller.ConsumptionPerSec * Time.deltaTime;
		}

		if(_fuel > 0){
			if(!_audios.isPlaying)
				_audios.Play();

			foreach(Propeller p in haha[propNumber])
				p.Animate();
			rigidbody2D.AddRelativeForce(dir * (float)(_propellerForce[propNumber] / (0.5*_totalMass)));
		}
	}
	
	private void InspectPiece(PuzzlePiece piece){
		if(piece == null)
			return;

		_totalMass += piece.Mass;
		Propeller prop = piece.GetComponent<Propeller>();
		FuelTank tank = piece.GetComponent<FuelTank>();
		if(prop != null){
			_propellers.Add(prop);


			//propeller facing up
			if(((int)piece._rotationRelativeToShip) == 0){
				haha[1].Add(prop);
				_propellerForce[1] += prop.ForcePower;
				_propellerNumber[1]++;
				Debug.Log("propeller "+prop.name+" facing up: "+piece._rotationRelativeToShip);
			}
			//propeller facing left
			else if(((int)piece._rotationRelativeToShip) == 90){
				haha[0].Add(prop);
				_propellerForce[0] += prop.ForcePower;
				_propellerNumber[0]++;
				Debug.Log("propeller "+prop.name+" facing left: "+piece._rotationRelativeToShip);
			}

			//propeller facing down
			else if(((int)piece._rotationRelativeToShip) == 180){
				haha[3].Add(prop);
				_propellerForce[3] += prop.ForcePower;
				_propellerNumber[3]++;
				Debug.Log("propeller "+prop.name+" facing down: "+piece._rotationRelativeToShip);
			}

			//propeller facing right
			else{
				haha[2].Add(prop);
				_propellerNumber[2]++;
				_propellerForce[2] += prop.ForcePower;
				Debug.Log("propeller "+prop.name+" facing right: "+piece._rotationRelativeToShip);
			}

		}
		if(tank != null && tank.Fuel != 0){

			if(_fuel < MaxFuel){
				_fuel += tank.Fuel;
				if(MaxFuel - _fuel > 0)
					tank.Fuel = 0;
				else{
					tank.Fuel = _fuel-MaxFuel;
					_fuel = MaxFuel;
					_fuelTanks.Add(tank);
				}
			}
			else 
				_fuelTanks.Add(tank);
		}

		PuzzlePiece[] children = piece.transform.GetComponentsInChildren<PuzzlePiece>();
		//Debug.Log("Inspect children of "+piece.name+": "+children.Length);
		foreach(PuzzlePiece p in children)
			if(p != piece)
				InspectPiece(p);
	}
}
