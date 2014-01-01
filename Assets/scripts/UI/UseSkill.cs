using UnityEngine;
using System.Collections;

public class UseSkill : MonoBehaviour { 
	Transform champ;  
	SystemSound sysSound;
	GeneralSelection currentSel;
	MainInfoUI chessUI;
	int playerSide = 0;
	// Use this for initialization
	void Start () {
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		sysSound = GameObject.Find("SystemSoundB").transform.GetComponent<SystemSound>();
	}

	public void InsertChamp(Transform gf){
		champ = gf;
		playerSide = gf.GetComponent<CharacterProperty>().Player;
	}

	public Transform GetCurrentChamp(){
		if(champ)
			return champ;
		else
			return null; 
	}

	public void ActivateSkill(){
		if(currentSel.Playing && chessUI.PlayerSide == playerSide){
			currentSel.CleanMapsMat();
			Transform champSkill = champ.GetComponent<SkillSets>().Skills[0];
			chessUI.StopSkillRender = false;
			currentSel.ChessInSelection = champ;
			chessUI.CastSkills(champSkill, champ);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
