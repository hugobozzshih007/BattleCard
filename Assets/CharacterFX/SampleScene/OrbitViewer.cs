using UnityEngine;
using System.Collections;

public class OrbitViewer : MonoBehaviour 
{
	public Vector3 Offset = Vector3.up;
	public Transform Target;
	public float distance = 3.0f;
	public float Speed = 20.0f;
	public bool OrbitEnabled = false;
	
	private Vector3 TargetPos = Vector3.zero;
	
	public void SetTransform(Transform t)
	{
		OrbitEnabled = true;
		if (t == Target) return;
		Target = t;
		TargetPos = t.position + Offset;
		gameObject.transform.position = TargetPos - new Vector3(0,0,-distance);
	}
	

	// Update is called once per frame
	void Update () 
	{
		if (OrbitEnabled)
		{
			if (TargetPos == Vector3.zero)
			{
				SetTransform(Target);
			}
			gameObject.transform.RotateAround(TargetPos,Vector3.up,Speed*Time.deltaTime);
			gameObject.transform.LookAt(TargetPos);
		}
	}
}
