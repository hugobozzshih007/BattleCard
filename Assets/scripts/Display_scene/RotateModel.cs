using UnityEngine;
using System.Collections;

public class RotateModel : MonoBehaviour {
	
	float xDeg;  
	public float speed = 5.0f;
	public float friction = 0.2f;  
	public float lerpSpeed = 1.0f;
	Quaternion fromRotation; 
	Quaternion endRotation;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			xDeg -= Input.GetAxis("Mouse X") * speed * friction;
			fromRotation = transform.rotation;
			endRotation = Quaternion.Euler(0.0f,xDeg,0.0f);
			transform.rotation = Quaternion.Lerp(fromRotation,endRotation,Time.deltaTime  * lerpSpeed);
		}
	}
}
