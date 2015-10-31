using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class TDSocketIO : MonoBehaviour
{
	private SocketIOComponent socket;

	//process data
	List<Ship> allShips = new List<Ship>();
	int arraySize;
	public int lifeTime;
	float timer;
	public float sendDataTime;
	public int speed;

	//own ship
	public GameObject ship;
	int id;
	float xPos;
	float zPos;
	int shipTime;

	//received ship data
	public GameObject r_ship;
	int r_id;
	float r_xPos;
	float r_zPos;
	int r_shipTime;
	
	public void Start() {
		id = UnityEngine.Random.Range(0,100000);
		xPos = UnityEngine.Random.Range(-5.0f,5.0f);
		zPos = UnityEngine.Random.Range(-5.0f,5.0f);
		shipTime = 0;

		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		socket.On ("channelname",receiveSocketData);
	}

	public void Update(){
		//keyboard
		xPos += Input.GetAxis ("Horizontal") * speed;
		zPos += Input.GetAxis ("Vertical") * speed;

		//mouse move
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//touch
		if (Input.touchCount > 0){
			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		}

		if (Physics.Raycast(ray, out hit)){
			if (hit.rigidbody != null && hit.rigidbody.tag == "Background"){
				xPos = hit.point.x;
				zPos = hit.point.z;
			}
		}

		//timer
		timer += Time.deltaTime;
		if (timer > sendDataTime) {
			SendJsonData();
			timer = 0;
		}
	}

	public void SendJsonData(){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",id.ToString());
		json.Add("xPos",xPos.ToString());
		json.Add("yPos",zPos.ToString());
		json.Add("time",shipTime.ToString());
		
		socket.Emit("channelname",new JSONObject(json));
		
		print ("json send: "+json);
	}

	//------------------------------------------------------------------//
	
	public void receiveSocketData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		//print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);

		r_id = int.Parse(jo["id"].str);
		r_xPos = float.Parse(jo["xPos"].str);
		r_zPos = float.Parse(jo["yPos"].str);
		r_shipTime = int.Parse(jo["time"].str);
		ProcessData();
		CleanupOldData();
	}

	void ProcessData(){
		bool idFound = false;
		
		// check if id already exists
		arraySize = allShips.Count;
		for (int i = 0; i<arraySize; i++){
			if ((int)allShips[i].id == r_id) {
				//existing ship pos updaten
				allShips[i].xPos = r_xPos;
				allShips[i].zPos = r_zPos;
				allShips[i].ship.transform.position = new Vector3(r_xPos,0.5f,r_zPos);
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
			Vector3 spawnPosition = new Vector3(r_xPos,2,r_zPos);
			newShip = Instantiate(ship, spawnPosition, transform.rotation) as GameObject;
			Color randomColor = new Color (UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f));
			newShip.GetComponent<Renderer>().material.SetColor("_Color", randomColor);

			// neue daten ins array schreiben
			allShips.Add(new Ship(newShip, r_id, r_xPos, r_zPos, r_shipTime));
			print (r_id+": added!");
			arraySize++;
		}

		//check array for debug
//		string listString = "";
//		
//		for (int i = 0; i<arraySize; i++){
//			listString += allShips[i].id+", ";
//		}
//		
//		print(arraySize+" : "+listString);
	}
	
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
