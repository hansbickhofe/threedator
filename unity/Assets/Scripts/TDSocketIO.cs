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
	public Game GameScript;
	public PlayerData PlayerScript;
	public SetTargetCourse TargetScript;
	public SetTorpedoCourse TorpedoScript;

	// player
	public GameObject ship;
	string id;
	float posX;
	float posZ;
	float targetX;
	float targetZ;
	int shipTime;

	// torpedo
	public GameObject torpedo;

	//muni
	public GameObject Muni;
	public int muniAmmount;
	GameObject[] MuniArray = new GameObject[3]; 
	Vector3 spawnPosition;

	// received player data
	public GameObject r_ship;
	string r_id;
	float r_posX;
	float r_posZ;
	float r_targetX;
	float r_targetZ;
	int r_shipTime;

	// received torpedo data
	string t_id;
	float t_posX;
	float t_posZ;
	float t_targetX;
	float t_targetZ;

	// received muni data
	string m_id;
	float m_posX;
	float m_posZ;


	// process player data
	public List<Ship> allShips = new List<Ship>();
	int playerArraySize;
	public int lifeTime;
	float timer;
	public float sendDataTime;
	public int speed;
	
	// process torpedo data
	public List<Torpedo> allTorpedos = new List<Torpedo>();
	int torpedoArraySize;


	public void Start() {
		// connect to socketIO
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On ("head",receiveHeadData); // VR kopfbewegungen

		socket.On ("player",receiveSocketData);
		socket.On ("newmuni",receiveNewMuniData); // only receive no send
		socket.On ("pickedmuni",receivePickedMuniData);
		socket.On ("torpedo",receiveTorpedoData);
		socket.On ("gothit",receiveHitData);
		socket.On ("water",receiveWaterData);
		CreateMunition();
	}

	public void Update(){
		//timer
		timer += Time.deltaTime;

		if (timer > sendDataTime) {
			//set current target course
			targetX = TargetScript.targetX;
			targetZ = TargetScript.targetZ;
			SendPlayerJsonData();

			//vr headtracing
			//SendHeadData(new Vector3(UnityEngine.Random.Range(0,360),UnityEngine.Random.Range(0,360),UnityEngine.Random.Range(0,360)));

			timer = 0;
		}
	}

	// create 3 initial muni packs
	public void CreateMunition(){
		for (int i=0; i<muniAmmount; i++){
			GameObject newMuni = Instantiate(Muni, Vector3.zero, Quaternion.identity) as GameObject;
			newMuni.GetComponent<Munition>().ID = (i+1)*333; //ID setzen
			newMuni.SetActive(false);
			MuniArray[i] = newMuni; // push to array
			newMuni.transform.parent = GameObject.Find("Munition").transform; //make child of empty game object
		}
	}


	// send receive head positions data
	public void SendHeadData(Vector3 rot){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",PlayerScript.id);
		json.Add("rotX",rot.x.ToString());
		json.Add("rotY",rot.y.ToString());
		json.Add("rotZ",rot.y.ToString());
		socket.Emit("head",new JSONObject(json));
	}

	// receive head data
	public void receiveHeadData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		
		//andere spieler

		string playerID = jo["id"].str;
		Vector3 playerRotation = new Vector3 (float.Parse(jo["rotX"].str),float.Parse(jo["rotY"].str),float.Parse(jo["rotZ"].str));
		PlayerScript.headRotation = playerRotation;
	}


	//send receive player data ------------------------------------------------

	// send player data
	public void SendPlayerJsonData(){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",PlayerScript.id);
		json.Add("posX",posX.ToString());
		json.Add("posZ",posZ.ToString());
		json.Add("targetX",targetX.ToString());
		json.Add("targetZ",targetZ.ToString());
		json.Add("time",shipTime.ToString());
		socket.Emit("player",new JSONObject(json));
	}

	// receive player data
	public void receiveSocketData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;

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

	// send receive muni data---------------------------------------------

	// receive new muni data
	public void receiveNewMuniData(SocketIOEvent e){
		//Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		
		// ID's filtern
		m_id = jo["id"].str;
		m_posX = float.Parse(jo["posX"].str);
		m_posZ = float.Parse(jo["posZ"].str);

		//translate muni id (z.b.333,666,999) to array position -> return 0,1,2
		int arrayPos = (int.Parse (m_id) / 333) - 1;
		//show muni & set position
		MuniArray[arrayPos].SetActive (true);
		MuniArray[arrayPos].transform.position = new Vector3 (m_posX, .05f, m_posZ);
	}


	// send pickup data
	public void SendPickupJsonData(int muniID){
		Dictionary<string,string> json = new Dictionary<string, string>();

		json.Add("p_id",PlayerScript.id);
		json.Add("k_id",muniID.ToString()); //kisten id
		socket.Emit("pickedmuni",new JSONObject(json));
	}

	// receive muni data
	public void receivePickedMuniData(SocketIOEvent e){

		//Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
	
		string muniPicker = jo["p_id"].str;
		string muniID = jo["k_id"].str;
		//print("id "+jo["id"].str);

		if (muniPicker == PlayerScript.id) {
			PlayerScript.muni++;
			int arrayPos = (int.Parse (muniID) / 333) - 1;
			MuniArray[arrayPos].SetActive (false);
		}
	}



	//send receive torpedo data ------------------------------------------------

	//send torpedo target data
	public void SetTorpedoTarget(Vector3 target){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",PlayerScript.id);
		json.Add("posX",PlayerScript.position.x.ToString());
		json.Add("posZ",PlayerScript.position.z.ToString());
		json.Add("targetX",target.x.ToString());
		json.Add("targetZ",target.z.ToString()); 
		socket.Emit("torpedo",new JSONObject(json)); // muss später neu benannt werden
	}

	// receive torpedo target data
	public void receiveTorpedoData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;

		t_id = jo["id"].str;
		t_posX = float.Parse(jo["posX"].str);
		t_posZ = float.Parse(jo["posZ"].str);
		t_targetX = float.Parse(jo["targetX"].str);
		t_targetZ = float.Parse(jo["targetZ"].str); 

		ProcessTorpedoData();
	}

	// send receive hit data ---------------------------------------------------
	public void SendGotHit(string torpedoID){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("torpedoID",PlayerScript.id);
		json.Add("shipID",PlayerScript.id);
		socket.Emit("gothit",new JSONObject(json)); 
	}

	public void receiveHitData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;

		string hit_tID = jo["torpedoID"].str; //receive emeny torpedo id
		string hit_sID = jo["shipID"].str; //receive emeny torpedo id

		ProcessHitData(hit_tID,hit_sID);
	}

	// send receive water data
	public void SendWater(string torpedoID){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("torpedoID",PlayerScript.id);
		socket.Emit("water",new JSONObject(json)); 
	}
	
	public void receiveWaterData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		
		string tID = jo["torpedoID"].str; //receive torpedo id
		
		ProcessWaterData(tID);
	}






	// process all collected data ----------------------------------------------------------

	// process
	void ProcessPlayerData(){

		bool idFound = false;

		// check if id already exists
		playerArraySize = allShips.Count;

		for (int i = 0; i<playerArraySize; i++){
			//print("hello"+arraySize);

			if (allShips[i].id == r_id) {
				//existing ship pos updaten

				ShipMove ShipScript = allShips[i].ship.GetComponent<ShipMove>();
				// evtl später daten ins array schreiben. k.a. was schneller ist
				// ShipScript.posX = r_posX;
				// ShipScript.posZ = r_posX;
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
			Vector3 spawnPosition = new Vector3(r_posX,4,r_posZ);
			newShip = Instantiate(ship, spawnPosition, transform.rotation) as GameObject;

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

			//color und name setzen
			newShip.GetComponent<Renderer>().material.SetColor("_Color", playerColor);
			newShip.transform.Find("PlayerName").GetComponent<TextMesh>().text = playername;


			// neue daten ins array schreiben
			//allShips.Add(new Ship(newShip, r_id, r_posX, r_posZ, r_targetX, r_targetZ, r_shipTime));
			allShips.Add(new Ship(newShip, r_id, r_shipTime));
			playerArraySize++;
			ProcessPlayerData(); // prozess neu triggern um neuen id mit einzubeziehen
		}
	}

	// look for unused player objects after shiptime (sec.)
	void CleanupPlayerData(){
		playerArraySize = allShips.Count;

		for (int i = 0; i<playerArraySize; i++){
			if (allShips[i].time > lifeTime) {
				//ship object und array eintrag löschen
				Destroy(allShips[i].ship);
				allShips.RemoveAt(i);
				break;
			} else {
				allShips[i].time++;
			}
		}
	}

	void ProcessTorpedoData(){
		bool idFound = false;
		
		// check if id already exists
		torpedoArraySize = allTorpedos.Count;
		
		for (int i = 0; i<torpedoArraySize; i++){
			
			if (allTorpedos[i].id == t_id) {
				//existing torpedo pos updaten -> aus ship move script!!
				
				TorpedoMove TorpedoScript = allTorpedos[i].torpedo.GetComponent<TorpedoMove>();
				// evtl später daten ins array schreiben. k.a. was schneller ist
				TorpedoScript.id = t_id;
				TorpedoScript.posX = t_posX;
				TorpedoScript.posZ = t_posZ;
				TorpedoScript.targetX = t_targetX;
				TorpedoScript.targetZ = t_targetZ;
				TorpedoScript.SetStartPosition();
				//print(id+": is already there!");
				idFound = true;
				break;
			}
		}
		
		// neue id eintragen
		if (!idFound){
			GameObject newTorpedo;
			Vector3 spawnPosition = new Vector3(posX,4,posZ); // torpedo an aktueller ship position erzeugen
			newTorpedo = Instantiate(torpedo, spawnPosition, transform.rotation) as GameObject;

			// neue daten ins array schreiben
			allTorpedos.Add(new Torpedo(newTorpedo, t_id));

			torpedoArraySize++;
			ProcessTorpedoData(); // prozess sofort restarten um neue id mit einzubeziehen
		}
	}

	void ProcessHitData(string torpedo, string ship){
		if (ship == PlayerScript.id) {
			print ("u got hit by: "+torpedo);
		}
	}

	void ProcessWaterData(string torpedo){
		for (int i = 0; i<torpedoArraySize; i++){
			if (allTorpedos[i].id == torpedo) {
				TorpedoMove TorpedoScript = allTorpedos[i].torpedo.GetComponent<TorpedoMove>();
				TorpedoScript.HideTorpedo();
				break;
			}
		}
		print (torpedo+" missed!");
	}
}
