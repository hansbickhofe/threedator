using UnityEngine;
using System.Collections;

public class SetTargetCourse : MonoBehaviour {

	//scripts
	public PlayerData PlayerScript;

	//touch
	RaycastHit hit;
	Ray ray;

	//target position
	public GameObject Waypoint;
	public GameObject FloatMarker;
	public float targetX;
	public float targetZ;

	//new course every three seconds 
	//bool canTouch; 
	public float waitTime; // 3.0f
	float time;
	bool newCourse;

	// Use this for initialization
	void Start () {
		//canTouch = true;
		time = 3;
		Waypoint.transform.Find("Marker").gameObject.SetActive(false); // real waypoint
	}
	
	void Update () {

		//timer
		newCourse = false;
		time -= Time.deltaTime;

		//check mode and scale floating target down
		if (time >= 0 && PlayerScript.raycastMode == "waypoint") {
			FloatMarker.SetActive (true);
			FloatMarker.transform.localScale = new Vector3 (time * 2f, .1f, time * 2f);
		} else {
			FloatMarker.SetActive (false);
		}
			
		
		//SetNewCourse
		if (time <= 0 && newCourse == false){
			newCourse = true;
		}

		//ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//mouse click
		if (Input.GetMouseButtonDown(0)){
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//debug
			PlayerScript.clickText = "click";
		} else {
			PlayerScript.clickText = "no click";
		}

		//touch
		if (Input.touchCount > 0){
			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			//debug
			PlayerScript.touchText = "touched";
		} else {
			PlayerScript.touchText = "no touch";
		}
		
		// ray hit test for touch click usw.
		if (Physics.Raycast(ray, out hit)){
			if (hit.rigidbody != null && hit.rigidbody.tag == "Background" && newCourse == true){
				targetX = hit.point.x;
				targetZ = hit.point.z;
			    newCourse = false;
			    time = 3;

				//set visible waypoint
				Waypoint.transform.Find("WMarker").gameObject.SetActive(true);
				Waypoint.transform.position = new Vector3(targetX,.1f,targetZ);
				print (Waypoint.transform.position);

				//debug
				PlayerScript.hitPos = targetX.ToString()+" "+targetZ.ToString();
			} else if (hit.rigidbody != null && hit.rigidbody.tag == "Background" && newCourse == false){
				//waypointmarker frei bewegen
				FloatMarker.transform.position = new Vector3(hit.point.x,.1f,hit.point.z);
			
			}
		}
	}
}
