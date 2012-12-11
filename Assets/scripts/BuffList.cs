using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuffList : MonoBehaviour {
	public BuffType[] addBuff;
	public BuffType[] deBuff;
	public Dictionary<BuffType, bool> AddBuffDict;
	public Dictionary<BuffType, bool> DeBuffDict;
	
	// Use this for initialization
	void Start () {
		AddBuffDict = new Dictionary<BuffType, bool>();
		foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
			AddBuffDict.Add(Buff, false);
		}
		DeBuffDict = new Dictionary<BuffType, bool>();
		foreach(BuffType Buff in Enum.GetValues(typeof(BuffType))){
			DeBuffDict.Add(Buff, false);
		}
		if(addBuff.Length>0){
			foreach(BuffType bt in addBuff){
				AddBuffDict[bt] = true;
			}
		}
		if(deBuff.Length>0){
			foreach(BuffType bt in addBuff){
				DeBuffDict[bt] = true;
			}
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
	
	// Update is called once per frame
	void Update () {
	
	}
}
