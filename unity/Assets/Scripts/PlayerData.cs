using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

	public string VRmode;
	Vector3 camPos0 =  new Vector3(0,9,-22);
	Vector3 camPos1 =  new Vector3(-22,9,12);
	Vector3 camPos2 =  new Vector3(22,9,12);

	Vector3 camRot0 =  new Vector3(0,0,0);
	Vector3 camRot1 =  new Vector3(0,120,0);
	Vector3 camRot2 =  new Vector3(0,240,0);

	//vr head movement
	public Vector3 headRotation;

	// player
	public Vector3 position;
	public float speed;
	public float rotationSpeed;
	public int score;
	public int muni;
	public bool canShoot;

	// debug
	public GameObject debugCanvas; 
	public string clickText;
	public string touchText;
	public string hitPos;

	public string id;
	public string playername;
	public string team;

	// vr
	public GameObject VRCam;
	public GameObject VRCamHead;
	public GameObject FireCube;
	public GameObject MuniMsg;
	public Camera[] Cameras = new Camera[3];
	public string raycastMode = "waypoint";


	// Use this for initialization
	void Start () {
		id = PlayerPrefs.GetString("ID");
		playername = PlayerPrefs.GetString("PLAYER");
		team = PlayerPrefs.GetString("TEAM");

		// set cam for vr mode
		VRmode = PlayerPrefs.GetString("VR");

		if (VRmode == "") VRmode = "on"; // VR is default

		if (VRmode == "off"){
			debugCanvas.SetActive(true);
			if (team == "red") SetCam(0);
			else if (team == "green")SetCam(1);
			else if (team == "blue")SetCam(2);
		} else {
			//hide debug canvas
			debugCanvas.SetActive(false);
			// activate vr cam
			VRCam.SetActive(true);

			if (team == "red") {
				VRCam.transform.position = camPos0;
				VRCam.transform.eulerAngles = camRot0;
			} else if (team == "green"){
				VRCam.transform.position = camPos1;
				VRCam.transform.eulerAngles = camRot1;
			} else if (team == "blue"){
				VRCam.transform.position = camPos2;
				VRCam.transform.eulerAngles = camRot2;
			}
		}
	}

	void SetCam(int camID){
		print ("non vr cam");
		VRCam.SetActive(false);
		for (int i = 0; i<3; i++){
			if (i == camID) Cameras[i].enabled = true;
			else Cameras[i].enabled = false;
		}
	}

	void Update(){
		UpdateMuniCube();
	}

	void UpdateMuniCube(){
		if (muni == 0) {
			FireCube.SetActive (false);
			MuniMsg.SetActive (true);
			MuniMsg.GetComponent<TextMesh> ().text = "collect\nammo!!!";
		} else if (muni > 0 && muni < 3) {
			FireCube.SetActive (true);
			MuniMsg.SetActive (false);
		} else if (muni == 3) {
			MuniMsg.SetActive (true);
			MuniMsg.GetComponent<TextMesh> ().text = "ammo___\nfull___";
		}
	}
}
