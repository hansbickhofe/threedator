using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class TDSocketIO : MonoBehaviour
{
	private SocketIOComponent socket;

	//scripts
	public PlayerData PlayerScript;
	public SetTargetCourse TargetScript;

	// process data
	public List<Ship> allShips = new List<Ship>();
	int arraySize;
	public int lifeTime;
	float timer;
	public float sendDataTime;
	public int speed;

	// player
	public GameObject ship;
	string id;
	float posX;
	float posZ;
	float targetX;
	float targetZ;
	int shipTime;

	// received muni data
	string m_id;
	float m_posX;
	float m_posZ;

	// received player data
	public GameObject r_ship;
	string r_id;
	float r_posX;
	float r_posZ;
	float r_targetX;
	float r_targetZ;
	int r_shipTime;

	public void Start() {
		// connect to socketIO
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		socket.On ("channelname",receiveSocketData);
		socket.On ("muni",receiveSocketData);
	}

	public void Update(){
		//timer
		timer += Time.deltaTime;

		if (timer > sendDataTime) {
			//get current target course
			targetX = TargetScript.targetX;
			targetZ = TargetScript.targetZ;
			SendJsonData();
			timer = 0;
		}
	}

	// send data
	public void SendJsonData(){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",PlayerScript.id);
		json.Add("posX",posX.ToString());
		json.Add("posZ",posZ.ToString());
		json.Add("targetX",targetX.ToString());
		json.Add("targetZ",targetZ.ToString());
		json.Add("time",shipTime.ToString());

		socket.Emit("channelname",new JSONObject(json));

		//print ("json send: "+json);
	}


	// receive data
	public void receiveSocketData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		//print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);


		// ID's filtern
		if (jo ["id"].str == "333" || jo ["id"].str == "666" || jo ["id"].str == "999") {
			//muni
			m_id = jo["id"].str;
			m_posX = float.Parse(jo["posX"].str);
			m_posZ = float.Parse(jo["posZ"].str);
			print("id "+jo["id"].str);
			//ProcessMuniData();
		} else {
			//andere spieler
			r_id = jo["id"].str;
			r_posX = float.Parse(jo["posX"].str);
			r_posZ = float.Parse(jo["posZ"].str);
			r_targetX = float.Parse(jo["targetX"].str);
			r_targetZ = float.Parse(jo["targetZ"].str);
			r_shipTime = int.Parse(jo["time"].str);
			print("id "+jo["id"].str);
			ProcessPlayerData();
			CleanupPlayerData();
		}
	}

	// process
	void ProcessPlayerData(){

		bool idFound = false;

		// check if id already exists
		arraySize = allShips.Count;

		for (int i = 0; i<arraySize; i++){
			//print("hello"+arraySize);

			if (allShips[i].id == r_id) {
				//existing ship pos updaten

				ShipMove ShipScript = allShips[i].ship.GetComponent<ShipMove>();
				ShipScript.posX = r_posX;
				ShipScript.posZ = r_posX;
				ShipScript.targetX = r_targetX;
				ShipScript.targetZ = r_targetZ;
				allShips[i].time = r_shipTime;

				//print(id+": is already there!");
				idFound = true;
				break;
			}
		}

		// neue id eintragen
		if (!idFound){
			//ship an random pos mit random color erzeugen
			GameObject newShip;
			Vector3 spawnPosition = new Vector3(r_posX,2,r_posZ);
			newShip = Instantiate(ship, spawnPosition, transform.rotation) as GameObject;
			print("hello"+arraySize);

			//color
			Color playerColor;
			string playername;

			if (r_id == PlayerScript.id){ // eigenes ship finden

				// tag für eigenen Spieler setzen
				newShip.tag = "Player";

				//teamfarbe setzen
				if (PlayerScript.team == "red") playerColor = Color.red;
				else if (PlayerScript.team == "green") playerColor = Color.green;
				else if (PlayerScript.team == "blue") playerColor = Color.blue;
				else playerColor = Color.red; //default

				playername = PlayerScript.playername;
			} else {

				// tag für Ememy setzen
				newShip.tag = "Enemy";

				// später color und name der gegner mitsenden
				playerColor = new Color (UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f));
				playername = "?";
			}

			//randomColor = new Color (UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f));

			//color und name setzen
			newShip.GetComponent<Renderer>().material.SetColor("_Color", playerColor);
			newShip.transform.Find("PlayerName").GetComponent<TextMesh>().text = playername;


			// neue daten ins array schreiben
			//allShips.Add(new Ship(newShip, r_id, r_posX, r_posZ, r_targetX, r_targetZ, r_shipTime));
			allShips.Add(new Ship(newShip, r_id, r_shipTime));
			print (r_id+": added!");
			arraySize++;
		}
	}

	// look for unused player objects after shiptime (sec.)
	void CleanupPlayerData(){
		arraySize = allShips.Count;

		for (int i = 0; i<arraySize; i++){
			if (allShips[i].time > lifeTime) {
				//ship object und array eintrag löschen
				Destroy(allShips[i].ship);
				print(allShips[i].id+": Removed!");
				allShips.RemoveAt(i);
				break;
			} else {
				allShips[i].time++;
			}
		}
	}


	// hex to rgb converter
//	Color HexToColor(string hex){
//		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
//		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
//		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
//		return new Color32(r,g,b, 255);
//	}
}
