using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour {

	public float speed;
	public float rotationSpeed;
	public int score;
	public int shots;

	public string id;
	public string playername;
	public string color;

	// Use this for initialization
	void Start () {
		id = PlayerPrefs.GetString("ID");
		playername = PlayerPrefs.GetString("PLAYER");
		color = PlayerPrefs.GetString("COLOR");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
