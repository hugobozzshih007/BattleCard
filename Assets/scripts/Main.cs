using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	
	private MapNode[] mNodes;
	public Transform prefab;
	
	// Use this for initialization
	void Start () {
		
		mNodes = new MapNode[7];
		
		for (int i=0; i<mNodes.Length; i++)
		{
			mNodes[i] = new MapNode();
			mNodes[i].mUnit = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
						
			mNodes[i].mId = i;			
			
			if (i>0)
			{
				//Debug.Log(i % 6);
				mNodes[0].AddNeighbor((i % 6), mNodes[i], true);
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
