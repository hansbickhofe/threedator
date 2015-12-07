// login verwendet eigenes json plugin (litjson) im lib folder

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
	public string pTeam;
	//public string roomID;
	public string loginurl = "https://threedator.appspot.com/admin.checklogin";
	// public string loginurl = "http://localhost:24080/admin.checklogin";
	public int pScore;

	bool dataValidated = false;
	
	InputField InputName;
	InputField InputPW;
	GameObject ButtonTeam;
	//InputField InputRoomID;
	InputField InputUrl;

	void Start () {
		//access input fields
		InputName = GameObject.Find("InputFieldPlayer").GetComponent<InputField>();
		InputPW = GameObject.Find("InputFieldPW").GetComponent<InputField>();
		ButtonTeam = GameObject.Find("ButtonTeam");
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
		Text ButtonTeamText = ButtonTeam.GetComponentInChildren<Text>();
		Image ButtonTeamColor = ButtonTeam.GetComponent<Image>();

		// re-fill input fields from local prefs
		if (PlayerPrefs.HasKey("PLAYER")) {
			InputName.text = PlayerPrefs.GetString("PLAYER");
		}

		if (PlayerPrefs.HasKey("PASSWORD")) {
			InputPW.text = PlayerPrefs.GetString("PASSWORD");
		}

		if (PlayerPrefs.HasKey("TEAM")) {
			ButtonTeamText.text = PlayerPrefs.GetString("TEAM");
		} else {
			ButtonTeamText.text = "red"; // default
		}

		if (ButtonTeamText.text == "red") ButtonTeamColor.color = Color.red;
		else if (ButtonTeamText.text == "green") ButtonTeamColor.color = Color.green;
		else if (ButtonTeamText.text == "blue") ButtonTeamColor.color = Color.blue;
	
		//InputRoomID.text = PlayerPrefs.GetString("ROOM");
		//InputUrl.text = PlayerPrefs.GetString("URL");
	}

	// on button click "SET"
	public void SetTeam(){
		Text ButtonTeamText = ButtonTeam.GetComponentInChildren<Text>();
		Image ButtonTeamColor = ButtonTeam.GetComponent<Image>();

		if (ButtonTeamText.text == "red") {
			ButtonTeamText.text = "blue";
			ButtonTeamColor.color = Color.blue;
		} else if (ButtonTeamText.text == "blue"){
			ButtonTeamText.text = "green";
			ButtonTeamColor.color = Color.green;
		} else if (ButtonTeamText.text == "green"){
			ButtonTeamText.text = "red";
			ButtonTeamColor.color = Color.red;
		}

	}

	// on button click "SET"
	public void SetFormData(){
		Text ButtonTeamText = ButtonTeam.GetComponentInChildren<Text>();
		pName = InputName.text;
		pPW = InputPW.text;
		pTeam = ButtonTeamText.text;
		print(pTeam);
		//roomID = InputRoomID.text;
		//url = InputUrl.text;
		
		StartCoroutine(SendJsonData());
	}

	void SaveToLocalPrefs(){
		PlayerPrefs.SetString("ID", pID);
		PlayerPrefs.SetString("PLAYER", pName);
		PlayerPrefs.SetString("PASSWORD", pPW); // später nicht mehr in die prefs schreiben
		PlayerPrefs.SetString("TEAM", pTeam);
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
		PlayerPrefs.SetString("VR", "off");
		if (dataValidated) Application.LoadLevel ("Threedator");
		else ErrorTxt.GetComponent<Text>().text = "No login data set!";
	}

	public void ClearScores(){
		Text ButtonTeamText = ButtonTeam.GetComponentInChildren<Text>();
		// clear input fields
		InputName.text = "";
		InputPW.text = "";
		ButtonTeamText.text = "green"; //auto toggle to next color "red"
		SetTeam();
		//InputRoomID.text = "";
		
		// delete player prefs
		PlayerPrefs.DeleteAll();
		ErrorTxt.GetComponent<Text>().text = "All player prefs cleared!";
	}
}
