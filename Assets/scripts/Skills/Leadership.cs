using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class Leadership : MonoBehaviour, CommonSkill {
	
	Transform aider;
	public PowerType[] Mode;
	public int[] Value;
	public Dictionary<PowerType, int> PowerList;
	
	// Use this for initialization
	void Start () {
		aider = transform.parent.parent;
		PowerList = new Dictionary<PowerType, int>();
		for(int i=0;i<Mode.Length;i++){
			PowerList.Add(Mode[i],Value[i]);
		}
	}
	
	public void InsertSelection (Transform map)
	{
		throw new System.NotImplementedException ();
	}
	
	public IList GetSelectionRange ()
	{
		throw new System.NotImplementedException ();
	}
	
	public void Execute ()
	{
		IList atkList = new List<Transform>();
		Transform localMap = aider.GetComponent<CharacterSelect>().getMapPosition();
		Transform[] attackableMaps = localMap.GetComponent<Identy>().neighbor;
		foreach(Transform unit in attackableMaps){
			if((unit!=null) && MapHelper.IsMapOccupied(unit)){
				Transform character = MapHelper.GetMapOccupiedObj(unit);
				if(character.GetComponent<CharacterProperty>().Player == aider.GetComponent<CharacterProperty>().Player)
					atkList.Add(character);
			}
		}
		
		if(atkList.Count>0){
			foreach(Transform target in atkList){
				target.GetComponent<CharacterProperty>().Hp += 1;
				target.GetComponent<CharacterProperty>().Damage += 1;
			}
		}
	}
}
