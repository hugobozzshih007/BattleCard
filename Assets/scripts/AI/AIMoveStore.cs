using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct AIMoveStore{
	public Transform Chess;  
	public IList CmdsLeft;
	public IList CmdsUsed;
	
	public AIMoveStore(Transform chess){
		Chess = chess;  
		CharacterProperty chessP = chess.GetComponent<CharacterProperty>();
		CmdsLeft = new List<UICommands>();
		CmdsUsed = new List<UICommands>();
		
		if(!chessP.Moved)
			CmdsLeft.Add(UICommands.Move);
		else if(!chessP.Defensed)
			CmdsUsed.Add(UICommands.Move);
		
		if(!chessP.Attacked)
			CmdsLeft.Add(UICommands.Attack);
		else if(!chessP.Defensed)
			CmdsUsed.Add(UICommands.Attack);
		
		if(!chessP.Activated)
			CmdsLeft.Add(UICommands.Skill);
		else
			CmdsUsed.Add(UICommands.Skill);
		
		if(!chessP.Defensed)
			CmdsLeft.Add(UICommands.Defense);
		else
			CmdsUsed.Add(UICommands.Defense);
		
		if(chessP.Summoner)
			CmdsLeft.Add(UICommands.Summon);
		
	}
}
