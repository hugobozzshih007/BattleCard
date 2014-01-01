using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapUtility;
using System.Linq;

public class MainInfoUI : MonoBehaviour {
	public bool GUIControl = true;
	public Transform Left_Info, Right_Info; 

	public bool StopSkillRender = true;

	//NGUI
	public Transform RedUIQuickInfo;
	public Transform YelUIQuickInfo;
	public Transform ChampBtPrefab; 
	public Transform HUDText; 
	public int LimitedRounds = 30; 
	public int LeftRounds = 1;
	public int PlayerSide = 1;

	Transform redTerritory, yelTerritory, roundNum;
	
	Dictionary<GameObject, Transform> champBts = new Dictionary<GameObject, Transform>();
	bool updateChampBts = false;
	Color red = new Color(1.0f, 0.314f, 0.314f, 1.0f);
	Color yellow = new Color(1.0f, 1.0f, 0.314f, 1.0f);
	Color alpha = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	Transform leftChess, rightChess, playerA, playerB;
	RoundCounter rc;
	GeneralSelection currentSel;  
	Vector2 mousePos;
	StatusMachine sMachine;
	FollowCam fCam;
	SystemSound sysSound;
	
	// Use this for initialization
	void Start () {
		Left_Info.gameObject.SetActive(false); 
		Right_Info.gameObject.SetActive(false);
		fCam = transform.GetComponent<FollowCam>();
		rc = transform.GetComponent<RoundCounter>();
		currentSel = transform.GetComponent<GeneralSelection>();
		sMachine = GameObject.Find("StatusMachine").transform.GetComponent<StatusMachine>();
		sysSound = GameObject.Find("SystemSoundB").transform.GetComponent<SystemSound>();
		leftChess = null;
		rightChess = null;
		LeftRounds = 1;


		//NGUI
		redTerritory = GameObject.Find("red_num").transform;  
		yelTerritory = GameObject.Find("yel_num").transform;
		roundNum = GameObject.Find("rounds").transform;

		rc.SetPlayerChesses();
	}
	
	//old GUI
	public void SomeoneTaking(Transform talker, string content, bool npc){
	}

	//NGUI
	public void SomeoneTalking(Transform talker, string content, bool delay){
		Transform talkingBox = null;
		int talkerSide = talker.GetComponent<CharacterProperty>().Player;
		if(talkerSide == PlayerSide){
			talkingBox = Left_Info.FindChild("talk_column").transform;
		}else{
			talkingBox = Right_Info.FindChild("talk_column").transform;
		}
		talkingBox.GetChild(0).GetComponent<UILabel>().text = content;
		talkingBox.GetComponent<UISprite>().color = Color.white;
		ShowTalkingBox(talker, talkingBox);
		if(delay)
			DelayDeactivateInfoUI(talker, 2.0f);
	}
	
	void Awake(){
		if(Network.connections.Length>0){
			if(Network.peerType == NetworkPeerType.Server){
				PlayerSide = 1;
			}else if(Network.peerType == NetworkPeerType.Client){
				PlayerSide = 2;
			}
		}else{
			PlayerSide = 1;
		}
		
	}

	//NGUI
	public GameObject GetQuickInfo(int side){
		if(side == 1){
			return RedUIQuickInfo.gameObject;
		}else{
			return YelUIQuickInfo.gameObject;
		}
	}

	public void InitChampButtons(){
		SetInitButtonsStatus(rc.PlayerAChesses, 1);
		SetInitButtonsStatus(rc.PlayerBChesses, 2);
		updateChampBts = true;
	}

