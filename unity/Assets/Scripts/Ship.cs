using UnityEngine;
using System.Collections;

public class Ship
{
	public GameObject ship;
	public string id;
//	public float posX;
//	public float posZ;
//	public float targetX;
//	public float targetZ;
	public int time;

	//public Ship(GameObject newShip, int newID, float newPosX, float newPosZ, float newTargetX, float newTargetZ, int newTime){ 
	public Ship(GameObject newShip, string newID, int newTime){ 
		ship = newShip;
		id = newID;
		//posX = newPosX;
		//posZ = newPosZ;
		//targetX = newTargetX;
		//targetZ = newTargetZ;
		time = newTime;
	}
}