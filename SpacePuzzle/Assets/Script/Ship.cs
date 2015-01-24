using UnityEngine;
using System.Collections.Generic;

public class Ship : MonoBehaviour {
	
	public float RotationSpeed = 1.0f;

	public int Hp = 5;
	public float MaxFuel = 100.0f;

	private List<Propeller> _propellers;
	private List<FuelTank> _fuelTanks;
	private float _totalMass;
	private float _totalSpeed;
	private PuzzlePiece _thisPiece;
	private float _fuel;
	private float _fuelInPieces = 0;
	private float[] _propellerForce;

	void Start () {
		_propellerForce = new float[4];
		_propellers = new List<Propeller>();
		_fuelTanks = new List<FuelTank>();
		_thisPiece = GetComponent<PuzzlePiece>();
		_fuel = MaxFuel;
		UpdatePieces();
	}
	

	void Update () {
		if(Input.GetKey(KeyCode.W)){


			rigidbody2D.AddRelativeForce(Vector2.up * _propellerForce[1]);
		}
		if(Input.GetKey(KeyCode.A))
			transform.Rotate(Vector3.forward * RotationSpeed);
		if(Input.GetKey(KeyCode.D))
			transform.Rotate(Vector3.back * RotationSpeed);
	}

	public void UpdatePieces(){
		_totalSpeed = 0;
		_propellers.Clear();
		_fuelTanks.Clear();
		InspectPiece(_thisPiece);

	}

	private void InspectPiece(PuzzlePiece piece){
		if(piece == null)
			return;

		_totalMass += piece.Mass;
		Propeller prop = piece.GetComponent<Propeller>();
		FuelTank tank = piece.GetComponent<FuelTank>();
		if(prop != null){
			_propellers.Add(prop);
			if(prop.transform.localRotation.eulerAngles.z == 0)
				_propellerForce[1] += prop.ForcePower;
			else if(prop.transform.localRotation.eulerAngles.z == 90)
				_propellerForce[0] += prop.ForcePower;
			else if(prop.transform.localRotation.eulerAngles.z == 90)
				_propellerForce[2] += prop.ForcePower;
			else
				_propellerForce[3] += prop.ForcePower;

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

		PuzzlePiece[] children = transform.GetComponentsInChildren<PuzzlePiece>();
		Debug.Log("Inspect children of "+name+" "+children.Length);
		foreach(PuzzlePiece p in children)
			if(p != piece)
				InspectPiece(p);

	}
}