	void SetInitButtonsStatus(IList chesses, int side){
		int i = 0; 
		GameObject left_container = GameObject.Find("left_champ_container"); 
		GameObject right_container = GameObject.Find("right_champ_container");
		Color sideColor = new Color();
		GameObject container = new GameObject();

		if(side == 1){
			container = left_container;
			sideColor = red; 
		}else{
			container = right_container;
			sideColor = yellow;
		}

		if(chesses.Count >0){
			foreach(Transform champ in chesses){
				CharacterProperty cp = champ.GetComponent<CharacterProperty>();
				string uiPosName = "champ" + i.ToString();
				GameObject btContainer = container.transform.FindChild(uiPosName).gameObject;
				GameObject champBt = NGUITools.AddChild(btContainer, ChampBtPrefab.gameObject);
				champBt.transform.FindChild("death_sprite").GetComponent<UISprite>().spriteName = "sIcon_" + cp.BigIcon_Name + "_BW";
				
				Transform summonBT = champBt.transform.FindChild("summon_bt").transform;
				summonBT.GetComponent<TweenAlpha>().enabled = false;
				summonBT.GetComponent<UISprite>().color = sideColor;
				summonBT.GetComponent<UISprite>().spriteName = "sIcon_" + cp.BigIcon_Name;
				summonBT.GetComponent<UISprite>().fillAmount = 0.0f;
				summonBT.GetChild(0).GetComponent<SummonChamp>().InsertGF((Transform)chesses[0],champ);
				
				Transform skillBT = champBt.transform.FindChild("skill_bt").transform;
				skillBT.GetComponent<UISprite>().spriteName = "sIcon_" + cp.BigIcon_Name;
				skillBT.GetComponent<UISprite>().color = alpha; 
				skillBT.GetChild(0).GetComponent<UILabel>().color = alpha;
				skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled = false;
				skillBT.GetComponent<UseSkill>().InsertChamp(champ);

				champBts.Add(champBt, champ);
				//Transform gfSkill = champ.GetComponent<SkillSets>().Skills[0];
				//SkillProperty gfSp = gfSkill.GetComponent<SkillProperty>();
				//skillBT.GetChild(0).GetComponent<UILabel>().text = gfSp.WaitingRounds.ToString();
				i++;
			}
		}
	}

	void UpdateChampButtons(){
		foreach(var bt in champBts){
			GameObject champBt = bt.Key as GameObject;  
			Transform summonBT = champBt.transform.FindChild("summon_bt").transform;
			Transform skillBT = champBt.transform.FindChild("skill_bt").transform;

			Color originColor = summonBT.GetComponent<UISprite>().color;

			Transform champ = bt.Value as Transform;
			CharacterProperty cp = champ.GetComponent<CharacterProperty>();
			Transform champSkill = champ.GetComponent<SkillSets>().Skills[0];
			SkillProperty gfSp = champSkill.GetComponent<SkillProperty>();

			//update summon button with cool down info
			float progress = (float)(cp.StandByRounds - cp.WaitRounds) / (float)cp.StandByRounds;
			summonBT.GetComponent<UISprite>().fillAmount = progress; 
			if(progress >= 1.0f){
				if(cp.Death){
					if(summonBT.GetChild(0).GetComponent<UIButton>().isEnabled == false)
						summonBT.GetChild(0).GetComponent<UIButton>().isEnabled = true;
					if(summonBT.GetComponent<TweenAlpha>().enabled == false)
						summonBT.GetComponent<TweenAlpha>().enabled = true;
				}else{
					if(summonBT.GetComponent<TweenAlpha>().enabled == true)
						summonBT.GetComponent<TweenAlpha>().enabled = false;
				}
				//summonBT.GetComponent<TweenAlpha>().Play();
			}else{
				if(cp.Death){
					if(summonBT.GetComponent<UISprite>().color.a != 1.0f)
						summonBT.GetComponent<UISprite>().color = new Color(originColor.r, originColor.g, originColor.b, 1.0f);
				}
				if(summonBT.GetChild(0).GetComponent<UIButton>().isEnabled == true)
					summonBT.GetChild(0).GetComponent<UIButton>().isEnabled = false;
				if(summonBT.GetComponent<TweenAlpha>().enabled == true);
					summonBT.GetComponent<TweenAlpha>().enabled = false;
			}

			//update skiil button with skill cool down info
			if(!cp.Death){
				skillBT.GetComponent<UISprite>().color = Color.white;
				skillBT.GetChild(0).GetComponent<UILabel>().color = Color.white;
				if(summonBT.GetChild(0).GetComponent<UIButton>().isEnabled == true)
					summonBT.GetChild(0).GetComponent<UIButton>().isEnabled = false;
				if(gfSp.SkillReady){
					if(skillBT.GetChild(0).GetComponent<UILabel>().text != "up")
						skillBT.GetChild(0).GetComponent<UILabel>().text = "up";
					if(cp.CmdTimes>0){
						if(skillBT.GetComponent<UIButton>().isEnabled == false)
							skillBT.GetComponent<UIButton>().isEnabled = true;
						if(skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled == false)
							skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled =true;
					}else{
						if(skillBT.GetComponent<UIButton>().isEnabled == true)
							skillBT.GetComponent<UIButton>().isEnabled = false;
						if(skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled == true){
							skillBT.GetChild(0).GetComponent<TweenAlpha>().ResetToBeginning();
							skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled =false;
						}
					}
				}else{
					if(skillBT.GetComponent<UIButton>().isEnabled == true)
						skillBT.GetComponent<UIButton>().isEnabled = false;
					skillBT.GetChild(0).GetComponent<UILabel>().text = gfSp.WaitingRounds.ToString();
					if(skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled == true)
						skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled =false;
				}
			}else{
				if(skillBT.GetComponent<UISprite>().color != alpha)
					skillBT.GetComponent<UISprite>().color = alpha;
				if(skillBT.GetComponent<UIButton>().isEnabled == true)
					skillBT.GetComponent<UIButton>().isEnabled = false;
				if(skillBT.GetChild(0).GetComponent<UILabel>().color != alpha)
					skillBT.GetChild(0).GetComponent<UILabel>().color = alpha; 
				if(skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled == false)
					skillBT.GetChild(0).GetComponent<TweenAlpha>().enabled =false;
			}
		}
	}

