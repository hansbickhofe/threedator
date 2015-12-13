using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public PlayerData PlayerScript;
	public GameObject Cam;
	public float camHeight = 15f;
	public float camDist = -15f;

	[HideInInspector] public float targetX;
	[HideInInspector] public float targetZ;
	
	float stepMove;

	public bool isMoving = false;

	void Start(){
		Cam = GameObject.Find ("Camera");
		PlayerScript = GameObject.Find("_Main").GetComponent<PlayerData>();
		transform.position = PlayerScript.position; //new Vector3 (posX, 0, posZ);
	}

	void Update () {

		//broadcast own position if ship is "player"

		if (gameObject.tag == "Player") {
			Cam.transform.position = new Vector3(transform.position.x, camHeight, transform.position.z-camDist);
			PlayerScript.position = transform.position;
		}

		//move
		Vector3 targetPos = new Vector3 (targetX, 0, targetZ);
		stepMove = PlayerScript.speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, targetPos, stepMove);


		
//		//rotate
		Vector3 targetDir = new Vector3 (targetX, 0, targetZ) - transform.position;
		float stepRotate = PlayerScript.rotationSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, stepRotate, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);

		//freeze x and z rotation
		transform.eulerAngles = new Vector3(0f,transform.eulerAngles.y,0f);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Waypoint") {
			isMoving = false;
			other.gameObject.transform.Find("Marker").gameObject.SetActive(false);
		}
	}
}
