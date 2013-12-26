using UnityEngine;
using System.Collections;

public class ArrowUI : MonoBehaviour {
	public Texture2D LeftArrow, RightArrow;
	Texture2D realArrow;
	float _Alpha = 0.0f;
	bool fadeIn = true; 
	bool fadeOut = false;
	Rect arrowPos = new Rect();
	bool showArrowUI = false;
	
	// Use this for initialization
	void Start () {
		realArrow = LeftArrow;
	}
	
	public enum ArrowMode{
		left, right, up, down, upLeft, upRight, downLeft, downRight,
	};
	
	Texture2D ChooseArrow(ArrowMode mode){
		switch(mode){
			case ArrowMode.upLeft:
				return LeftArrow;
				break;
			case ArrowMode.downLeft:
				return LeftArrow;
				break;
			case ArrowMode.left:
				return LeftArrow;
				break;
			case ArrowMode.downRight:
				return RightArrow;
				break;
			case ArrowMode.upRight:
				return RightArrow;
				break;
			case ArrowMode.right:
				return RightArrow;
				break;
			default:
				return LeftArrow;
				break;
		}
	}
	
	public bool ShowArrow(Rect pos, ArrowMode mode){
		realArrow = ChooseArrow(mode);
		showArrowUI = true; 
		arrowPos = pos; 
		return true;
	}
	
	public bool HideArrow(){
		showArrowUI = false; 
		return showArrowUI;
	}
	
	void FadeIn(){
		if(fadeIn){
			_Alpha = Mathf.Lerp(_Alpha,1.0f,Time.deltaTime*4);
			if(_Alpha>=0.9f){
				fadeOut = true;
				fadeIn = false;
			}
		}
	}
	
	void FadeOut(){
		if(fadeOut){
			_Alpha = Mathf.Lerp(_Alpha,0.0f,Time.deltaTime*4);
			if(_Alpha<=0.1f){
				fadeOut = false;
				fadeIn = true;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(showArrowUI){
			FadeIn();
			FadeOut();
		}
	}
	
	void OnGUI(){
		GUI.depth = 0;
		GUI.backgroundColor = Color.clear;
		if(showArrowUI){
			GUI.color = new Color(1.0f, 1.0f, 1.0f, _Alpha);
			if(realArrow!=null)
				GUI.DrawTexture(arrowPos, realArrow);
		}
	}
}
