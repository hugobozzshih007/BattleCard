// Camera Path
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://camerapath.jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using System.Collections;

public class SimpleEventListener : MonoBehaviour 
{
	
	void Start () 
	{
		
		CameraPathEvent.AddListener(CameraPathEvent.ANIMATION_STARTED, OnAnimationStarted);
		CameraPathEvent.AddListener(CameraPathEvent.ANIMATION_PAUSED, OnAnimationPaused);
		CameraPathEvent.AddListener(CameraPathEvent.ANIMATION_STOPPED, OnAnimationStopped);
		CameraPathEvent.AddListener(CameraPathEvent.ANIMATION_FINISHED, OnAnimationFinished);
		CameraPathEvent.AddListener(CameraPathEvent.ANIMATION_LOOPED, OnAnimationLooped);
		CameraPathEvent.AddListener(CameraPathEvent.ANIMATION_PINGPONG, OnAnimationPingPonged);
		
		CameraPathEvent.AddListener(CameraPathEvent.POINT_REACHED, OnPointReached);
		CameraPathEvent<int>.AddListener(CameraPathEvent.POINT_REACHED_WITH_NUMBER, OnPointReachedByNumber);
		
	}
	
	private void OnAnimationStarted()
	{
		Debug.Log("The animation has begun");
	}
	
	private void OnAnimationPaused()
	{
		Debug.Log("The animation has been paused");
	}
	
	private void OnAnimationStopped()
	{
		Debug.Log("The animation has been stopped");
	}
	
	private void OnAnimationFinished()
	{
		Debug.Log("The animation has finished");
	}
	
	private void OnAnimationLooped()
	{
		Debug.Log("The animation has looped back to the start");
	}
	
	private void OnAnimationPingPonged()
	{
		Debug.Log("The animation has ping ponged into the other direction");
	}
	
	private void OnPointReached()
	{
		Debug.Log("A point was reached");
	}
	
	private void OnPointReachedByNumber(int pointNumber)
	{
		Debug.Log("The point "+pointNumber+" was reached");
	}
}
