using UnityEngine;
using System.Collections;

public class Munition : MonoBehaviour {

	public TDSocketIO SocketScript; 
	public Game GameScript;


	public float rotSpeed;
	Vector3 spawnPosition;

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
			//call server for new position
			//spawnPosition = new Vector3(Random.Range(-GameScript.gameWidth,GameScript.gameWidth),.5f,Random.Range(-GameScript.gameHeight,GameScript.gameHeight));
			//transform.position = spawnPosition;
			SocketScript.SendMuniHit();
			gameObject.SetActive(false);
		}
	}
}
