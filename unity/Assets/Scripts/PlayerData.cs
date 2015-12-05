using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

	// player
	public Vector3 position;
	public float speed;
	public float rotationSpeed;
	public int score;
	public int muni;
	public bool canShoot;

	// debug
	public Canvas debugCanvas; 
	public string hitPos;

	public string id;
	public string playername;
	public string team;

	public string raycastMode = "waypoint";


	// Use this for initialization
	void Start () {
		id = PlayerPrefs.GetString("ID");
		playername = PlayerPrefs.GetString("PLAYER");
		team = PlayerPrefs.GetString("TEAM");
	}

	void Update(){

	}
}