	//for new NGUI system
	public void UpdateInfoUI(Transform infoUI, Transform chess){
		if(chess != null){
			CharacterProperty cp = chess.GetComponent<CharacterProperty>();
			CharacterPassive cpp = chess.GetComponent<CharacterPassive>();
			infoUI.gameObject.SetActive(true);
			Transform[] passiveSprite = {infoUI.FindChild("passive"), infoUI.FindChild("passive_A"),infoUI.FindChild("passive_B"),infoUI.FindChild("passive_C")};
			
			MapHelper.FindAnyChildren(infoUI, "big_icon").GetComponent<UISprite>().spriteName = cp.BigIcon_Name;
			MapHelper.FindAnyChildren(infoUI, "name").GetComponent<UILabel>().text = cp.NameString; 
			MapHelper.FindAnyChildren(infoUI, "move_num").GetComponent<UILabel>().text = cp.BuffMoveRange.ToString();
			MapHelper.FindAnyChildren(infoUI, "range_num").GetComponent<UILabel>().text = cp.BuffAtkRange.ToString();
			MapHelper.FindAnyChildren(infoUI, "atk_num").GetComponent<UILabel>().text = cp.Damage.ToString();
			MapHelper.FindAnyChildren(infoUI, "hp_num").GetComponent<UILabel>().text = cp.Hp.ToString();
			MapHelper.FindAnyChildren(infoUI, "critiq_num").GetComponent<UILabel>().text = cp.BuffCriticalHit.ToString();
			MapHelper.FindAnyChildren(infoUI, "def_num").GetComponent<UILabel>().text = cp.ModifiedDefPow.ToString();

			infoUI.FindChild("move").GetComponent<UISprite>().spriteName = GetIcon(BuffType.MoveRange, chess);
			infoUI.FindChild("range").GetComponent<UISprite>().spriteName = GetIcon(BuffType.AttackRange, chess);
			infoUI.FindChild("atk").GetComponent<UISprite>().spriteName = GetIcon(BuffType.Attack, chess);
			infoUI.FindChild("hp").GetComponent<UISprite>().spriteName = GetIcon(BuffType.Hp, chess);
			infoUI.FindChild("critiq").GetComponent<UISprite>().spriteName = GetIcon(BuffType.CriticalHit, chess);
			infoUI.FindChild("def").GetComponent<UISprite>().spriteName = GetIcon(BuffType.Defense, chess);

			if(cpp.PassiveAbility.Length>0){
				int j=0;
				if(cpp.PassiveDict.Values.ElementAt(0)){
					passiveSprite[0].GetComponent<UISprite>().spriteName = cpp.PassiveDict.Keys.ElementAt(0).ToString();
				}
				if(cpp.PassiveDict.Values.ElementAt(1)){
					passiveSprite[1].GetComponent<UISprite>().spriteName = cpp.PassiveDict.Keys.ElementAt(1).ToString();
				}else{
					passiveSprite[1].GetComponent<UISprite>().spriteName = "nothing";
				}
				if(cpp.PassiveDict.Values.ElementAt(2)){
					passiveSprite[2].GetComponent<UISprite>().spriteName = cpp.PassiveDict.Keys.ElementAt(2).ToString();
				}else{
					passiveSprite[2].GetComponent<UISprite>().spriteName = "nothing";
				}
				if(cpp.PassiveDict.Values.ElementAt(3)){
					passiveSprite[3].GetComponent<UISprite>().spriteName = cpp.PassiveDict.Keys.ElementAt(3).ToString();
				}else{
					passiveSprite[3].GetComponent<UISprite>().spriteName = "nothing";
				}
			}else{
				passiveSprite[0].GetComponent<UISprite>().spriteName = "nothing";
				passiveSprite[1].GetComponent<UISprite>().spriteName = "nothing";
				passiveSprite[2].GetComponent<UISprite>().spriteName = "nothing";
				passiveSprite[3].GetComponent<UISprite>().spriteName = "nothing";
			}
		}
	}

