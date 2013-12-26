using UnityEngine;
using System.Collections;
using MapUtility;
using System.Collections.Generic;
public class ActivateSkillFX : MonoBehaviour {
	public Transform FX;
	public BuffType Buff;
	Transform targetCharacter;
	int power_value; 
	
	// Use this for initialization
	void Start () {
	}
	
	void BuffVisualUI(BuffType type, int val){
		Dictionary<BuffType,int> dict = new Dictionary<BuffType, int>();
		dict.Add(type, val);
		BuffSlidingFX targetBFX = targetCharacter.GetComponent<BuffSlidingFX>();
		targetBFX.ActiveBuffSlidingFX(dict);
	}
	
	public void ActivateFX(){
		BuffVisualUI(Buff, power_value);
		MapHelper.SetFX(targetCharacter,FX,4.0f);
	}
	
	public void InsertPowerValue(int power){
		power_value = power; 
	}
	
	public void InsertTarget(Transform aimObj){
		targetCharacter = aimObj;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
