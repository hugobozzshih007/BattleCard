using UnityEngine;
using System.Collections;

public class InfoUI : MonoBehaviour {
	public Texture2D[] Move, Range, Damage,Hp, Critiq, Defense, BuffInt, BuffRate;
	public Texture2D Fly, Mole, FlyHit, Haste, Flamming, Leader, InvisibleInWood;
	Rect posStaticInfo;
	Rect posHeadIcon;
	float width = 500;
	// Use this for initialization
	void Start () {
		posStaticInfo = new Rect(10,6,width,26);
		posHeadIcon = new Rect(200,87,90,90);
	}
	
	public Texture2D GetIcon(BuffType mode, Transform chess){
		Texture2D icon = null;
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		switch(mode){
			case BuffType.MoveRange:
				if(cp.BuffMoveRange == cp.moveRange)
					icon = Move[0];
				else if(cp.BuffMoveRange > cp.moveRange)
					icon = Move[1];
				else
					icon = Move[2];
				break;
			case BuffType.AttackRange:
				if(cp.BuffAtkRange == cp.atkRange)
					icon = Range[0];
				else if(cp.BuffAtkRange > cp.atkRange)
					icon = Range[1];
				else
					icon = Range[2];
				break;
			case BuffType.Attack:
				if(cp.Damage == cp.atkPower)
					icon = Damage[0];
				else if(cp.Damage > cp.atkPower)
					icon = Damage[1];
				else
					icon = Damage[2];
				break;
			case BuffType.Hp:
				if(cp.Hp == cp.MaxHp)
					icon = Hp[0];
				else if(cp.Hp > cp.MaxHp)
					icon = Hp[1];
				else
					icon = Hp[2];
				break;
			case BuffType.CriticalHit:
				if(cp.BuffCriticalHit == cp.CriticalhitChance)
					icon = Critiq[0];
				else if(cp.BuffCriticalHit > cp.CriticalhitChance)
					icon = Critiq[1];
				else
					icon = Critiq[2];
				break;
			case BuffType.Defense:
				if(cp.ModifiedDefPow == cp.defPower)
					icon = Defense[0];
				else if(cp.ModifiedDefPow > cp.defPower)
					icon = Defense[1];
				else
					icon = Defense[2];
				break;
		}
		return icon;
	}
	
	public void ShowSkillInfo(Transform gf, GUIStyle style){
		SkillProperty sProperty = gf.GetComponent<SkillSets>().Skills[0].GetComponent<SkillProperty>();
		int width = sProperty.info.Length*16;
		Rect infoRect = new Rect(10,6,width,26);
		GUI.Label(infoRect,sProperty.info,style);
	}
	
	
	public void CreatedGFRollOver(Transform chess, GUIStyle style){
		float seg = 6.0f;
		int sq = 24;
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		CharacterPassive cpp = chess.GetComponent<CharacterPassive>();
		/*
		Texture2D headIcon = cp.SmallIcon;
		if(headIcon != null)
			GUI.DrawTexture(posHeadIcon,headIcon);
		*/
		Rect moveIcon = new Rect(30,8,sq,sq);
		GUI.DrawTexture(moveIcon,Move[0]);
		GUI.Label(new Rect(moveIcon.x+sq+seg,moveIcon.y,sq,sq),cp.moveRange.ToString(),style);
		GUI.DrawTexture(new Rect(moveIcon.x+sq*2+seg*2,moveIcon.y,sq,sq),Range[0]);
		GUI.Label(new Rect(moveIcon.x+sq*3+seg*3,moveIcon.y,sq,sq),cp.atkRange.ToString(),style);
		GUI.DrawTexture(new Rect(moveIcon.x+sq*4+seg*4,moveIcon.y,sq,sq),Damage[0]);
		GUI.Label(new Rect(moveIcon.x+sq*5+seg*5,moveIcon.y,sq,sq),cp.atkPower.ToString(),style);
		GUI.DrawTexture(new Rect(moveIcon.x+sq*6+seg*6,moveIcon.y,sq,sq),Hp[0]);
		GUI.Label(new Rect(moveIcon.x+sq*7+seg*7,moveIcon.y,sq,sq),cp.MaxHp.ToString(),style);
		GUI.DrawTexture(new Rect(moveIcon.x+sq*8+seg*8,moveIcon.y,sq,sq),Critiq[0]);
		GUI.Label(new Rect(moveIcon.x+sq*9+seg*9,moveIcon.y,sq*3,sq),cp.CriticalhitChance.ToString(),style);
		GUI.DrawTexture(new Rect(moveIcon.x+sq*10+seg*10,moveIcon.y,sq,sq),Defense[0]);
		GUI.Label(new Rect(moveIcon.x+sq*11+seg*11,moveIcon.y,sq*3,sq),cp.defPower.ToString(),style);
		Rect posPass = new Rect(moveIcon.x+sq*11+seg*11,8,sq,sq);
		if(cpp.PassiveAbility.Length>0){
			int sg = 1; sq = 24;
			foreach(PassiveType pt in cpp.PassiveAbility){
				GUI.DrawTexture(new Rect(posPass.x+sq*sg+seg*(sg-1),posPass.y,sq,sq),GetPassiveTexture(pt));
				sg+=1;
			}
		}
	}
	
	public Texture2D GetPassiveTexture(PassiveType pt){
		Texture2D tx = null;
		switch(pt){
			case PassiveType.Flying: 
				tx = Fly;
				break;
			case PassiveType.FlyingHit:
				tx = FlyHit;
				break;
			case PassiveType.DefenseAddOne:
				tx = Mole;
				break;
			case PassiveType.MultiArrow:
				tx = Haste;
				break;
			case PassiveType.WoundBite:
				tx = Flamming;
				break;
			case PassiveType.Leader:
				tx = Leader;
				break;
			case PassiveType.InvisibleInWoods:
				tx = InvisibleInWood;
				break;
		}
		return tx;
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
	}
}
