using UnityEngine;
using System.Collections;

public class SetTargetCourse : MonoBehaviour {

	//touch
	RaycastHit hit;
	Ray ray;

	//target position
	public float targetX;
	public float targetZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//mouse move
		if (Input.GetMouseButtonDown(0)){
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		}

		//touch
		if (Input.touchCount > 0){
			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		}
		
		// ray hit test
		if (Physics.Raycast(ray, out hit)){
			if (hit.rigidbody != null && hit.rigidbody.tag == "Background"){
				targetX = hit.point.x;
				targetZ = hit.point.z;
			}
		}
	
	}
}
