using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public GameObject DebugTxt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleGameData(){
		if (DebugTxt.activeSelf) {
			DebugTxt.SetActive (false);
			print ("hide");
		} else {
			DebugTxt.SetActive (true);
			print ("show");
		}
	}

	public void EnterSettingsMenu(){
		print ("hit");
		Application.LoadLevel ("Settings");
	}
}
