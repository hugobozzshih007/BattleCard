using UnityEngine;
using System.Collections;

public class PrizeStorage : MonoBehaviour {
	public Transform[] Prizes = new Transform[6];
	// Use this for initialization
	SystemSound sSound;
	void Start () {
		sSound = GameObject.Find("SystemSound").GetComponent<SystemSound>();
	}
	
	public int PrizeNum(){
		int prizeNum = Random.Range(0,6);
		return prizeNum;
	}
	
	public Transform PlaceRealPrize(Transform map, int prizeNum){
		Vector3 pos = new Vector3(map.position.x, map.position.y +3.0f, map.position.z);
		Transform realPrize = Instantiate(Prizes[prizeNum], pos, Quaternion.identity) as Transform;
		sSound.PlaySound(SysSoundFx.OpenPrize);
		return realPrize;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public enum PrizeType{
	LeaderPromotion, 
	FullHPRecovery, 
	DefBuff, 
	AttackBuff, 
	CriticalBuff,
	MoveRangeBuff,
}