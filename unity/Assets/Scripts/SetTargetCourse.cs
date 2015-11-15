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
	public float targetX;
	public float targetZ;

	//new course every three seconds 
	bool canTouch; 
	public float waitTime; // 3.0f
	float time;


	// Use this for initialization
	void Start () {
		canTouch = true;
		time = 0;
		Waypoint.transform.Find("Marker").gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {

		//timer

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
		
		// ray hit test
		if (Physics.Raycast(ray, out hit)){
			if (hit.rigidbody != null && hit.rigidbody.tag == "Background"){
				targetX = hit.point.x;
				targetZ = hit.point.z;

				//set visible waypoint
				Waypoint.transform.Find("Marker").gameObject.SetActive(true);
				Waypoint.transform.position = new Vector3(targetX,.1f,targetZ);

				//debug
				PlayerScript.hitPos = targetX.ToString()+" "+targetZ.ToString();
			}
		}
	}
}