	public void ActivateInfoUI(Transform chess){
		Transform infoUI = null;
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		if(cp.Player == PlayerSide){
			infoUI = Left_Info;
			leftChess = chess;
		}else{
			infoUI = Right_Info;
			rightChess = chess;
		}
		infoUI.GetComponent<TweenAlpha>().PlayForward();
		UpdateInfoUI(infoUI, chess);
		ShowSkillBox(chess);
	}

	public void ShowTalkingBox(Transform chess, Transform talkingBox){
		if(talkingBox.parent.GetComponent<UISprite>().color.a == 0.0f)
			ActivateInfoUI(chess);
		talkingBox.GetComponent<TweenAlpha>().PlayForward();
	}

	public void ShowSkillBox(Transform chess){
		GameObject skillContent = GameObject.Find("skill_content");
		GameObject skill = GameObject.Find("skill");
		SkillProperty sProperty = chess.GetComponent<SkillSets>().Skills[0].GetComponent<SkillProperty>();
		skillContent.GetComponent<UILabel>().text = sProperty.info;
		skill.GetComponent<TweenAlpha>().PlayForward();
		skillContent.GetComponent<TweenAlpha>().PlayForward();
	}

	public void HideSkillBox(){
		GameObject skillContent = GameObject.Find("skill_content");
		GameObject skill = GameObject.Find("skill");
		skill.GetComponent<TweenAlpha>().PlayReverse();
		skillContent.GetComponent<TweenAlpha>().PlayReverse();
	}

	//for new NGUI system
	public void DeactivateInfoUI(int side){
		Transform infoUI = null;
		if(side == 1){
			infoUI = Left_Info;
		}else{
			infoUI = Right_Info;
		}
		infoUI.GetComponent<TweenAlpha>().PlayReverse();
		HideSkillBox();
		//infoUI.gameObject.SetActive(false);
	}

	public void DeactivateInfoUI(Transform chess){
		Transform infoUI = null;
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		if(cp.Player == PlayerSide){
			infoUI = Left_Info;
			leftChess = chess;
		}else{
			infoUI = Right_Info;
			rightChess = chess;
		}
		if(infoUI.GetComponent<UISprite>().color.a == 1.0f)
			infoUI.GetComponent<TweenAlpha>().PlayReverse();
		HideSkillBox();
	}

	public void DelayDeactivateInfoUI(int side){
		Transform infoUI = null;
		if(side == 1){
			infoUI = Left_Info;
		}else{
			infoUI = Right_Info;
		}
		infoUI.GetComponent<UIDelayFade>().StartDelay();
	}

	public void DelayDeactivateInfoUI(Transform chess, float delayTime){
		Transform infoUI = null;
		if(chess.GetComponent<CharacterProperty>().Player == PlayerSide){
			infoUI = Left_Info;
		}else{
			infoUI = Right_Info;
		}
		infoUI.GetComponent<UIDelayFade>().TimeToDelay = delayTime;
		infoUI.GetComponent<UIDelayFade>().StartDelay();
	}



	//for new NGUI system
	public void ShowHitStatus(bool critiq, int side){
		if(critiq){
			GameObject hitStatus = null;
			if(side == 1){
				hitStatus = Left_Info.FindChild("hit_label").gameObject;
			}else{
				hitStatus = Right_Info.FindChild("hit_label").gameObject;
			}
			hitStatus.SetActive(true);
			hitStatus.transform.GetComponent<TweenAlpha>().PlayForward();
			hitStatus.transform.GetComponent<UIDelayFade>().StartDelay();
		}
	}

