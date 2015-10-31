using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

	public string pName;
	public string pTeam;
	public string pID;
	public string pPW;
	public string roomID;
	public string url = "http://defaulturl.com";
	
	InputField InputName;
	InputField InputTeam;
	InputField InputID;
	InputField InputPW;
	InputField InputRoomID;
	InputField InputUrl;

	// Use this for initialization
	void Start () {
		//access input fields
		InputName = GameObject.Find("InputFieldPlayer").GetComponent<InputField>();
		InputTeam = GameObject.Find("InputFieldTeam").GetComponent<InputField>();
		InputID = GameObject.Find("InputFieldID").GetComponent<InputField>();
		InputPW = GameObject.Find("InputFieldPW").GetComponent<InputField>();
		InputRoomID = GameObject.Find("InputFieldRoomID").GetComponent<InputField>();
		InputUrl = GameObject.Find("InputFieldURL").GetComponent<InputField>();

		//set default url
		InputUrl.placeholder.GetComponent<Text>().text = url;
	}

	public void SetFormData(){
		pName = InputName.text;
		pTeam = InputTeam.text;
		pID = InputID.text;
		pPW = InputPW.text;
		roomID = InputRoomID.text;
		url = InputUrl.text;

		SaveToLocalPrefs ();
	}

	void SaveToLocalPrefs(){
		PlayerPrefs.SetString("PLAYER", pName);
		PlayerPrefs.SetString("TEAM", pTeam);
		PlayerPrefs.SetString("ID", pID);
		PlayerPrefs.SetString("PASSWORD", pPW);
		PlayerPrefs.SetString("ROOM", roomID);
		PlayerPrefs.SetString("URL", url);
	}

	public void StartGame(){
		Application.LoadLevel ("Threedator");
	}
}
