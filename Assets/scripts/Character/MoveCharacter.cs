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
	const float timeToReach = 0.6f;
	const float timeToRotate = 0.2f;
	float accl = 0.1f;
	float blendRate = 0.022f;
	float s, t,r;
	int Step = 0;
	int init = 0;
	public bool MoveMode = false;
	bool facingTower = false;
	GeneralSelection currentSelect;
	CharacterPassive cPass;

	FollowCam fCam;
	// Use this for initialization
	void Start () {
		currentSelect = transform.GetComponent<GeneralSelection>();
	
		fCam = transform.GetComponent<FollowCam>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Step>0){
			t += Time.deltaTime/timeToReach;
			r += Time.deltaTime/timeToRotate;
			//Vector3 fowardPos = Chess.transform.forward*accl+Chess.transform.position;
			Vector3 plannedPos = new Vector3();
			Vector3 currentPos = Chess.transform.position;
			
			Vector3 relativePos = target - currentPos;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			float yAngle = Mathf.LerpAngle(OldRotation.eulerAngles.y, rotation.eulerAngles.y, r);
			Chess.transform.rotation = Quaternion.Euler(new Vector3(OldRotation.eulerAngles.x, yAngle, OldRotation.eulerAngles.z));
			
			if(cPass.PassiveDict[PassiveType.Flying]){
				plannedPos = Vector3.Lerp(startPosition, target, t);
			}else{
				plannedPos = Vector3.Lerp(startPosition, target, t);
			}
			
        	//Chess.transform.position = Vector3.Lerp(fowardPos,plannedPos, blendRate);
			Chess.transform.position = plannedPos;
			fCam.CamFollowMe(Chess);
			
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
			float yAngle = Mathf.LerpAngle(OldRotation.eulerAngles.y, rotation.eulerAngles.y, s);
			Chess.transform.rotation = Quaternion.Euler(new Vector3(OldRotation.eulerAngles.x, yAngle, OldRotation.eulerAngles.z));
			//networkView.RPC("RPCRotateCharacter",RPCMode.Others, Chess.name, OldRotation, rotation, s);
			//float angle = Quaternion.Angle(Chess.transform.rotation, rotation);
			float angle = Mathf.Abs(rotation.eulerAngles.y - yAngle);
			if(Mathf.RoundToInt(angle) == 360){
				angle -= 360.0f;
				angle = Mathf.Abs(angle);
			}
			if(angle<=1.0f){
				s=0;
				facingTower=false;
				if(!currentSelect.NpcPlaying)
					currentSelect.CancelCmds();
				MoveMode = false;
				Chess.GetComponent<CharacterProperty>().Moved = true;
				Chess.GetComponent<CharacterProperty>().CmdTimes -= 1;
				currentSelect.TurnFinished(Chess, false);
				//networkView.RPC("RPCUpdateChessMoved", RPCMode.Others,Chess.name,true);
				currentSelect.updateAllCharactersPowers();
				if(chessModel.GetComponent<AnimVault>()!=null){
					//chessModel.GetComponent<AnimVault>().CurrentState = AnimVault.AnimState.idle;
					currentSelect.AnimStateNetWork(Chess, AnimVault.AnimState.idle);
				}
				if(!currentSelect.Playing){
					Transform npc = GameObject.Find("NpcPlayer").transform;
					NpcPlayer npcPlayer = npc.GetComponent<NpcPlayer>();
					npcPlayer.InPause = true;
				}
				//if(mUI.InTutorial){
				//	GameObject.Find("InitStage").GetComponent<InitStage>().ShowBuff = true;
				//}
				//set machine free
				GameObject.Find("StatusMachine").GetComponent<StatusMachine>().InBusy = false;
				//cancel move extra buff
				Chess.GetComponent<BuffList>().ExtraDict[BuffType.MoveRange] = 0;
			}
		}
		
	}
				
	public void SetSteps(Transform chess, IList t){
		Chess = chess;
		tower = GetClosetChess(Chess);
		if(tower == null)
			tower = chess;
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
		fCam.timeSeg = 0.0f;
	}
	
	public Transform GetClosetChess(Transform chess){
		Transform target = null;
		IList targets = new List<Transform>();
		RoundCounter rc = transform.GetComponent<RoundCounter>();
		if(chess.GetComponent<CharacterProperty>().Player==1){
			foreach(Transform t in rc.PlayerBChesses){
				if(!t.GetComponent<CharacterProperty>().Death)
					targets.Add(t);
			}
		}else{
			foreach(Transform t in rc.PlayerAChesses){
				if(!t.GetComponent<CharacterProperty>().Death)
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
