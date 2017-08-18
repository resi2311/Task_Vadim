using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private Vector3 offset;  // Направление на камеру
	public GameObject Ball;

	// Use this for initialization
	void Start () 
	{
		offset = transform.position - Ball.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		transform.position = Ball.transform.position + offset;  // Обновление позиции камеры
	}
}
