using UnityEngine;
using System.Collections;

public class ShowSkillRange : MonoBehaviour {
	GeneralSelection currentSel;
	MainInfoUI chessUI;
	UseSkill skillBT;
	Transform champ; 

	// Use this for initialization
	void Start () {
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		skillBT = transform.parent.Find("skill_bt").GetComponent<UseSkill>();
	}

	void OnHover(bool isOver){
		if(!currentSel.selectMode){
			champ = skillBT.GetCurrentChamp();
			CharacterProperty cp = champ.GetComponent<CharacterProperty>();
			int playerSide = cp.Player;
			if(currentSel.Playing && chessUI.PlayerSide == playerSide && !cp.Death){
				if(isOver){
					currentSel.RenderSkillRange(champ);
				}else{
					currentSel.CleanMapsMat();
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
