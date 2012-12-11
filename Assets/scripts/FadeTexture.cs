using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeTexture : MonoBehaviour {

	private GUITexture gt;
	private float _alpha = 1F;
	private bool fadeIn = false;
	
	// Use this for initialization
	void Start () 
	{
	    gt = (GUITexture) gameObject.GetComponent(typeof(GUITexture));
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	    print(_alpha);
	
	    if(fadeIn)
	    {
	        _alpha = Mathf.Lerp(_alpha, 1F, Time.deltaTime);
	        gt.color = new Color(.5F,.5F,.5F,_alpha);
	        if(_alpha > .98F) fadeIn = false;
	    }
	    else
	    {
	        _alpha = Mathf.Lerp(_alpha, 0F, Time.deltaTime);
	        gt.color = new Color(.5F,.5F,.5F,_alpha);
	        if(_alpha < .01F) fadeIn = true;
	    }                   
	}
}