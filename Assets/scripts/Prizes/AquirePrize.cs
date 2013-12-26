using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using BuffUtility;

public class AquirePrize : MonoBehaviour {
	public PrizeType PrizeType;
	public int BuffAmount = 0; 
	public Transform PosMap = null;
	private float castLength = 20.0f;
	int flyTimes = 400;
	int t = 0;
	int f = 0;
	bool flying = false;
	const float seg = 0.001f;
	Transform fxBuffAtk, fxBuffDef, fxBuffCritiq, fxBuffMoveRange, fxBuffHp, fxBuffLeader; 
	//BuffSlidingUI bSUI;
	SkillSlidingUI sUI;
	bool excuted = false;
	MoveCharacter mc; 
	PlacePrizes pp;
	Vector3 noWhere = new Vector3(1000.0f,1000.0f,1000.0f);
	// Use this for initialization
	void Start () {
		fxBuffAtk = Camera.mainCamera.GetComponent<CommonFX>().BuffAtk;
		fxBuffDef = Camera.mainCamera.GetComponent<CommonFX>().BuffDef;
		fxBuffCritiq = Camera.mainCamera.GetComponent<CommonFX>().BuffCritiq;
		fxBuffMoveRange = Camera.mainCamera.GetComponent<CommonFX>().BuffMove;
		fxBuffHp = Camera.mainCamera.GetComponent<CommonFX>().BuffHp;
		fxBuffLeader = Camera.mainCamera.GetComponent<CommonFX>().BuffLeader;
		//bSUI = Camera.mainCamera.GetComponent<BuffSlidingUI>();
		sUI = Camera.mainCamera.GetComponent<SkillSlidingUI>();
		mc = Camera.mainCamera.GetComponent<MoveCharacter>();
		excuted = false;
		pp = GameObject.Find("Maps").transform.GetComponent<PlacePrizes>();
	}
	
	void OnTriggerStay (Collider collision){
		Transform hit = collision.transform;
		Transform posMap = null;
		if(hit.gameObject.layer == 10 || hit.gameObject.layer == 11){
			posMap = hit.GetComponent<CharacterSelect>().getMapPosition();
			if(!mc.MoveMode && !excuted){
				// add buff to the hit character
				ExcuteBuff(hit, BuffAmount);
				transform.position = noWhere;
				//restore map's prize
				if(posMap!=null){
					Identy posMapID = posMap.GetComponent<Identy>();
					posMapID.Prize = null;
					pp.RemovePrizeMap(posMap);
				}
				Destroy(transform.gameObject, 6.0f);
				excuted = true;
			}
		}
	}
	
	public Transform GetMapPosition(){
		Transform mapPosition = null; 
		Vector3 rayDir = -transform.up;
		Ray rayDown = new Ray(transform.position, rayDir);
		RaycastHit hit;
		if(Physics.Raycast(rayDown,out hit,castLength)){
			mapPosition = hit.transform;
		}
		return mapPosition;
	}
	
	public int GetBuffAmount(PrizeType bt){
		int amount = 0;
		switch(bt){
			case PrizeType.AttackBuff:
				amount = Random.Range(1,5);
				break;
			case PrizeType.CriticalBuff:
				amount = Random.Range(10, 20);
				break;
			case PrizeType.DefBuff:
				amount = Random.Range(1, 5);
				break;
			case PrizeType.MoveRangeBuff:
				amount = Random.Range(1, 2);
				break;
			default:
				amount = 0;
				break;
		}
		return amount;
	}
	
	public void ExcuteBuff(Transform chess, int amount){
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		BuffList chessBuff = chess.GetComponent<BuffList>();
		switch(PrizeType){
			case PrizeType.AttackBuff:
				chessBuff.ExtraDict[BuffType.Attack] += amount;
				SetBuffUI(BuffType.Attack, chess, amount);
				break;
			case PrizeType.CriticalBuff:
				chessBuff.ExtraDict[BuffType.CriticalHit] += amount;
				SetBuffUI(BuffType.CriticalHit, chess, amount);
				break;
			case PrizeType.DefBuff:
				chessBuff.ExtraDict[BuffType.Defense] += amount;
				SetBuffUI(BuffType.Defense, chess, amount);
				break;
			case PrizeType.FullHPRecovery:
				SkillUI sui = new SkillUI(chess, true, "HP Full Recovery");
				sUI.UIItems.Add(sui);
				sUI.FadeInUI = true;
				MapHelper.SetFX(chess,GetFX(BuffType.Hp),4.0f);
				chessP.Hp = chessP.MaxHp;
				break;
			case PrizeType.LeaderPromotion:
				CharacterPassive chessPass = chess.GetComponent<CharacterPassive>();
				chessPass.PassiveDict[PassiveType.Leader] = true;
				SkillUI suiLeader = new SkillUI(chess, true, "Leader Promoted");
				sUI.UIItems.Add(suiLeader);
				sUI.FadeInUI = true;
				MapHelper.SetFX(chess,GetFX(BuffType.SkillRate),4.0f);
				chessP.LeadingCharacter = true;
				break;
			case PrizeType.MoveRangeBuff:
				int mAmount = Random.Range(1,2);
				chessBuff.ExtraDict[BuffType.MoveRange] += amount;
				SetBuffUI(BuffType.MoveRange, chess, amount);
				break;
		}
		BuffCalculation buffCal = new BuffCalculation(chess);
		buffCal.UpdateBuffValue();
	}
	
	void SetBuffUI(BuffType type, Transform target, int amount){
		//show Visual UI
		Dictionary<BuffType,int> dict = new Dictionary<BuffType, int>();
		dict.Add(type, amount);
		BuffSlidingFX targetBFX = target.GetComponent<BuffSlidingFX>();
		targetBFX.ActiveBuffSlidingFX(dict);
		//BuffUI bUI = new BuffUI(target,dict);
		//bSUI.UIItems.Add(bUI);
		MapHelper.SetFX(target,GetFX(type),4.0f);
		//bSUI.FadeInUI = true;
	}
	
	Transform GetFX(BuffType type){
		Transform buffFX = null;
		switch(type){
			case BuffType.Attack:
				buffFX = fxBuffAtk;
				break;
			case BuffType.CriticalHit:
				buffFX = fxBuffCritiq;
				break;
			case BuffType.Defense:
				buffFX = fxBuffDef;
				break;
			case BuffType.MoveRange:
				buffFX = fxBuffMoveRange;
				break;
			case BuffType.Hp:
				buffFX = fxBuffHp;
				break;
			default:
				buffFX = fxBuffLeader;
				break;
		}
		return buffFX;
	}
	
	// Update is called once per frame
	void Update () {
		if(!flying){
			if(f<800){
				transform.Translate(0.0f,seg,0.0f);
				f+=1;
				if(f==flyTimes){
					f=0;
					flying = true;
				}
			}
		}else{
			if(t<flyTimes){
				transform.Translate(0.0f,seg,0.0f);
				t+=1;
			}else if(t>=flyTimes){
				transform.Translate(0.0f,-seg,0.0f);
				t+=1;
				if(t>=flyTimes*2)
					t=0;
			}
		}
	}
}
