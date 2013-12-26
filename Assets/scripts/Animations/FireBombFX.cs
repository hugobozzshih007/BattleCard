using UnityEngine;
using System.Collections;
using MapUtility;

public class FireBombFX : MonoBehaviour {
	public Transform SkillFx;
	public string SkillName;
	Transform bombMap;
	Transform bombAttack;
	Vector3 bombLocation;
	// Use this for initialization
	void Start () {
		bombLocation = new Vector3();
		bombAttack = MapHelper.FindAnyChildren(transform.parent, SkillName);
	}
	
	public void InsertBombLocation(Transform map){
		bombMap = map; 
		bombLocation = new Vector3(bombMap.position.x, 1.01f, bombMap.position.z);
	}
	
	void PlaySkillFX(){
		if(bombLocation != Vector3.zero){
			Transform fx = Instantiate(SkillFx, bombLocation, Quaternion.identity) as Transform;
			Destroy(fx.gameObject,3.0f);
		}
		bombAttack.GetComponent<BombAttack>().CalculateDamage();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
