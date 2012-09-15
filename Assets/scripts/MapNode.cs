using UnityEngine;
using System;

public class MapNode 
{
	public int mId;
	private NodeType mType;
	private MapNode[] mNeighbor;
	public GameObject mUnit;

	public MapNode ()
	{
		mType = NodeType.Normal;
		mNeighbor = new MapNode[6];
	}
			
	public void AddNeighbor (int index, MapNode node, bool assignPos = false)
	{
		
		
		if (mNeighbor [index] == null) {
			
			mNeighbor [index] = node;
			
			if (assignPos)
			{
				node.mUnit.transform.Translate(GetNeighborPosition(index));
			}
			
			int oppsite = GetOppsiteEdge (index);
			node.AddNeighbor (oppsite, this);
				
			//Check connect left node
			node.CheckConnect (index, node, true);
				
			//Check connect right node
			node.CheckConnect (index, node, false);
		}			
	}
		
	public void CheckConnect (int index, MapNode newNode, bool isLeft = true)
	{
		MapNode node = null;
		int edgeIndex = -1;
		int nodeConnectIndex = -1;
		if (isLeft) {
			edgeIndex = GetLeftEdge (index);
			node = mNeighbor [edgeIndex];
			
			nodeConnectIndex = GetLeftEdge (GetOppsiteEdge (edgeIndex));
			Debug.Log(nodeConnectIndex);
		} else {
			edgeIndex = GetRightEdge (index);
			node = mNeighbor [edgeIndex];
			nodeConnectIndex = GetRightEdge (GetOppsiteEdge (edgeIndex));
		}
			
		if (node != null) {
			node.AddNeighbor (nodeConnectIndex, newNode);
		}
	}
		
	public static int GetOppsiteEdge (int index)
	{
		return (index + 3) % 6;
	}
		
	public static int GetLeftEdge (int index)
	{
		int result = index - 1;
		if (result < 0) {
			return 5;
		}
		return result;
	}
		
	public static int GetRightEdge (int index)
	{
		int result = index + 1;
		if (result > 5) {
			return 0;
		}
		return result;
	}
	
	public Vector3 GetNeighborPosition(int index)
	{
		Vector3 myPos = mUnit.transform.position;
		float delta = (float) (4 * Math.Sin(Math.PI / 3));
		
		switch (index)
		{
		case 0:
			myPos.z += 2 * delta;
			break;
		case 1:
			myPos.x += 6;
			myPos.z += delta;
			break;
		case 2:
			myPos.x += 6;
			myPos.z -= delta;
			break;
		case 3:
			myPos.z -= 2 * delta;
			break;
		case 4:
			myPos.x -= 6;
			myPos.z -= delta;
			break;
		case 5:
			myPos.x -= 6;
			myPos.z += delta;
			break;
		}
		
		return myPos;
	}
}


