using UnityEngine;
using System.Collections;

public class SmoothFollow: MonoBehaviour
{
	public Transform followTransform;
	
	public Vector3 offset = new Vector3(0f, 2.5f, -5f);
	public float moveSpeed = 1;
	public float turnSpeed = 1;
	
	Vector3 goalPos;
	
	void FixedUpdate()
	{
		goalPos = followTransform.position + followTransform.TransformDirection(offset);
		transform.position = Vector3.Lerp(transform.position, goalPos, Time.deltaTime * moveSpeed);
		transform.rotation = Quaternion.Lerp(transform.rotation, followTransform.rotation, Time.deltaTime * moveSpeed);
	}
}