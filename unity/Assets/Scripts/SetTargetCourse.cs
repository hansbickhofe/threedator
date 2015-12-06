using UnityEngine;
using System.Collections;

public class SetTargetCourse : MonoBehaviour {

	//scripts
	public TDSocketIO SocketScript;
	public PlayerData PlayerScript;

	//touch
	RaycastHit hit;
	Ray ray;
	public GameObject Waypoint;

	//new course every three seconds 
	//bool canTouch; 
	public float waitTime; // 3.0f
	float time;
	bool newCourse;

	// Use this for initialization
	void Start () {
		//canTouch = true;
		time = 3;
	}
	
	void Update () {

		//timer
		newCourse = false;
		time -= Time.deltaTime;
		
		//SetNewCourse
		if (time <= 0 && newCourse == false){
			newCourse = true;
		}

		//mouse click
		if (Input.GetMouseButtonDown(0) && newCourse == true){
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		}

		//touch
		if (Input.touchCount > 0 && newCourse == true){
			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		}
		
		// ray hit test for touch click usw.
		if (Physics.Raycast(ray, out hit)){
			if (hit.rigidbody != null && hit.rigidbody.tag == "Background"){
			    newCourse = false;
			    time = 3;

				//set visible waypoint
				Waypoint.transform.Find("Marker").gameObject.SetActive(true);
				Waypoint.transform.position = new Vector3(hit.point.x,.1f,hit.point.z);
				//print (Waypoint.transform.position);

				// send coordinates to server
				SocketScript.targetX = hit.point.x;
				SocketScript.targetZ = hit.point.z;
				SocketScript.SendPlayerJsonData();

				//debug
				//PlayerScript.hitPos = targetX.ToString()+" "+targetZ.ToString();
			}
		}
	}
}
