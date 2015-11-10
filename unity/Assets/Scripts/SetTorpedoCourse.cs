using UnityEngine;
using System.Collections;

public class SetTorpedoCourse : MonoBehaviour {

	//touch
	RaycastHit hit;
	Ray ray;

	//target position
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
	}
	
	// Update is called once per frame
	void Update () {

		//timer

		//mouse click
		if (Input.GetMouseButtonDown(1)){ // right mouse button
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
			}
		}
	
	}
}
