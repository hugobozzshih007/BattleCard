using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffList : MonoBehaviour {
	public BuffType[] addBuff;
	public BuffType[] deBuff;
	// Use this for initialization
	void Start () {
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
