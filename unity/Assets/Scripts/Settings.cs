﻿// login verwendet eigenes json plugin (litjson) im lib folder

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class Settings : MonoBehaviour {

	public GameObject ErrorTxt;

	public string pID;
	public string pName;
	public string pPW;
	public string pColor;
	//public string pTeam;
	//public string roomID;
	public string loginurl = "https://threedator.appspot.com/admin.checklogin";
	// public string loginurl = "http://localhost:24080/admin.checklogin";
	public int pScore;

	bool dataValidated = false;
	
	InputField InputName;
	InputField InputPW;
	InputField InputColor;
	InputField InputTeam;
	InputField InputRoomID;
	InputField InputUrl;

	void Start () {
		//access input fields
		InputName = GameObject.Find("InputFieldPlayer").GetComponent<InputField>();
		InputPW = GameObject.Find("InputFieldPW").GetComponent<InputField>();
		InputColor = GameObject.Find("InputFieldColor").GetComponent<InputField>();
		//InputTeam = GameObject.Find("InputFieldTeam").GetComponent<InputField>();
		//InputRoomID = GameObject.Find("InputFieldRoomID").GetComponent<InputField>();
		//InputUrl = GameObject.Find("InputFieldURL").GetComponent<InputField>();
		//reset error message
		ErrorTxt.GetComponent<Text>().text = "";

		// read existing values
		ReadLocalPrefs();
	}

	void Update(){
		//android back button -> Quit
		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}

	void ReadLocalPrefs(){
		// re-fill input fields from local prefs
		InputName.text = PlayerPrefs.GetString("PLAYER");
		InputPW.text = PlayerPrefs.GetString("PASSWORD");
		InputColor.text = PlayerPrefs.GetString("COLOR");
		//InputTeam.text = PlayerPrefs.GetString("TEAM");
		//InputRoomID.text = PlayerPrefs.GetString("ROOM");
		//InputUrl.text = PlayerPrefs.GetString("URL");
	}

	// on button click "SET"
	public void SetFormData(){
		pName = InputName.text;
		pPW = InputPW.text;
		pColor = InputColor.text;
		//pTeam = InputTeam.text;
		//roomID = InputRoomID.text;
		//url = InputUrl.text;
		
		StartCoroutine(SendJsonData());
	}

	void SaveToLocalPrefs(){
		PlayerPrefs.SetString("ID", pID);
		PlayerPrefs.SetString("PLAYER", pName);
		PlayerPrefs.SetString("PASSWORD", pPW); // später nicht mehr in die prefs schreiben
		PlayerPrefs.SetString("COLOR", pColor); // später nicht mehr in die prefs schreiben
		//PlayerPrefs.SetString("TEAM", pTeam);
		//PlayerPrefs.SetString("ROOM", roomID);
		//PlayerPrefs.SetString("URL", url);
	}

	// send data
	private IEnumerator SendJsonData() {
		WWWForm form = new WWWForm();
		form.AddField("playername", pName.ToString());
		form.AddField("password", pPW.ToString());
		WWW loginResponse = new WWW(loginurl, form);

		yield return loginResponse;

		if (loginResponse.error == null) {
			JsonData jo = JsonMapper.ToObject(loginResponse.text);

			if (jo["logon"]["status"].ToString() == "ack"){
				pID = jo["logon"]["playerid"].ToString();
				pScore = int.Parse(jo["logon"]["playerscore"].ToString());
				SaveToLocalPrefs();
				ErrorTxt.GetComponent<Text>().text = "Login data ok!";
				dataValidated = true;

				// Debug
				//print("Recv: OK - User: "+pName.ToString()+" ID: "+pID.ToString()+" Score: "+pScore.ToString());
			}
			else if (jo["logon"]["status"].ToString() == "nack" ){
				ErrorTxt.GetComponent<Text>().text = "Login data incorrect!";
				dataValidated = false;
			}
		}
	}

	// on button click "GO"
	public void StartGame(){
		if (dataValidated) Application.LoadLevel ("Threedator");
		else ErrorTxt.GetComponent<Text>().text = "No login data set!";
	}

	public void ClearScores(){
		// clear input fields
		InputName.text = "";
		InputPW.text = "";
		InputColor.text = "";
		//InputTeam.text = "";
		//InputRoomID.text = "";
		
		// delete player prefs
		PlayerPrefs.DeleteAll();
		ErrorTxt.GetComponent<Text>().text = "All player prefs cleared!";
	}
}
