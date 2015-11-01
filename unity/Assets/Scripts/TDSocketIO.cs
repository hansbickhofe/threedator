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
	public Player PlayerScript;
	public MoveShips MoveScript;
	public SetTargetCourse TargetScript;

	// process data
	List<Ship> allShips = new List<Ship>();
	int arraySize;
	public int lifeTime;
	float timer;
	public float sendDataTime;
	public int speed;

	// player
	public GameObject ship;
	int id;
	float posX;
	float posZ;
	float targetX;
	float targetZ;
	int shipTime;

	// received data
	public GameObject r_ship;
	int r_id;
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
		json.Add("id",id.ToString());
		json.Add("posX",posX.ToString());
		json.Add("posZ",posZ.ToString());
		json.Add("targetX",targetX.ToString());
		json.Add("targetZ",targetZ.ToString());
		json.Add("time",shipTime.ToString());
		
		socket.Emit("channelname",new JSONObject(json));
		
		print ("json send: "+json);
	}


	// receive data
	public void receiveSocketData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		//print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);

		r_id = int.Parse(jo["id"].str);
		r_posX = float.Parse(jo["posX"].str);
		r_posZ = float.Parse(jo["posZ"].str);
		r_shipTime = int.Parse(jo["time"].str);
		ProcessData();
		CleanupOldData();
	}

	// process
	void ProcessData(){
		bool idFound = false;
		
		// check if id already exists
		arraySize = allShips.Count;
		for (int i = 0; i<arraySize; i++){
			if ((int)allShips[i].id == r_id) {
				//existing ship pos updaten
				allShips[i].posX = r_posX;
				allShips[i].posZ = r_posZ;
				allShips[i].ship.transform.position = new Vector3(r_posX,0.5f,r_posZ);
				allShips[i].time = r_shipTime;
				print(id+": is already there!");
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
			Color randomColor = new Color (UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f));
			newShip.GetComponent<Renderer>().material.SetColor("_Color", randomColor);

			// neue daten ins array schreiben
			allShips.Add(new Ship(newShip, r_id, r_posX, r_posZ, r_shipTime));
			print (r_id+": added!");
			arraySize++;
		}
	}

	// look for unused player objects after shiptime (sec.)
	void CleanupOldData(){
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
}
