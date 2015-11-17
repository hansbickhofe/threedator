using UnityEngine;
using System.Collections;

public class PlayPause : MonoBehaviour {

	Animator myAnimator;

	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey("space")){
			myAnimator.SetBool("isMoving", true);
		} else {
			myAnimator.SetBool("isMoving", false);
		}
	}
}
