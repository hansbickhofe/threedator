using UnityEngine;
using System.Collections;

public class PlayerHead : MonoBehaviour {

	public PlayerData PlayerScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(PlayerScript.position);
	}
}
