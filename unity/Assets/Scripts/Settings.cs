using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
// using SocketIO;
using UnityEngine.UI;
using LitJson;

public class Settings : MonoBehaviour {

	// private SocketIOComponent socket;

	public GameObject ErrorTxt;

	public string pName;
	//public string pTeam;
	public string pID;
	public string pPW;
	//public string roomID;
	public string loginurl = "https://threedator.appspot.com/admin.checklogin";
	// public string loginurl = "http://localhost:24080/admin.checklogin";
	public int pScore;

	bool dataValidated = false;

	InputField InputName;
	InputField InputTeam;
	InputField InputID;
	InputField InputPW;
	InputField InputRoomID;
	InputField InputUrl;

	void Start () {

		// connect to socketIO
		// GameObject go = GameObject.Find("SocketIO");
		// socket = go.GetComponent<SocketIOComponent>();
		// socket.On ("logonack",receiveSocketDataAck);
		//socket.On ("logonnack",receiveSocketDataNack);

		//access input fields
		InputName = GameObject.Find("InputFieldPlayer").GetComponent<InputField>();
		//InputTeam = GameObject.Find("InputFieldTeam").GetComponent<InputField>();
		//InputID = GameObject.Find("InputFieldID").GetComponent<InputField>();
		InputPW = GameObject.Find("InputFieldPW").GetComponent<InputField>();
		//InputRoomID = GameObject.Find("InputFieldRoomID").GetComponent<InputField>();
		//InputUrl = GameObject.Find("InputFieldURL").GetComponent<InputField>();
		//reset error message
		ErrorTxt.GetComponent<Text>().text = "";

		//set values
		ReadLocalPrefs();
	}

	public void SetFormData(){
		pName = InputName.text;
		//pTeam = InputTeam.text;
		//pID = InputID.text;
		pPW = InputPW.text;
		//roomID = InputRoomID.text;
		//url = InputUrl.text;

		// 1st version via socketIO
		// SendJsonData();
		StartCoroutine(SendJsonData());

	}

	// on button click "SET"
	void SaveToLocalPrefs(){
		PlayerPrefs.SetString("PLAYER", pName);
		//PlayerPrefs.SetString("TEAM", pTeam);
		//PlayerPrefs.SetString("ID", pID);
		PlayerPrefs.SetString("PASSWORD", pPW);
		//PlayerPrefs.SetString("ROOM", roomID);
		//PlayerPrefs.SetString("URL", url);
	}

	public void ClearScores(){
		//
	}

	// on button click "GO"
	public void StartGame(){
		if (dataValidated) Application.LoadLevel ("Threedator");
	}

	void ReadLocalPrefs(){
		InputName.text = PlayerPrefs.GetString("PLAYER");
		//InputTeam.text = PlayerPrefs.GetString("TEAM");
		//InputID.text = PlayerPrefs.GetString("ID");
		InputPW.text = PlayerPrefs.GetString("PASSWORD");
		//InputRoomID.text = PlayerPrefs.GetString("ROOM");
		//InputUrl.text = PlayerPrefs.GetString("URL");
	}

	// 1st version via socketIO
	// send data
	// public void SendJsonData(){
		// Dictionary<string,string> json = new Dictionary<string, string>();
		// json.Add("login",pName.ToString());
		// json.Add("password",pPW.ToString());
		// socket.Emit("logon",new JSONObject(json));
		// send via http post urlencoded
	private IEnumerator SendJsonData() {
		WWWForm form = new WWWForm();
		form.AddField("playername", pName.ToString());
		form.AddField("password", pPW.ToString());
		WWW loginResponse = new WWW(loginurl, form);
		// Debug
		// print("Send: "+pName.ToString()+" "+pPW.ToString());

		yield return loginResponse;

		if (loginResponse.error == null) {
			// JSONObject jo = loginResponse.text as JSONObject;
			JsonData jo = JsonMapper.ToObject(loginResponse.text);

			if (jo["logon"]["status"].ToString() == "ack"){
				pID = jo["logon"]["playerid"].ToString();
				pScore = int.Parse(jo["logon"]["playerscore"].ToString());
				SaveToLocalPrefs ();
				ErrorTxt.GetComponent<Text>().text = "Login data ok!";
				dataValidated = true;
				// Debug
				print("Recv: OK - User: "+pName.ToString()+" ID: "+pID.ToString()+" Score: "+pScore.ToString());
				SaveToLocalPrefs ();
				dataValidated = true;
			}
			else if (jo["logon"]["status"].ToString() == "nack" ){
				ErrorTxt.GetComponent<Text>().text = "Login data incorrect!";
				dataValidated = false;
			}
		}
	}
	// receive data when status ok
	// public void receiveSocketDataAck(SocketIOEvent e){
	// 	Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
	// 	JSONObject jo = e.data as JSONObject;
	// 	//print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);
	//
	// 	if (pName == jo["playername"].str){
	// 		pID = jo["playerid"].str;
	// 		pScore = int.Parse(jo["playerscore"].str);
	// 		SaveToLocalPrefs ();
	// 		ErrorTxt.GetComponent<Text>().text = "Login data ok!";
	// 		dataValidated = true;
	// 	}
	// }

	// receive data when status ok
	// public void receiveSocketDataNack(SocketIOEvent e){
	// 	Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
	// 	JSONObject jo = e.data as JSONObject;
	// 	//print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);
	//
	// 	if (pName == jo["playername"].str){
	// 		ErrorTxt.GetComponent<Text>().text = "Login data incorrect!";
	// 		dataValidated = false;
	// 	}
	// }
}
