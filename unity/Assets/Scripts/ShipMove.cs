﻿using UnityEngine;
using System.Collections;

public class ShipMove : MonoBehaviour {

	public PlayerData PlayerScript;

	[HideInInspector] public float posX;
	[HideInInspector] public float posZ;
	[HideInInspector] public float targetX;
	[HideInInspector] public float targetZ;
	
	float stepMove;

	void Start(){
		PlayerScript = GameObject.Find("_Main").GetComponent<PlayerData>();
		transform.position = new Vector3 (posX, 0, posZ);
	}

	void Update () {
		//move
		stepMove = PlayerScript.speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, new Vector3 (targetX, 0, targetZ), stepMove);
		
//		//rotate
		Vector3 targetDir = new Vector3 (targetX, 0, targetZ) - transform.position;
		float stepRotate = PlayerScript.rotationSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, stepRotate, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);

		//freeze x and z rotation
		transform.eulerAngles = new Vector3(0f,transform.eulerAngles.y,0f);
	}
}