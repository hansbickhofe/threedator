using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

	public Vector3 position;
	public float speed;
	public float rotationSpeed;
	public int score;
	public int shots;

	public string id;
	public string playername;
	public string team;

	public Camera DefaultCam;
	public Camera Cam0;
	public Camera Cam1;
	public Camera Cam2;

	// Use this for initialization
	void Start () {
		id = PlayerPrefs.GetString("ID");
		playername = PlayerPrefs.GetString("PLAYER");
		team = PlayerPrefs.GetString("TEAM");

		if (team == "red") {
			DefaultCam.enabled = false;
			Cam0.enabled = true;
			Cam1.enabled = false;
			Cam2.enabled = false;
		} else if (team == "green") {
			DefaultCam.enabled = false;
			Cam0.enabled = true;
			Cam1.enabled = true;
			Cam2.enabled = false;
		} else if (team == "blue") {
			DefaultCam.enabled = false;
			Cam0.enabled = false;
			Cam1.enabled = false;
			Cam2.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
