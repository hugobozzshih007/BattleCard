using UnityEngine;
using System.Collections;

public class SimpleRotate : MonoBehaviour {
	
	public float DegPerSec = 90.0f;

	// Update is called once per frame
	void Update () {
		gameObject.transform.Rotate(new Vector3(0.0f,DegPerSec * Time.deltaTime,0.0f));
	}
}
