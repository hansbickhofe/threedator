using UnityEngine;
using System.Collections;

public class Ship
{
	public GameObject ship;
	public string id;
	public int time;
	
	public Ship(GameObject newShip, string newID, int newTime){ 
		ship = newShip;
		id = newID;
		time = newTime;
	}
}