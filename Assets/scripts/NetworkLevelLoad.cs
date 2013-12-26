using UnityEngine;
using System.Collections;

public class NetworkLevelLoad : MonoBehaviour {
	
	public string SupportedNetworkLevels = "summon_land";
	public string disconnectedLevel = "opening";
	
	private int lastLevelPrefix = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void Awake(){
		DontDestroyOnLoad(this);
		networkView.group = 1;
		Application.LoadLevel(disconnectedLevel);
	}
	
	void OnGUI(){
		if(Network.peerType != NetworkPeerType.Disconnected){
			GUILayout.BeginArea(new Rect(0, Screen.height - 30, Screen.width, 30));
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(SupportedNetworkLevels)){
				Network.RemoveRPCsInGroup(0);
				Network.RemoveRPCsInGroup(1);
				networkView.RPC( "LoadLevel", RPCMode.AllBuffered, SupportedNetworkLevels, lastLevelPrefix + 1);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
	
	[RPC]
	void LoadLevel(string level, int levelPrefix){
		Debug.Log("Loading level " + level + " with prefix " + levelPrefix);
		lastLevelPrefix = levelPrefix;
		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);
		// We need to stop receiving because first the level must be loaded.
		// Once the level is loaded, RPC's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		
		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data
		Network.SetSendingEnabled(0, true);
		
		GameObject[] allObj = (GameObject[])GameObject.FindObjectsOfType(typeof( GameObject));
		
		foreach(GameObject gameObj in allObj){
			gameObj.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);	
		}
		
	}
	
	void OnDisconnectedFromServer (){
		Application.LoadLevel(disconnectedLevel);
	}
}
