using UnityEngine;
using System.Collections;

public class SetTorpedoCourse : MonoBehaviour {

	//scipts
	public TDSocketIO SocketScript;
	public PlayerData PlayerScript;

	//touch
	RaycastHit hit;
	Ray ray;

	//target position
	public GameObject Targetpoint;
	public float targetX;
	public float targetZ;

	//new shot when last torpedo is gone
	bool canShoot;
	public float waitTime; // 3.0f
	float time;
	bool newCourse;

	// Use this for initialization
	void Start () {
		PlayerScript.canShoot = true;
		time = 3;
		Targetpoint.transform.Find("Marker").gameObject.SetActive(false); // real torpedo point
	}
	
	// Update is called once per frame
	void Update () {

		//timer
		newCourse = false;
		if (PlayerScript.raycastMode == "torpedo") time -= Time.deltaTime;

		//check mode and scale floating target down
		if (time >= 0 && PlayerScript.raycastMode == "torpedo") {

		}

		//SetNewTorpedoCourse
		if (time <= 0 && newCourse == false){
			PlayerScript.muni--;
			newCourse = true;
		}

		//ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//mouse click
		if (Input.GetMouseButtonDown(1)){ // right mouse button
			if (PlayerScript.canShoot == true && PlayerScript.muni > 0) {
				PlayerScript.canShoot = false;
				PlayerScript.muni--;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			}
		}

		//touch
//		if (Input.touchCount > 0){
//			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
//		}
		
		// ray hit test
		if (Physics.Raycast(ray, out hit)){
			if (hit.rigidbody != null && hit.rigidbody.tag == "Background"){
				targetX = hit.point.x;
				targetZ = hit.point.z;

				//set visible waypoint
				Targetpoint.transform.Find("Marker").gameObject.SetActive(true);
				Targetpoint.transform.position = new Vector3(targetX,.1f,targetZ);
				
				//create initial torpedo target
				SocketScript.SetTorpedoTarget(Targetpoint.transform.position);
			}
		}
	}
}
