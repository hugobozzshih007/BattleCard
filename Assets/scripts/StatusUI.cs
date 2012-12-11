using UnityEngine;
using System.Collections;

public class StatusUI : MonoBehaviour {
	
	public bool Attacking, Moving, Skilling, Summoning;
	public Texture2D AtkTexture, MovTexture, SkiTexture, SumTexture;
	public Transform Chess;
	Vector3 screenPos;
	Rect labPosition;
	// Use this for initialization
	void Start () {
		Attacking = Moving = Skilling = Summoning = false;
		Chess = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){
		GUI.depth = 1;
		if(Chess!=null){
			screenPos = Camera.main.WorldToScreenPoint(Chess.position);
			screenPos.y = Screen.height - screenPos.y;
			labPosition = new Rect(screenPos.x-32, screenPos.y-97,100,100);
			if(Attacking)
				GUI.Label(labPosition,AtkTexture);
			if(Moving)
				GUI.Label(labPosition,MovTexture);
			if(Skilling)
				GUI.Label(labPosition,SkiTexture);
			if(Summoning)
				GUI.Label(labPosition,SumTexture);
		}
	}
}
