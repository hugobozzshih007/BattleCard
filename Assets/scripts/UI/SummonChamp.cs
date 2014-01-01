using UnityEngine;
using System.Collections;

public class SummonChamp : MonoBehaviour {
	SystemSound sysSound;
	GeneralSelection currentSel;
	Transform gf, player;
	MainInfoUI chessUI;
	int playerSide = 0;
	// Use this for initialization
	void Start () {
		chessUI = Camera.main.GetComponent<MainInfoUI>();
		currentSel = Camera.main.GetComponent<GeneralSelection>();
		sysSound = GameObject.Find("SystemSoundB").transform.GetComponent<SystemSound>();
	}

	public void InsertGF(Transform leader, Transform champ){
		gf = champ; 
		player = leader;
		playerSide = gf.GetComponent<CharacterProperty>().Player;
	}



	public void ShowTheSummonField(){
		if(!currentSel.reviveMode && currentSel.Playing && chessUI.PlayerSide == playerSide){
			if(gf && player){
				currentSel.CancelCmds();
				sysSound.PlaySound(SysSoundFx.CommandClick);
				currentSel.summonCommand(player,gf);
			}
		}
		transform.GetComponent<UIButton>().isEnabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
