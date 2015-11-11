using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public GameObject DebugTxt;
	public PlayerData PlayerScript;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		DebugTxt.GetComponent<Text>().text = 
		"Name: "+PlayerScript.playername+"\n"+
		"ID: "+PlayerScript.id+"\n"+
		"Score: "+PlayerScript.score+"\n"+
		"Shots: "+PlayerScript.shots+"\n"+
		"Team: "+PlayerScript.team+"\n"+
		"Click: "+PlayerScript.clickText+"\n"+
		"Touch: "+PlayerScript.touchText+"\n"+
		"HitPos: "+PlayerScript.hitPos+"\n"
		;

		//android back button -> Settings
		if (Input.GetKeyDown(KeyCode.Escape)) Application.LoadLevel ("Settings");
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
