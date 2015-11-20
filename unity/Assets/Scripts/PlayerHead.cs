using UnityEngine;
using System.Collections;

public class PlayerHead : MonoBehaviour {

	public PlayerData PlayerScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if (PlayerData.VRmode == "off"){
			transform.LookAt(PlayerScript.position);
	//	} else { 
			//transform.LookAt(PlayerScript.headRotation);
			//print ("ddd");
	//	}
	}
}
