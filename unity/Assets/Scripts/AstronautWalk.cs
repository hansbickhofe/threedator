using UnityEngine;
using System.Collections;

public class AstronautWalk : MonoBehaviour {

	Move MoveScript;
	public Animator MyAnimator;

	// Use this for initialization
	void Start () {
		MoveScript = transform.parent.GetComponent<Move> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (MoveScript.isMoving) MyAnimator.SetBool("isMoving",true);
		else MyAnimator.SetBool("isMoving",false);
	}
}
