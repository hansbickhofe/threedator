using UnityEngine;
using System.Collections;

public class Munition : MonoBehaviour {

	public TDSocketIO SocketScript; 
	public Game GameScript;


	public float rotSpeed;
	Vector3 spawnPosition;
	public int ID;

	// Use this for initialization
	void Start () {
		SocketScript = GameObject.Find("_Main").GetComponent<TDSocketIO>();
		GameScript = GameObject.Find("_Main").GetComponent<Game>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.World);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player"){ // Player ist immer nur der eigene player
			SocketScript.SendPickupJsonData(ID); //send pickup with muni ID
			gameObject.SetActive(false);
		}
	}
}
