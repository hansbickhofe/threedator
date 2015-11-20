using UnityEngine;
using System.Collections;

public class TorpedoMove : MonoBehaviour {

	//scripts
	public TDSocketIO SocketScript;
	public PlayerData PlayerScript;

	public GameObject TorpedoObject;

	public float speed;
	public float rotationSpeed;

	public string id;
	public string torpedoStatus;
	[HideInInspector] public float posX;
	[HideInInspector] public float posZ;
	[HideInInspector] public float targetX;
	[HideInInspector] public float targetZ;
	
	float stepMove;

	void Start(){
		PlayerScript = GameObject.Find("_Main").GetComponent<PlayerData>();
		SocketScript = GameObject.Find("_Main").GetComponent<TDSocketIO>();
	}

	public void SetStartPosition(){
		TorpedoObject.SetActive(true);
		transform.position = new Vector3(posX,0.25f,posZ);
	}

	void Update () {
		if (TorpedoObject.activeSelf == true) {
			// torpedo position aktualisieren um sie später am server zu updaten
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
		//got hit
		if (other.tag == "Player" && PlayerScript.id != id){
			// nur treffer durch fremde torpedo id's auswerten
			TorpedoObject.SetActive(false);
			ShipHit();
		}

		if (other.tag == "Emeny" && id == PlayerScript.id){
			// nur treffer durch fremde torpedo id's auswerten
			TorpedoObject.SetActive(false);
			PlayerScript.canShoot = true;
		}

		if (other.tag == "Targetpoint"){ 
			other.gameObject.transform.Find("Marker").gameObject.SetActive(false);
			TorpedoObject.SetActive(false);
			WaterHit();
		}
	}

	void ShipHit(){
		SocketScript.SendGotHit(id);
	}

	void WaterHit(){
		SocketScript.SendWater(id);
		PlayerScript.canShoot = true;
	}

	public void HideTorpedo(){
		TorpedoObject.SetActive(false);
	}

}
