using UnityEngine;
using System.Collections;

public class ShipMove : MonoBehaviour {

	public PlayerData PlayerScript;

	[HideInInspector] public float targetX;
	[HideInInspector] public float targetZ;
	
	float stepMove;

	void Start(){
		PlayerScript = GameObject.Find("_Main").GetComponent<PlayerData>();
		transform.position = PlayerScript.position; //new Vector3 (posX, 0, posZ);
	}

	void Update () {

		//broadcast own position if ship is "player"

		if (gameObject.tag == "Player") {
			PlayerScript.position = transform.position;
			//print ("pos: "+transform.position);
		}

		//move
		stepMove = PlayerScript.speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, new Vector3 (targetX, 0, targetZ), stepMove);
		
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
			//other.gameObject.transform.Find("WaypointMarker").gameObject.SetActive(false);
		}
	}
}
