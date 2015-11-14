using UnityEngine;
using System.Collections;

public class Torpedo
{
	public GameObject torpedo;
	public string id;
	public int time;
	
	public Torpedo(GameObject newTorpedo, string newID){ 
		torpedo = newTorpedo;
		id = newID;
	}
}