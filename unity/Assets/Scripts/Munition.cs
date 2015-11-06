using UnityEngine;
using System.Collections;

public class Munition : MonoBehaviour {

	public Game GameScript;
	public PlayerData PlayerScript;

	public float speed;
	Vector3 spawnPosition;

	// Use this for initialization
	void Start () {
		GameScript = GameObject.Find("_Main").GetComponent<Game>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player"){
			//call server for new position
			spawnPosition = new Vector3(Random.Range(-GameScript.gameWidth,GameScript.gameWidth),.5f,Random.Range(-GameScript.gameHeight,GameScript.gameHeight));
			transform.position = spawnPosition;
		}
	}
}
