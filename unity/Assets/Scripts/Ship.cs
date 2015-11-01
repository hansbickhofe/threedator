using UnityEngine;
using System.Collections;

public class Ship
{
	public GameObject ship;
	public int id;
	public float posX;
	public float posZ;
	public int time;

	public Ship(GameObject newShip, int newID, float newPosX, float newPosZ, int newTime){
		ship = newShip;
		id = newID;
		posX = newPosX;
		posZ = newPosZ;
		time = newTime;
	}
}