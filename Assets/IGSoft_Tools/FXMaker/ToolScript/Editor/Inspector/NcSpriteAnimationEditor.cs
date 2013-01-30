// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

[CustomEditor(typeof(NcSpriteAnimation))]

public class NcSpriteAnimationEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcSpriteAnimation		m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcSpriteAnimation;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcSpriteAnimation");
   }

    void OnDisable()
    {
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();
		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_nTilingX				= EditorGUILayout.IntField	(GetHelpContent("m_nTilingX")		, m_Sel.m_nTilingX);
			m_Sel.m_nTilingY				= EditorGUILayout.IntField	(GetHelpContent("m_nTilingY")		, m_Sel.m_nTilingY);
			m_Sel.m_nStartFrame				= EditorGUILayout.IntField	(GetHelpContent("m_nStartFrame")	, m_Sel.m_nStartFrame);
			m_Sel.m_nFrameCount				= EditorGUILayout.IntField	(GetHelpContent("m_nFrameCount")	, m_Sel.m_nFrameCount);

			m_Sel.m_PlayMode				= (NcSpriteAnimation.PLAYMODE)EditorGUILayout.EnumPopup (GetHelpContent("m_PlayMode")		, m_Sel.m_PlayMode, GUILayout.MaxWidth(Screen.width));
			if (m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.SELECT)
				m_Sel.m_fDelayTime			= EditorGUILayout.FloatField(GetHelpContent("m_fDelayTime")		, m_Sel.m_fDelayTime);

			if (m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.RANDOM && m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.SELECT)
			{
				m_Sel.m_bLoop				= EditorGUILayout.Toggle	(GetHelpContent("m_bLoop")			, m_Sel.m_bLoop);
				if (m_Sel.m_bLoop == false)
					m_Sel.m_bAutoDestruct	= EditorGUILayout.Toggle	(GetHelpContent("m_bAutoDestruct")	, m_Sel.m_bAutoDestruct);
				m_Sel.m_fFps				= EditorGUILayout.FloatField(GetHelpContent("m_fFps")			, m_Sel.m_fFps);
			}

			if (m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
				m_Sel.m_nSelectFrame		= EditorGUILayout.IntField	(GetHelpContent("m_nSelectFrame")	, m_Sel.m_nSelectFrame);

			// check
			SetMinValue(ref m_Sel.m_nTilingX, 1);
			SetMinValue(ref m_Sel.m_nTilingY, 1);
			SetMinValue(ref m_Sel.m_fFps, 0.1f);
			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMinValue(ref m_Sel.m_nStartFrame, 0);
			SetMinValue(ref m_Sel.m_nFrameCount, 1);
			SetMinValue(ref m_Sel.m_nSelectFrame, 0);
			SetMaxValue(ref m_Sel.m_nSelectFrame, (0 < m_Sel.m_nFrameCount ? m_Sel.m_nFrameCount-1 : m_Sel.m_nTilingX*m_Sel.m_nTilingY-1));

			if (m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.RANDOM && m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.SELECT)
				EditorGUILayout.TextField(GetHelpContent("DurationTime"), m_Sel.GetDurationTime().ToString());


			// Texture --------------------------------------------------------------
			Rect rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(150));
			{
				GUI.Box(rect, "");
				GUILayout.Label("");

				Rect subRect = rect;
				FXMakerLayout.GetOffsetRect(rect, 0, 5, 0, -5);

				// draw texture
				if (m_Sel.renderer != null && m_Sel.renderer.sharedMaterial != null && m_Sel.renderer.sharedMaterial.mainTexture != null)
				{
					GUI.DrawTexture(subRect, m_Sel.renderer.sharedMaterial.mainTexture, ScaleMode.StretchToFill, true);

					if (m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
					{
						Event	e			= Event.current;
						Vector2	mousePos	= e.mousePosition;
						Rect	calRect		= rect;

						mousePos.x -= calRect.x;
						calRect.x = 0;
						mousePos.y -= calRect.y;
						calRect.y = 0;

						int tileWidth		= (int)(calRect.width  / m_Sel.m_nTilingX);
						int tileHeight		= (int)(calRect.height / m_Sel.m_nTilingY);
						int absSelectPos	= m_Sel.m_nStartFrame + m_Sel.m_nSelectFrame;

						int posx = absSelectPos % m_Sel.m_nTilingX;
						int posy = absSelectPos / m_Sel.m_nTilingX;

						// draw current
						Rect selRect = new Rect(rect.x+posx*tileWidth, rect.y+posy*tileHeight, tileWidth, tileHeight);
						NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(selRect, -1), Color.yellow, 1, false);
						GUI.DrawTexture(subRect, m_Sel.renderer.sharedMaterial.mainTexture, ScaleMode.StretchToFill, true);

						// Change selIndex
						if ((e.type == EventType.MouseDown && e.button == 0) && rect.Contains(e.mousePosition))
						{
							posx = (int)(mousePos.x / tileWidth);
							posy = (int)(mousePos.y / tileHeight);
							m_Sel.m_nSelectFrame = posx + (posy * m_Sel.m_nTilingX) - m_Sel.m_nStartFrame;
							bClickButton = true;
						}
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		m_UndoManager.CheckDirty();
		// --------------------------------------------------------------
		if ((EditorGUI.EndChangeCheck() || bClickButton) && GetFXMakerMain())
			GetFXMakerMain().CreateCurrentInstanceEffect(true);
		// ---------------------------------------------------------------------
		if (GUI.tooltip != "")
			m_LastTooltip	= GUI.tooltip;
		HelpBox(m_LastTooltip);
	}

	// ----------------------------------------------------------------------------------
	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcSpriteAnimation(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcSpriteAnimation("");
		base.HelpBox(str);
	}
}
