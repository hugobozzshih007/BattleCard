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

[CustomEditor(typeof(NcSpriteTexture))]

public class NcSpriteTextureEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcSpriteTexture		m_Sel;
	protected	FxmPopupManager	m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcSpriteTexture;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcSpriteTexture");
   }

    void OnDisable()
    {
		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
			m_FxmPopupManager.CloseNcPrefabPopup();
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();

		Rect	rect;

		m_FxmPopupManager = GetFxmPopupManager();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_NcSpriteFactoryPrefab	= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_NcSpriteFactoryPrefab"), m_Sel.m_NcSpriteFactoryPrefab, typeof(GameObject), false, null);
			NcSpriteFactory ncSpriteFactory	= (m_Sel.m_NcSpriteFactoryPrefab == null ? null : m_Sel.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>());
			if (ncSpriteFactory != null)
			{
				int nSelIndex	= EditorGUILayout.IntSlider(GetHelpContent("m_nSpriteFactoryIndex")	, m_Sel.m_nSpriteFactoryIndex, 0, ncSpriteFactory.GetSpriteNodeCount()-1);
				if (m_Sel.m_nSpriteFactoryIndex != nSelIndex)
				{
					m_Sel.m_nSpriteFactoryIndex	= nSelIndex;
					ncSpriteFactory.m_nCurrentIndex = nSelIndex;
				}
			}

			// --------------------------------------------------------------
			if (m_Sel.m_NcSpriteFactoryPrefab != null && m_Sel.m_NcSpriteFactoryPrefab.renderer != null && m_Sel.renderer)
				if (m_Sel.m_NcSpriteFactoryPrefab.renderer.sharedMaterial != m_Sel.renderer.sharedMaterial)
					m_Sel.UpdateSpriteMaterial();

			// --------------------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
			{
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 0, 1), GetHelpContent("Select SpriteFactory"), (m_FxmPopupManager != null)))
					m_FxmPopupManager.ShowSelectPrefabPopup(m_Sel, true, 0);
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 1, 1), GetHelpContent("Clear SpriteFactory"), (m_Sel.m_NcSpriteFactoryPrefab != null)))
				{
					bClickButton = true;
					m_Sel.m_NcSpriteFactoryPrefab = null;
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();
			// --------------------------------------------------------------

			if (ncSpriteFactory != null)
			{
				// Texture --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(200));
				{
					GUI.Box(rect, "");
					GUILayout.Label("");

					Rect subRect = rect;
					FXMakerLayout.GetOffsetRect(rect, 0, 5, 0, -5);

					// draw texture
					if (m_Sel.renderer != null && m_Sel.renderer.sharedMaterial != null && m_Sel.renderer.sharedMaterial.mainTexture != null)
					{
						GUI.DrawTexture(subRect, m_Sel.renderer.sharedMaterial.mainTexture, ScaleMode.StretchToFill, true);

						Event	ev	= Event.current;

						for (int n = 0; n < ncSpriteFactory.GetSpriteNodeCount(); n++)
						{
							NcSpriteFactory.NcSpriteNode	ncSNode = ncSpriteFactory.GetSpriteNode(n);

							// draw indexRect
							Rect currRect = new Rect(rect.x+ncSNode.m_UvRect.xMin*rect.width, rect.y+(1-ncSNode.m_UvRect.yMin-ncSNode.m_UvRect.height)*rect.height, ncSNode.m_UvRect.width*rect.width, ncSNode.m_UvRect.height*rect.height);
							NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(currRect, 0), (m_Sel.m_nSpriteFactoryIndex == n ? Color.green : Color.red), 1, false);
							GUI.DrawTexture(subRect, m_Sel.renderer.sharedMaterial.mainTexture, ScaleMode.StretchToFill, true);

							// Change selIndex
							if ((ev.type == EventType.MouseDown) && currRect.Contains(ev.mousePosition))
							{
								m_Sel.m_nSpriteFactoryIndex = n;
								ncSpriteFactory.m_nCurrentIndex = n;
								// Rotate
								if (ev.button == 1)
									m_Sel.transform.Rotate(0, 180, 0);
								bClickButton = true;
							}
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			} else {
				m_Sel.m_nSpriteFactoryIndex	= -1;
			}
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
	// ----------------------------------------------------------------------------------
	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcSpriteTexture(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcSpriteTexture("");
		base.HelpBox(str);
	}
}
