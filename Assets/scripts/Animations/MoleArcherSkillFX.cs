using UnityEngine;
using System.Collections;

public class MoleArcherSkillFX : MonoBehaviour {
	public Transform SkillFx; 
	// Use this for initialization
	void Start () {
	
	}
	
	void showFX(){
		Transform fx = Instantiate(SkillFx, transform.parent.position, Quaternion.identity) as Transform;
		Destroy(fx.gameObject,4.0f);
	}
}
