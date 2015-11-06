using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public int gameTime;
	public int muniAmmount;
	public float gameWidth;
	public float gameHeight;

	public GameObject Muni;
	Vector3 spawnPosition;


	// Use this for initialization
	void Start () {
		CreateMunition();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateMunition(){
		for (int i=0; i<muniAmmount; i++){
			spawnPosition = new Vector3(Random.Range(-gameWidth,gameWidth),.5f,Random.Range(-gameHeight,gameHeight));
			GameObject newMuni = Instantiate(Muni, spawnPosition, Quaternion.identity) as GameObject;

			newMuni.transform.parent = GameObject.Find("Munition").transform;
		}
		
	}
}
