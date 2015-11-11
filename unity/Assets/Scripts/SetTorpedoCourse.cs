using UnityEngine;
using System.Collections;

public class SetTorpedoCourse : MonoBehaviour {

	//touch
	RaycastHit hit;
	Ray ray;

	//target position
	public GameObject Targetpoint;
	public float targetX;
	public float targetZ;

	//new course every three seconds 
	bool canShoot;
	float time;


	// Use this for initialization
	void Start () {
		canShoot = true;
		time = 0;
		Targetpoint.SetActive(false);
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

				//set visible waypoint
				Targetpoint.SetActive(true);
				Targetpoint.transform.position = new Vector3(targetX,.1f,targetZ);
			}
		}
	
	}
}
