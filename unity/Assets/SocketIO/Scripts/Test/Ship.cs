using UnityEngine;
using System.Collections;

public class Ship
{
	public GameObject ship;
	public int id;
	public float xPos;
	public float zPos;
	public int time;

	public Ship(GameObject newShip, int newID, float newXpos, float newZpos, int newTime){
		ship = newShip;
		id = newID;
		xPos = newXpos;
		zPos = newZpos;
		time = newTime;
	}
}