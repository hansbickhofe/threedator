using UnityEngine;
using System.Collections;

public class AstronautMove : MonoBehaviour {

	public Animator MyAnimator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) MyAnimator.SetBool("isMoving",true);
		if(Input.GetMouseButtonUp(0)) MyAnimator.SetBool("isMoving",false);
	}
}
