using UnityEngine;
using System.Collections;

public class Munition : MonoBehaviour {

	public TDSocketIO SocketScript;
	public Game GameScript;
	public PlayerData PlayerScript;


	public float rotSpeed;
	Vector3 spawnPosition;
	public int ID;

	// Use this for initialization
	void Start () {
		SocketScript = GameObject.Find("_Main").GetComponent<TDSocketIO>();
		GameScript = GameObject.Find("_Main").GetComponent<Game>();
		PlayerScript = GameObject.Find("_Main").GetComponent<PlayerData>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.World);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && PlayerScript.muni < 3){ // Player ist immer nur der eigene player
			SocketScript.SendPickupJsonData(ID); //send pickup with muni ID
			gameObject.SetActive(false);
		}

		if (other.tag == "Enemy"){ // Player ist immer nur der eigene player
			gameObject.SetActive(false);
		}
	}
}
