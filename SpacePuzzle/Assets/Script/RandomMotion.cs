using UnityEngine;
using System.Collections;



public class RandomMotion : MonoBehaviour
{
	public float period;
	public float radius;

	void Start()
	{
		m_centerPosition = transform.position;
		//period = Random.
	}
	
	void Update()
	{
		// Update degrees
		float dps = 360.0f / period; //angle in degree per second
		m_degrees = Mathf.Repeat(radius + (Time.deltaTime * dps), 360.0f);
		float radians = m_degrees * Mathf.Deg2Rad;
		
		// Offset on circle
		Vector3 offset = new Vector3(m_radius * Mathf.Cos(radians), m_radius * Mathf.Sin(radians), 0.0f);
		transform.position = m_centerPosition + offset;
	}
	
	Vector3 m_centerPosition;
	float m_degrees;
	
	[SerializeField]
	float m_radius = 1.0f;
	
	//float m_radius = Random.Range(10,20);
	
	[SerializeField]
	float m_period = 1.0f;
	
	//float m_period = Random.Range(10,20);
}



