using UnityEngine;
using System.Collections;

public class PlayerHead : MonoBehaviour {

	public PlayerData PlayerScript;
	public string color;

	// Use this for initialization
	void Start () {
		if (color == "red") transform.eulerAngles = PlayerScript.camRot0;
		if (color == "green") transform.eulerAngles = PlayerScript.camRot1;
		if (color == "blue") transform.eulerAngles = PlayerScript.camRot2;

	}

	// Update is called once per frame
	void Update () {	
		if (PlayerScript.VRmode == "off"){
			transform.LookAt(PlayerScript.position);
		} else if (PlayerScript.VRmode == "on") {
			if (color == PlayerScript.team){ // eigenen kopf unsichtbar machen
				gameObject.SetActive(false);
			} else {
				if (color == "red") {
					iTween.RotateTo(gameObject, iTween.Hash("rotation",PlayerScript.redHeadRotation,"time",.2f));
					//transform.eulerAngles = PlayerScript.redHeadRotation;

				}
				if (color == "green") {
					//transform.eulerAngles = PlayerScript.greenHeadRotation;
					iTween.RotateTo(gameObject, iTween.Hash("rotation",PlayerScript.greenHeadRotation,"time",.2f));
				}


				if (color == "blue") {
					//transform.eulerAngles = PlayerScript.blueHeadRotation;
					iTween.RotateTo(gameObject, iTween.Hash("rotation",PlayerScript.blueHeadRotation,"time",.2f));
				}

			}
		}
	}
}
