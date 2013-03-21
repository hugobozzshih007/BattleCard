using UnityEngine;
using System.Collections;

public class InsertCharacter : MonoBehaviour {
	Transform award;
	
	
	// Use this for initialization
	void Start () {
		print(Application.loadedLevelName);
		award = GameObject.Find("Award").transform;
		if(award!=null){
			string awardString = award.GetComponent<KeepAward>().Award;
			GameObject awardPrefab =Instantiate(Resources.Load(awardString)) as GameObject;
			awardPrefab.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
