using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class Leadership : MonoBehaviour, SkillInterface {
	
	Transform aider, fxBuffAtk, fxBuffDef,fxBuffHp;
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
		fxBuffAtk = Camera.mainCamera.GetComponent<CommonFX>().BuffAtk;
		fxBuffDef = Camera.mainCamera.GetComponent<CommonFX>().BuffDef;
		fxBuffHp = Camera.mainCamera.GetComponent<CommonFX>().BuffHp;
	}
	
	public void InsertSelection (Transform map)
	{
		throw new System.NotImplementedException ();
	}
	
	public IList GetSelectionRange ()
	{
		IList atkList = new List<Transform>();
		Transform localMap = aider.GetComponent<CharacterSelect>().getMapPosition();
		Transform[] aidMaps = localMap.GetComponent<Identy>().neighbor;
		foreach(Transform unit in aidMaps){
			if((unit!=null) && MapHelper.IsMapOccupied(unit)){
				Transform character = MapHelper.GetMapOccupiedObj(unit);
				if(character.GetComponent<CharacterProperty>().Player == aider.GetComponent<CharacterProperty>().Player){
					atkList.Add(character.GetComponent<CharacterSelect>().getMapPosition());
				}
			}
		}
		return atkList;
	}
	
	public void Execute ()
	{
		//BuffSlidingUI bSUI = Camera.mainCamera.GetComponent<BuffSlidingUI>();
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
				// update data 
				target.GetComponent<BuffList>().ExtraDict[BuffType.Defense] += 1;
				target.GetComponent<BuffList>().ExtraDict[BuffType.Attack] += 1;
				BuffCalculation buffCal = new BuffCalculation(target);
				buffCal.UpdateBuffValue();
				//show Visual UI
				Dictionary<BuffType,int> dict = new Dictionary<BuffType, int>();
				dict.Add(BuffType.Defense, 1);
				dict.Add(BuffType.Attack, 1);
				BuffSlidingFX targetBFX = target.GetComponent<BuffSlidingFX>();
				targetBFX.ActiveBuffSlidingFX(dict);
				MapHelper.SetFX(target,fxBuffAtk,4.0f);
				MapHelper.SetFX(target,fxBuffHp,4.0f);
			}
		}
		Camera.mainCamera.GetComponent<MainInfoUI>().StopSkillRender = true;
		//bSUI.FadeInUI = true;
	}
}
