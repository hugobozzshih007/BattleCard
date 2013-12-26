using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;

public class DarkBallFX : MonoBehaviour {
	public Transform SkillFx; 
	string skillName = "DarkBallAttack";
	Transform skill; 
	IList targetList = new List<Transform>();
	// Use this for initialization
	void Start () {
		skill =	MapHelper.FindAnyChildren(transform.parent, skillName);
	}
	
	public void InsertTargets(IList targets){
		targetList = targets;
	}
		
	void UnRenderSkill(){
		Camera.mainCamera.GetComponent<MainInfoUI>().StopSkillRender = true;
	}
	
	void PlaySkillFX(){
		if(targetList.Count>0){
			foreach(Transform target in targetList){
				Vector3 bombLocation = new Vector3(target.position.x, target.position.y +5.0f, target.position.z);
				Transform fx = Instantiate(SkillFx, bombLocation, Quaternion.identity) as Transform;
				Destroy(fx.gameObject,2.5f);
			}
			skill.GetComponent<DarkBallAttack>().CalculateDamge();
		}
	}
	
	void ShowDamageUI(){
		skill.GetComponent<DarkBallAttack>().ShowDamge();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
