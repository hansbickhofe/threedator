using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
	
	float speed = .5f;

	float xPos;
	float zPos;

	float dir;
	float rotSpeed = 5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//xPos = transform.position.x + speed * Input.GetAxis("Vertical");
		//zPos = transform.position.z + speed * Input.GetAxis("Horizontal");

		// rotation
		dir = transform.eulerAngles.y + rotSpeed * Input.GetAxis("Horizontal");
		transform.rotation = Quaternion.Euler(0f,dir,0f);

		// position
		transform.position += transform.forward * speed * Input.GetAxis("Vertical");

	}
}
