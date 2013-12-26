using UnityEngine;
using System.Collections;

public class BeamGui : MonoBehaviour {
	
	public BeamController Beamer;
	public CustomColorsController Custom;
	public StoneController Stoner;
	public OrbitViewer   Orbiter;
	public GameObject    HoloGuy;
	public GameObject    SpiritGuy;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		if (GUI.Button (new Rect (5,5,80,20), "Beam Out")) {
			Orbiter.SetTransform(Beamer.gameObject.transform);
			Beamer.BeamOut(false);
		}
		if (GUI.Button (new Rect (90,5,80,20), "Beam In")) {
			Orbiter.SetTransform(Beamer.gameObject.transform);
			Beamer.BeamIn();
		}
		if (GUI.Button (new Rect (5,30,100,20), "Change Eyes")) 
		{
			// point cam at it.
			Orbiter.SetTransform(Custom.gameObject.transform);
			Custom.SetEyeColor(Random.value,Random.value,Random.value);
		}
		if (GUI.Button (new Rect (5,60,100,20), "Change \"Hair\"")) {
			Orbiter.SetTransform(Custom.gameObject.transform);
			Custom.SetHairColor(Random.value,Random.value,Random.value);
		}
		if (GUI.Button (new Rect (5,90,100,20), "Change Skin")) {
			Orbiter.SetTransform(Custom.gameObject.transform);
			Custom.SetSkinColor(Random.value,Random.value,Random.value);
		}
		if (GUI.Button (new Rect (5,120,100,20), "Stone Lerpz")) {
			Orbiter.SetTransform(Stoner.gameObject.transform);
			Stoner.TurnToStone();
		}
		
		if (GUI.Button (new Rect (5,150,100,20), "Stone To Flesh")) 
		{
			Orbiter.SetTransform(Stoner.gameObject.transform);
			Stoner.StoneToFlesh();
		}
		if (GUI.Button (new Rect(5,180,100,20),"View Hologram"))
		{
			Orbiter.SetTransform(HoloGuy.transform);
		}
		
		if (GUI.Button (new Rect(5,210,100,20),"View Spirit"))
		{
			Orbiter.SetTransform(SpiritGuy.transform);
		}
		
	}
}
