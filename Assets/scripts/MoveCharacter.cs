using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MoveCharacter : MonoBehaviour {
	
	Transform Chess, tower;
	Transform chessModel;
	Quaternion OldRotation;
	Transform[] pathList;
	Vector3 startPosition;
   	Vector3 target;
	const float timeToReach = 0.8f;
	const float timeToRotate = 0.3f;
	float accl = 0.018f;
	float s, t,r;
	int Step = 0;
	int init = 0;
	public bool MoveMode = false;
	bool facingTower = false;
	selection currentSelect;
	CharacterPassive cPass;
	MainUI mUI;
	// Use this for initialization
	void Start () {
		currentSelect = transform.GetComponent<selection>();
		mUI = transform.GetComponent<MainUI>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Step>0){
			mUI.MainGuiFade = false;
			mUI.SubGuiFade = true;
			t += Time.deltaTime/timeToReach;
			r += Time.deltaTime/timeToRotate;
			Vector3 fowardPos = Chess.transform.forward*accl+Chess.transform.position;
			
			if(cPass.PassiveDict[PassiveType.Flying]){
				
			}
			Vector3 relativePos = target - Chess.transform.position;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			Chess.transform.rotation = Quaternion.Lerp(OldRotation, rotation, r);
			networkView.RPC("RPCRotateCharacter",RPCMode.Others, Chess.name, OldRotation, rotation, r);
			Vector3 plannedPos = Vector3.Lerp(startPosition, target, t);
        	Chess.transform.position = Vector3.Lerp(fowardPos,plannedPos, 0.01f);
			networkView.RPC("RPCMoveCharacter",RPCMode.Others,Chess.name,fowardPos,plannedPos,0.01f);
			float d = Vector3.Distance(Chess.transform.position,target);
			if(d<=0.05f){
				Step-=1;
				init+=1;
				if(Step>0){
					SetDestination();
				}else if(Step ==0){
					init = 0;
					t=0;
					r=0;
					s=0;
					OldRotation = Chess.transform.rotation;
					facingTower=true;
				}
			}
		}
		if(Step==0 && facingTower){
			s+=Time.deltaTime/timeToRotate;
			Vector3 relativePos = tower.transform.position - Chess.transform.position;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			Chess.transform.rotation = Quaternion.Lerp(OldRotation, rotation, s);
			networkView.RPC("RPCRotateCharacter",RPCMode.Others, Chess.name, OldRotation, rotation, s);
			float angle = Quaternion.Angle(Chess.transform.rotation, rotation);
			if(angle<1.0f){
				s=0;
				facingTower=false;
				mUI.MainGuiFade = true;
				mUI.SubGuiFade = false;
				MoveMode = false;
				Chess.GetComponent<CharacterProperty>().Moved = true;
				networkView.RPC("RPCUpdateChessMoved", RPCMode.Others,Chess.name,true);
				currentSelect.updateAllCharactersPowers();
				if(chessModel.GetComponent<AnimVault>()!=null){
					//chessModel.GetComponent<AnimVault>().CurrentState = AnimVault.AnimState.idle;
					currentSelect.AnimStateNetWork(Chess, AnimVault.AnimState.idle);
				}
			}
		}
		
	}
				
	public void SetSteps(Transform chess, IList t){
		Chess = chess;
		tower = GetClosetChess(Chess);
		cPass = Chess.GetComponent<CharacterPassive>();
		chessModel = chess.FindChild("Models");
		if(chessModel.GetComponent<AnimVault>()!=null){
			//chessModel.GetComponent<AnimVault>().CurrentState = AnimVault.AnimState.run;
			currentSelect.AnimStateNetWork(chess, AnimVault.AnimState.run);
		}
		int len = t.Count;
		pathList = new Transform[len];
		t.CopyTo(pathList,0);
		Step = pathList.Length-1;
		if(Step>0){
			SetDestination();
		}
	}
	
	public Transform GetClosetChess(Transform chess){
		Transform target = null;
		IList targets = new List<Transform>();
		RoundCounter rc = transform.GetComponent<RoundCounter>();
		if(chess.GetComponent<CharacterProperty>().Player==1){
			foreach(Transform t in rc.PlayerBChesses){
				if(!t.GetComponent<CharacterProperty>().death)
					targets.Add(t);
			}
		}else{
			foreach(Transform t in rc.PlayerAChesses){
				if(!t.GetComponent<CharacterProperty>().death)
					targets.Add(t);
			}
		}
		Dictionary<float,Transform> sortDict = new Dictionary<float, Transform>();
		if(targets.Count>0){
			foreach(Transform t in targets){
				float dis = Vector3.Distance(chess.transform.position, t.transform.position);
				if(!sortDict.ContainsKey(dis))
					sortDict.Add(dis, t);
			}
			var list = sortDict.Keys.ToList();
			list.Sort();
			target = sortDict[list[0]];
		}else{
			if(chess.GetComponent<CharacterProperty>().Player==1)
				target = GameObject.Find("yellow-tower").transform;
			else
				target = GameObject.Find("red-tower").transform;
		}
		
		return target;
	}
	
	void SetDestination(){
		MoveMode = true;
		float diff = 1.5f;
		OldRotation = Chess.transform.rotation;
		t = 0;	
		r = 0;
		s = 0;
        startPosition =new Vector3(pathList[init].transform.position.x,pathList[init].transform.position.y+1.5f,pathList[init].transform.position.z);
		if(cPass.PassiveDict[PassiveType.Flying])
			diff = 1.5f+800.0f*0.003f;
        target = new Vector3(pathList[init+1].transform.position.x,pathList[init+1].transform.position.y+diff,pathList[init+1].transform.position.z); 
	}
	
	[RPC]
	void RPCMoveCharacter(string chessName, Vector3 start, Vector3 target, float t){
		Transform chess = GameObject.Find(chessName).transform;
		chess.transform.position = Vector3.Lerp(start, target, t);
	}
	
	[RPC]
	void RPCRotateCharacter(string chessName, Quaternion start, Quaternion target, float t){
		Transform chess = GameObject.Find(chessName).transform;
		chess.transform.rotation = Quaternion.Lerp(start, target, t);
	}
	
	[RPC]
	void RPCUpdateChessMoved(string chessName, bool moved){
		Transform chess = GameObject.Find(chessName).transform;
		CharacterProperty chessProperty = chess.GetComponent<CharacterProperty>();
		chessProperty.Moved = moved;
	}
}
