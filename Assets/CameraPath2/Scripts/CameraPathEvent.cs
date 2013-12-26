// Based on Messenger.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
// http://wiki.unity3d.com/index.php?title=CSharpMessenger_Extended
//
// Inspired by and based on Rod Hyde's Messenger:
// http://www.unifycommunity.com/wiki/index.php?title=CSharpMessenger
//
// This is a C# messenger (notification center). It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other. The major improvement from Hyde's implementation is that
// there is more extensive error detection, preventing silent bugs.
//
// Usage example:
// Messenger<float>.AddListener("myEvent", MyEventHandler);
// ...
// Messenger<float>.Broadcast("myEvent", 1.0f);
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.



using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

static internal class MessengerInternal 
{
	
	static public Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
	 
	static public void OnListenerAdding(string eventType, Delegate listenerBeingAdded) 
	{
		if (!eventTable.ContainsKey(eventType)) 
		{
			eventTable.Add(eventType, null);
		}
 
		Delegate d = eventTable[eventType];
		if (d != null && d.GetType() != listenerBeingAdded.GetType()) 
		{
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}
 
	static public void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved) 
	{
		if (eventTable.ContainsKey(eventType)) {
			Delegate d = eventTable[eventType];
 
			if (d == null) 
			{
				throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", eventType));
			} else if (d.GetType() != listenerBeingRemoved.GetType()) {
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
			}
		} else {
			throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", eventType));
		}
	}
 
	static public void OnListenerRemoved(string eventType) 
	{
		if (eventTable[eventType] == null) 
		{
			eventTable.Remove(eventType);
		}
	}
 
	static public BroadcastException CreateBroadcastSignatureException(string eventType) 
	{
		return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
	}
 
	public class BroadcastException : Exception 
	{
		public BroadcastException(string msg)
			: base(msg) {
		}
	}
 
	public class ListenerException : Exception 
	{
		public ListenerException(string msg)
			: base(msg) {
		}
	}
}
	 
	 
	// No parameters
static public class CameraPathEvent {
	
	public static string ANIMATION_STARTED = "cameraPathAnimationStartedEvent";
	public static string ANIMATION_PAUSED = "cameraPathAnimationPausedEvent";
	public static string ANIMATION_STOPPED = "cameraPathAnimationStoppedEvent";
	public static string ANIMATION_FINISHED = "cameraPathAnimationFinishedEvent";
	public static string ANIMATION_LOOPED = "cameraPathAnimationLoopedEvent";
	public static string ANIMATION_PINGPONG = "cameraPathAnimationPingPongedEvent";
	public static string POINT_REACHED = "cameraPathPointRreachedEvent";
	public static string POINT_REACHED_WITH_NUMBER = "cameraPathPointRreachedWithNumberEvent";
	
	private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;
 
	static public void AddListener(string eventType, Callback handler) {
		MessengerInternal.OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback)eventTable[eventType] + handler;
	}
 
	static public void RemoveListener(string eventType, Callback handler) {
		MessengerInternal.OnListenerRemoving(eventType, handler);	
		eventTable[eventType] = (Callback)eventTable[eventType] - handler;
		MessengerInternal.OnListenerRemoved(eventType);
	}
 
	static public void Broadcast(string eventType) 
	{
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d)) {
			Callback callback = d as Callback;
			if (callback != null) {
				callback();
			} else {
				throw MessengerInternal.CreateBroadcastSignatureException(eventType);
			}
		}
	}
}
 
// One parameter
static public class CameraPathEvent<T> {
	
	
	private static Dictionary<string, Delegate> eventTable = MessengerInternal.eventTable;
 
	static public void AddListener(string eventType, Callback<T> handler) {
		MessengerInternal.OnListenerAdding(eventType, handler);
		eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
	}
 
	static public void RemoveListener(string eventType, Callback<T> handler) {
		MessengerInternal.OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
		MessengerInternal.OnListenerRemoved(eventType);
	}
 
	static public void Broadcast(string eventType, T arg1) {
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d)) {
			Callback<T> callback = d as Callback<T>;
			if (callback != null) {
				callback(arg1);
			} else {
				throw MessengerInternal.CreateBroadcastSignatureException(eventType);
			}
		}
	}
}