	//for new NGUI system
	void ShowDeadLabel(){
		if(leftChess){
			if(leftChess.GetComponent<CharacterProperty>().Hp<=0)
				Left_Info.FindChild("dead_label").gameObject.SetActive(true);
			else
				Left_Info.FindChild("dead_label").gameObject.SetActive(false);
		}
		if(rightChess){
			if(rightChess.GetComponent<CharacterProperty>().Hp<=0)
				Right_Info.FindChild("dead_label").gameObject.SetActive(true);
			else
				Right_Info.FindChild("dead_label").gameObject.SetActive(false);
		}
	}

	//NGUI character buff info, icon changer
	public string GetIcon(BuffType mode, Transform chess){
		string icon = "icon_";
		CharacterProperty cp = chess.GetComponent<CharacterProperty>();
		switch(mode){
			case BuffType.MoveRange:
				if(cp.BuffMoveRange == cp.MoveRange)
					icon += "move";
				else if(cp.BuffMoveRange > cp.MoveRange)
					icon += "moveUP";
				else
					icon += "moveDN";
				break;
			case BuffType.AttackRange:
				if(cp.BuffAtkRange == cp.AtkRange)
					icon += "renge";
				else if(cp.BuffAtkRange > cp.AtkRange)
					icon += "rengeUP";
				else
					icon += "rengeDN";
				break;
			case BuffType.Attack:
				if(cp.Damage == cp.AtkPower)
					icon += "atk";
				else if(cp.Damage > cp.AtkPower)
					icon += "atkUP";
				else
					icon += "atkDN";
				break;
			case BuffType.Hp:
				if(cp.Hp == cp.MaxHp)
					icon += "hp";
				else if(cp.Hp > cp.MaxHp)
					icon += "hpUP";
				else
					icon += "hpDN";
				break;
			case BuffType.CriticalHit:
				if(cp.BuffCriticalHit == cp.CriticalhitChance)
					icon += "critiq";
				else if(cp.BuffCriticalHit > cp.CriticalhitChance)
					icon += "critiqUP";
				else
					icon += "critiqDN";
				break;
			case BuffType.Defense:
				if(cp.ModifiedDefPow == cp.DefPower)
					icon += "def";
				else if(cp.ModifiedDefPow > cp.DefPower)
					icon += "defUP";
				else
					icon += "defDN";
				break;
		}
		return icon;
	}

	void UpdateUnderBar(){
		redTerritory.GetComponent<UILabel>().text = rc.PlayerATerritory.Count.ToString();
		yelTerritory.GetComponent<UILabel>().text = rc.PlayerBTerritory.Count.ToString();
		LeftRounds = LimitedRounds-rc.roundCounter;
		roundNum.GetComponent<UILabel>().text = LeftRounds.ToString();
	}

	// Update is called once per frame
	void Update () {
		mousePos.x = Input.mousePosition.x;
		mousePos.y = Screen.height-Input.mousePosition.y;

		ShowDeadLabel();

		//for NGUI
		if(Left_Info.GetComponent<UISprite>().color.a == 1.0f){
			UpdateInfoUI(Left_Info, leftChess);
		}
		if(Right_Info.GetComponent<UISprite>().color.a == 1.0f){
			UpdateInfoUI(Right_Info, rightChess);
		}
		
		if(updateChampBts){
			UpdateChampButtons();
			UpdateUnderBar();
		}
	}

	

	public void CastSkills(Transform skill, Transform gf){
		//MainUI mUI = transform.GetComponent<MainUI>();
		currentSel.CancelCmds();
		if(!skill.GetComponent<SkillProperty>().NeedToSelect){
			skill.GetComponent<SkillProperty>().GetRealSkillRate();
			skill.GetComponent<SkillProperty>().PassSkillRate = MapHelper.Success(skill.GetComponent<SkillProperty>().SkillRate);
			skill.GetComponent<SkillProperty>().ActivateSkill();
			gf.GetComponent<CharacterProperty>().Activated = true;
			skill.GetComponent<SkillProperty>().DefaultCDRounds();
			gf.GetComponent<CharacterProperty>().CmdTimes -= 1;
			GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = true;
			currentSel.AnimStateNetWork(gf,AnimVault.AnimState.skill);
			//mUI.TurnFinished(gf, true);
			//update network
			currentSel.SkillCmdNetwork(gf,skill);
		}else{
			currentSel.skillCommand(skill);
		}
	}	
}
