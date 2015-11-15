using UnityEngine;
using System.Collections;

public class TorpedoMove : MonoBehaviour {

	//scripts
	public PlayerData PlayerScript;

	public float speed;
	public float rotationSpeed;

	public string id;
	public float posX;
	public float posZ;
	public float targetX;
	public float targetZ;

//	[HideInInspector] public float posX;
//	[HideInInspector] public float posZ;
//	[HideInInspector] public float targetX;
//	[HideInInspector] public float targetZ;
	
	float stepMove;

	void Start(){
		PlayerScript = GameObject.Find("_Main").GetComponent<PlayerData>();
		transform.position = new Vector3 (posX, 0, posZ);
	}

	void Update () {
		print("move");

		if (gameObject.activeSelf == true) {
			// eigene position aktualisieren um sie später am server zu updaten
			posX = transform.position.x;
			posZ = transform.position.z;
		}

		//move
		stepMove = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, new Vector3 (targetX, 0, targetZ), stepMove);
		
//		//rotate
		Vector3 targetDir = new Vector3 (targetX, 0, targetZ) - transform.position;
		float stepRotate = rotationSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, stepRotate, 0.0F);
		transform.rotation = Quaternion.LookRotation (newDir);

		//freeze x and z rotation
		transform.eulerAngles = new Vector3(0f,transform.eulerAngles.y,0f);
	}

	void OnTriggerEnter(Collider other) {
		//enemy hit
		if (other.tag == "Player" && PlayerScript.id != id){ // nur treffer durch fremde torpedo id's auswerten
			ShipHit();
		}

		if (other.tag == "Targetpoint"){
			other.gameObject.transform.Find("Marker").gameObject.SetActive(false);
			gameObject.SetActive(false);
		}
	}

	void ShipHit(){

	}
}
