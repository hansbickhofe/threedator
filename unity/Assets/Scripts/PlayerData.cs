using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

	public string VRmode = "on";
	public Vector3 camPos0 =  new Vector3(0,10,-26);
	public Vector3 camPos1 =  new Vector3(-22,10,12.5f);
	public Vector3 camPos2 =  new Vector3(22,10,12.5f);

	public Vector3 camRot0 =  new Vector3(0,0,0);
	public Vector3 camRot1 =  new Vector3(0,120,0);
	public Vector3 camRot2 =  new Vector3(0,240,0);

	//vr head movement of other players
	public Vector3 redHeadRotation;
	public Vector3 greenHeadRotation;
	public Vector3 blueHeadRotation;

	// player
	public Vector3 position;
	public float speed;
	public float rotationSpeed;
	public int score;
	public int muni;
	public bool canShoot;

	// debug
	public Canvas debugCanvas; 
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
	public GameObject ScoreMsg;
	public GameObject HitMsg;
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
			debugCanvas.enabled = true;
			if (team == "red") SetCam(0);
			else if (team == "green")SetCam(1);
			else if (team == "blue")SetCam(2);
		} else if (VRmode == "on") {
			//hide debug canvas
			debugCanvas.enabled = false;

			// activate vr cam
			VRCam.SetActive(true);


			//vr-cam an spielerposition setzen
			if (team == "red") {
				VRCam.transform.position = camPos0;
				VRCam.transform.eulerAngles = camRot0;
				MuniMsg.transform.eulerAngles = camRot0;
				ScoreMsg.transform.eulerAngles = camRot0;
				HitMsg.transform.eulerAngles = camRot0;
			} else if (team == "green"){
				VRCam.transform.position = camPos1;
				VRCam.transform.eulerAngles = camRot1;
				MuniMsg.transform.eulerAngles = camRot1;
				ScoreMsg.transform.eulerAngles = camRot1;
				HitMsg.transform.eulerAngles = camRot1;
			} else if (team == "blue"){
				VRCam.transform.position = camPos2;
				VRCam.transform.eulerAngles = camRot2;
				MuniMsg.transform.eulerAngles = camRot2;
				ScoreMsg.transform.eulerAngles = camRot2;
				HitMsg.transform.eulerAngles = camRot2;
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
