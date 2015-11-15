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
//	string id;
//	float posX;
//	float posZ;
//	float targetX;
//	float targetZ;

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
		socket.On ("channelname",receiveSocketData);
		socket.On ("muni",receiveSocketData);
		socket.On ("test",receiveTorpedoData);
		CreateMunition();
	}

	public void Update(){
		//timer
		timer += Time.deltaTime;

		if (timer > sendDataTime) {
			//get current target course
			targetX = TargetScript.targetX;
			targetZ = TargetScript.targetZ;
			SendPlayerJsonData();
			print ("send");
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


	//send receive player & muni data ------------------------------------------------

	// send player data
	public void SendPlayerJsonData(){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",PlayerScript.id);
		json.Add("posX",posX.ToString());
		json.Add("posZ",posZ.ToString());
		json.Add("targetX",targetX.ToString());
		json.Add("targetZ",targetZ.ToString());
		json.Add("time",shipTime.ToString());
		socket.Emit("channelname",new JSONObject(json));
	}

	// send pickup data
	public void SendPickupJsonData(int muniID){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("p_id",PlayerScript.id);
		json.Add("k_id",muniID.ToString()); //kisten id
		socket.Emit("gotit",new JSONObject(json));

		print ("hit!"+muniID.ToString());
		PlayerScript.score++;
	}


	// receive data
	public void receiveSocketData(SocketIOEvent e){
		//Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;

		// ID's filtern
		if (jo ["id"].str == "333" || jo ["id"].str == "666" || jo ["id"].str == "999") {
			//muni
			m_id = jo["id"].str;
			m_posX = float.Parse(jo["posX"].str);
			m_posZ = float.Parse(jo["posZ"].str);
			//print("id "+jo["id"].str);
			ProcessMuniData();
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


	//send receive torpedo data ------------------------------------------------

	//send torpedo target data
	public void SetTorpedoTarget(Vector3 target){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",PlayerScript.id);
		json.Add("posX",PlayerScript.position.x.ToString());
		json.Add("posZ",PlayerScript.position.z.ToString());
		json.Add("targetX",target.x.ToString());
		json.Add("targetZ",target.z.ToString()); 
		socket.Emit("test",new JSONObject(json)); // muss später neu benannt werden
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

	//send receive test data ------------------------------------------------

	//	// send test data
	//	public void Test(string testtext){
	//		Dictionary<string,string> json = new Dictionary<string, string>();
	//		json.Add("testbla",testtext); // geht raus -> -> -> -> -> -> -> -> 
	//		socket.Emit("test",new JSONObject(json));
	//	}
	//
	//	// receive test data
	//	public void receiveTestData(SocketIOEvent e){
	//		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
	//		JSONObject jo = e.data as JSONObject;
	//		
	//		// ID's filtern
	//		print ("testbllllaaa: "+ jo ["testbla"].str); // geht rein <- <- <- <- <- <- <- <- <- <- 
	//	}


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
			Vector3 spawnPosition = new Vector3(r_posX,4,r_posZ);
			newShip = Instantiate(ship, spawnPosition, transform.rotation) as GameObject;
			print("hello"+playerArraySize);

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
			print (r_id+": ship added!");
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
				print(allShips[i].id+": Removed!");
				allShips.RemoveAt(i);
				break;
			} else {
				allShips[i].time++;
			}
		}
	}


	void ProcessMuniData(){
		//translate muni id (z.b.333,666,999) to array position -> return 0,1,2
		int arrayPos = (int.Parse (m_id) / 333) - 1;
		//show muni & set position
		MuniArray[arrayPos].SetActive (true);
		MuniArray[arrayPos].transform.position = new Vector3 (m_posX, .05f, m_posZ);
	}


	void ProcessTorpedoData(){
		bool idFound = false;
		
		// check if id already exists
		torpedoArraySize = allTorpedos.Count;
		
		for (int i = 0; i<torpedoArraySize; i++){
			print("torp"+torpedoArraySize);
			
			if (allTorpedos[i].id == t_id) {
				print("t_id: "+t_posX);
				//existing torpedo pos updaten -> aus ship move script!!
				
				TorpedoMove TorpedoScript = allTorpedos[i].torpedo.GetComponent<TorpedoMove>();
				// evtl später daten ins array schreiben. k.a. was schneller ist
				TorpedoScript.id = t_id;
				TorpedoScript.posX = t_posX;
				TorpedoScript.posZ = t_posZ;
				TorpedoScript.targetX = t_targetX;
				TorpedoScript.targetZ = t_targetZ;
				
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
			print (t_id+": torp added!");

			// neue daten ins array schreiben
			allTorpedos.Add(new Torpedo(newTorpedo, t_id));

			torpedoArraySize++;
			ProcessTorpedoData(); // prozess sofort restarten um neue id mit einzubeziehen
			print ("size: "+torpedoArraySize);
		}
	}
}
