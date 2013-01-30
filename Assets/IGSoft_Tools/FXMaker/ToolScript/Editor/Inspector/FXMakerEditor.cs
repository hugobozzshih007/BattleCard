// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------


// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------
// --------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class FXMakerEditor : Editor
{
	// Attribute ------------------------------------------------------------------------
	protected	string				m_LastTooltip	= "";
	protected	float				m_fButtonHeight	= 23;
	protected	FXMakerUndoManager	m_UndoManager;

	// ----------------------------------------------------------------------------------
	protected FXMakerMain GetFXMakerMain()
	{
		// check instance
		if ((target is Component) && (target as Component).GetComponent<FxmInfoIndexing>())
			return null;

		// find toolmain
		GameObject fxMaker = GameObject.Find("_FXMaker");
		if (Application.isPlaying && fxMaker != null)
			return fxMaker.GetComponent<FXMakerMain>();
		return null;
	}

	protected FxmPopupManager GetFxmPopupManager()
	{
		GameObject fxMaker = GameObject.Find("_FXMaker");
		if (Application.isPlaying && fxMaker != null && fxMaker.GetComponent("FXMakerMain") != null)
			return fxMaker.GetComponentInChildren<FxmPopupManager>();
		return null;
	}

	protected string GetScriptName(Component com)
	{
		string	name	= com.ToString();
		int		start	= name.IndexOf('(');
		int		end		= name.IndexOf(')');
		return name.Substring(start+1, end-start-1);
	}

	protected void AddScriptNameField(Component com)
	{
		EditorGUILayout.TextField(new GUIContent("Script", "Script"), GetScriptName(com));
	}

	protected virtual void HelpBox(string caption)
	{
		GUILayout.Space(10);
		GUILayout.TextArea(caption, GUILayout.Height(130));
		GUILayout.Space(20);
	}

	protected virtual void WarringBox(string caption)
	{
 		FXMakerLayout.GUIColorBackup(Color.red);
		GUILayout.TextArea(caption, GUILayout.Height(80));
		FXMakerLayout.GUIColorRestore();
	}

	protected GUIContent GetHelpContent(string caption, string text)
	{
		if (2 < caption.Length)
			if (caption.Substring(0, 2) == "m_")
				caption = caption.Substring(2);
		return new GUIContent(caption, text);
	}

	protected GUIContent GetCommonContent(string caption)
	{
		string text = FXMakerTooltip.GetHsToolInspector(caption);
		if (2 < caption.Length)
			if (caption.Substring(0, 2) == "m_")
				caption = caption.Substring(2);
		return new GUIContent(caption, text);
	}

	// --------------------------------------------------------------------------------------------------
	protected void SetMinValue(ref float value, float min)
	{
		if (value < min)
			value = min;
	}

	protected void SetMinValue(ref int value, int min)
	{
		if (value < min)
			value = min;
	}

	protected void SetMaxValue(ref float value, float max)
	{
		if (max < value)
			value = max;
	}

	protected void SetMaxValue(ref int value, int max)
	{
		if (max < value)
			value = max;
	}

	// --------------------------------------------------------------------------------------------------
	protected static LayerMask LayerMaskField(GUIContent con, LayerMask selected)
	{
	    List<string>	layers		 = new List<string>();

		for (int i=0; i < 32; i++)
		{
			string layerName = LayerMask.LayerToName(i);
			if (layerName != "")
				layers.Add(layerName);
		}

		selected = EditorGUILayout.MaskField(con, selected, layers.ToArray(), EditorStyles.layerMaskField);
		return selected;
	}
}


