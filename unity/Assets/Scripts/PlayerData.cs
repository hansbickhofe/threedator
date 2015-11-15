using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

	public Vector3 position;
	public float speed;
	public float rotationSpeed;
	public int score;
	public int shots;

	// debug
	public string clickText;
	public string touchText;
	public string hitPos;

	public string id;
	public string playername;
	public string team;

	public Camera DefaultCam;
	public Camera[] Cameras = new Camera[3];

	// Use this for initialization
	void Start () {
		id = PlayerPrefs.GetString("ID");
		playername = PlayerPrefs.GetString("PLAYER");
		team = PlayerPrefs.GetString("TEAM");

		if (team == "red") SetCam(0);
		else if (team == "green")SetCam(1);
		else if (team == "blue")SetCam(2);
	}

	void SetCam(int camID){
		DefaultCam.enabled = false;
		for (int i = 0; i<3; i++){
			if (i == camID) Cameras[i].enabled = true;
			else Cameras[i].enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
