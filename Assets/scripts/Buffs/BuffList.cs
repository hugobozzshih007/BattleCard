using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuffList : MonoBehaviour {
	public BuffType[] addBuff;
	public BuffType[] deBuff;
	public Dictionary<BuffType, bool> AddBuffDict = new Dictionary<BuffType, bool>();
	public Dictionary<BuffType, bool> DeBuffDict = new Dictionary<BuffType, bool>();
	public Dictionary<BuffType, int> ExtraDict =  new Dictionary<BuffType, int>();
	Transform papa; 
	// Use this for initialization
	void Start () {
		foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
			AddBuffDict.Add(Buff, false);
		}
		foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
			DeBuffDict.Add(Buff, false);
		}
		if(addBuff.Length>0){
			foreach(BuffType bt in addBuff){
				AddBuffDict[bt] = true;
			}
		}
		if(deBuff.Length>0){
			foreach(BuffType bt in deBuff){
				DeBuffDict[bt] = true;
			}
		}
		papa = transform;
		foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
			ExtraDict.Add(Buff, 0);
		}
	}
	
	public IList GetBuffs(BuffType buff){
		IList addBuffs = new List<BuffType>();
		foreach(BuffType bf in addBuff){
			addBuffs.Add(bf);
		}
		return addBuffs;
	}
	
	public IList GetDeBuffs(BuffType buff){
		IList deBuffs = new List<BuffType>();
		foreach(BuffType bf in deBuff){
			deBuffs.Add(bf);
		}
		return deBuffs;
	}
	
	public bool CheckBuff(BuffType buff){
		bool getBuff = false;
		foreach(BuffType b in addBuff){
			if(buff == b){
				getBuff = true;
				break;
			}
		}
		
		return getBuff; 
	}
	
	public bool CheckDeBuff(BuffType buff){
		bool getBuff = false;
		foreach(BuffType b in deBuff){
			if(buff == b){
				getBuff = true;
				break;
			}
		}
		
		return getBuff; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
