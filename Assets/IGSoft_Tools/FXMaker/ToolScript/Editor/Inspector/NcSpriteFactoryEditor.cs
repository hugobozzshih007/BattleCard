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

[CustomEditor(typeof(NcSpriteFactory))]

public class NcSpriteFactoryEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcSpriteFactory		m_Sel;
	protected	FxmPopupManager	m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcSpriteFactory;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcSpriteFactory");
   }

    void OnDisable()
    {
		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
			m_FxmPopupManager.CloseNcPrefabPopup();
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);

		int				nClickIndex		= -1;
		int				nClickButton	= 0;
		Rect			rect;
		int				nLeftWidth		= 35;
		int				nAddHeight		= 30;
		int				nDelWidth		= 35;
		int				nLineHeight		= 18;
		int				nSpriteHeight	= nLeftWidth;
		List<NcSpriteFactory.NcSpriteNode>	spriteList = m_Sel.m_SpriteList;

		m_FxmPopupManager = GetFxmPopupManager();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
			m_UndoManager.CheckUndo();
			// --------------------------------------------------------------
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			EditorGUILayout.Space();
			m_Sel.m_SpriteType			= (NcSpriteFactory.SPRITE_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_SpriteType"), m_Sel.m_SpriteType);

			// --------------------------------------------------------------
			if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation && m_Sel.gameObject.GetComponent("NcSpriteAnimation") == null)
			{
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(rect, GetHelpContent("Add NcSpriteAnimation Component"), true))
						m_Sel.gameObject.AddComponent("NcSpriteAnimation");
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
			}
			// --------------------------------------------------------------
			if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture && m_Sel.gameObject.GetComponent("NcSpriteTexture") == null)
			{
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(rect, GetHelpContent("Add NcSpriteTexture Component"), true))
						m_Sel.gameObject.AddComponent("NcSpriteTexture");
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();

			// --------------------------------------------------------------
			int nSelIndex				= EditorGUILayout.IntSlider(GetHelpContent("m_nCurrentIndex")				, m_Sel.m_nCurrentIndex, 0, (spriteList==null ? 0 : spriteList.Count));
			if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture)
			{
				m_Sel.m_fUvScale		= EditorGUILayout.FloatField(GetHelpContent("m_fUvScale")					, m_Sel.m_fUvScale);
			}
			m_Sel.m_nMaxAtlasTextureSize= EditorGUILayout.IntPopup("nMaxAtlasTextureSize", m_Sel.m_nMaxAtlasTextureSize, NgEnum.m_TextureSizeStrings, NgEnum.m_TextureSizeIntters);
// 			m_Sel.m_AtlasMaterial		= (Material)EditorGUILayout.ObjectField(GetHelpContent("m_AtlasMaterial")	, m_Sel.m_AtlasMaterial, typeof(Material), false);

			if (m_Sel.m_nCurrentIndex != nSelIndex)
			{
				m_Sel.m_nCurrentIndex = nSelIndex;
				m_Sel.SetSprite(nSelIndex, false);
			}

			// check

			// Add Button ------------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nAddHeight*2));
			{
				Rect lineRect = FXMakerLayout.GetInnerVerticalRect(rect, 2, 0, 1);
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 0, 1), GetHelpContent("Add Sprite")))
				{
					bClickButton	= true;
					m_Sel.AddSpriteNode();
				}
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 1, 1), GetHelpContent("Build Sprite")))
				{
					bClickButton	= true;
					BuildSpriteAtlas(m_Sel.renderer.sharedMaterial);
				}
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 2, 1), GetHelpContent("Clear All"), (0 < m_Sel.GetSpriteNodeCount())))
				{
					bClickButton	= true;
					if (m_FxmPopupManager != null)
						m_FxmPopupManager.CloseNcPrefabPopup();
					m_Sel.ClearAllSpriteNode();
				}
				lineRect = FXMakerLayout.GetInnerVerticalRect(rect, 2, 1, 1);
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 0, 1), GetHelpContent("Sequence"), (0 < m_Sel.GetSpriteNodeCount())))
				{
					m_Sel.m_bSequenceMode	= true;
					bClickButton			= true;
					m_Sel.SetSprite(0, false);
				}
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 1, 1), GetHelpContent("NewMaterial"), true))
				{
					Material	newMat		= new Material(m_Sel.renderer.sharedMaterial);
					string		matPath		= AssetDatabase.GetAssetPath(m_Sel.renderer.sharedMaterial);
					NgMaterial.SaveMaterial(newMat, NgFile.TrimFilenameExt(matPath), m_Sel.name); 
					m_Sel.renderer.sharedMaterial = newMat;
// 					m_Sel.renderer.sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath(savePath, typeof(Material));
				}

 				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();

			// Select ShotType -------------------------------------------------
//			showType		= (NcSpriteFactory.SHOW_TYPE)EditorGUILayout.EnumPopup		(GetHelpContent("m_ShowType")	, showType);
			// --------------------------------------------------------------
			EditorGUILayout.Space();
			NcSpriteFactory.SHOW_TYPE showType = (NcSpriteFactory.SHOW_TYPE)EditorPrefs.GetInt("NcSpriteFactory.SHOW_TYPE", 0);
	
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
			{
				showType	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 0, 1), showType==NcSpriteFactory.SHOW_TYPE.NONE		, GetHelpContent("NONE")		, true) ? NcSpriteFactory.SHOW_TYPE.NONE	: showType;
				showType	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 1, 1), showType==NcSpriteFactory.SHOW_TYPE.ALL		, GetHelpContent("ALL")			, true) ? NcSpriteFactory.SHOW_TYPE.ALL		: showType;
				if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
				{
					showType= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 2, 1), showType==NcSpriteFactory.SHOW_TYPE.SPRITE	, GetHelpContent("SPRITE")		, true) ? NcSpriteFactory.SHOW_TYPE.SPRITE		: showType;
					showType= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 3, 1), showType==NcSpriteFactory.SHOW_TYPE.ANIMATION, GetHelpContent("ANIMATION")	, true) ? NcSpriteFactory.SHOW_TYPE.ANIMATION	: showType;
				}
				showType	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 4, 1), showType==NcSpriteFactory.SHOW_TYPE.EFFECT	, GetHelpContent("EFFECT")		, true) ? NcSpriteFactory.SHOW_TYPE.EFFECT		: showType;
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();

			EditorPrefs.SetInt("NcSpriteFactory.SHOW_TYPE", ((int)showType));

			// Show Option -------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
			{
				m_Sel.m_bShowEffect			= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 0, 1), m_Sel.m_bShowEffect	, GetHelpContent("m_bShowEffect")	, true);
				if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
				{
					m_Sel.m_bTestMode		= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 1, 1), m_Sel.m_bTestMode		, GetHelpContent("m_bTestMode")		, true);
					m_Sel.m_bSequenceMode	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 2, 1), m_Sel.m_bSequenceMode	, GetHelpContent("m_bSequenceMode")	, true);
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();

			// Node List ------------------------------------------------------
			for (int n = 0; n < (spriteList != null ? spriteList.Count : 0); n++)
			{
				EditorGUILayout.Space();

				EditorGUI.BeginChangeCheck();
				// Load Texture ---------------------------------------------------------
				Texture2D	selTexture = null;
				if (spriteList[n].m_TextureGUID != "")
					selTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(spriteList[n].m_TextureGUID), typeof(Texture2D));

				// Enabled --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
				{
					Rect subRect;
					// enable
					spriteList[n].m_bIncludedAtlas = GUILayout.Toggle(spriteList[n].m_bIncludedAtlas, "Idx", GUILayout.Width(nLeftWidth));
					// change index
					subRect = rect;
					subRect.x += nLeftWidth;
					subRect.width = nLineHeight*2;
					int newPos = EditorGUI.IntPopup(subRect, n, NgConvert.GetIntStrings(0, spriteList.Count), NgConvert.GetIntegers(0, spriteList.Count));
					if (newPos != n)
					{
						NcSpriteFactory.NcSpriteNode node = spriteList[n];
						m_Sel.m_SpriteList.Remove(node);
						m_Sel.m_SpriteList.Insert(newPos, node);
						return;
					}

					// name
					subRect = rect;
					subRect.x += nLeftWidth+nLineHeight*2;
					subRect.width -= nLeftWidth+nLineHeight*2;
 					spriteList[n].m_TextureName = selTexture==null ? "" : selTexture.name;
 					GUI.Label(subRect, (selTexture==null ? "" : "(" + spriteList[n].m_nFrameCount + ") " + selTexture.name));
					GUI.Box(subRect, "");
					GUI.Box(rect, "");

					// delete
					if (GUI.Button(new Rect(subRect.x+subRect.width-nDelWidth, subRect.y, nDelWidth, subRect.height), GetHelpContent("Del")))
					{
						bClickButton	= true;
						if (m_FxmPopupManager != null)
							m_FxmPopupManager.CloseNcPrefabPopup();
						m_Sel.DeleteSpriteNode(n);
						return;
					}
				}
				EditorGUILayout.EndHorizontal();

				// MaxAlpha -------------------------------------------------------------
				spriteList[n].m_fMaxTextureAlpha = EditorGUILayout.FloatField(GetHelpContent("m_fMaxTextureAlpha"), spriteList[n].m_fMaxTextureAlpha);

				// Texture --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nSpriteHeight));
				{
					GUILayout.Label("", GUILayout.Width(nLeftWidth));

					Rect subRect = rect;
					subRect.width = nLeftWidth;
					FXMakerLayout.GetOffsetRect(rect, 0, 5, 0, -5);
					EditorGUI.BeginChangeCheck();
					selTexture	= (Texture2D)EditorGUI.ObjectField(subRect, GetHelpContent(""), selTexture, typeof(Texture2D), false);
					if (EditorGUI.EndChangeCheck())
					{
						if (selTexture != null)
							spriteList[n].m_TextureGUID	= AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selTexture));
					}
					if (selTexture != null)
					{
						if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
						{
							spriteList[n].m_nFrameCount = selTexture.width / selTexture.height;
							spriteList[n].m_nFrameSize	= selTexture.height;
							m_Sel.m_nFrameSize = spriteList[n].m_nFrameSize;
						} else {
							spriteList[n].m_nFrameCount = 1;
							spriteList[n].m_nFrameSize	= 1;
							m_Sel.m_nFrameSize = 1;
						}
					}

					// draw texture
					subRect = FXMakerLayout.GetOffsetRect(rect, nLeftWidth+4, 0, 0, -4);
					if (selTexture != null)
						GUI.DrawTexture(FXMakerLayout.GetOffsetRect(subRect, 0, 0, -nDelWidth, 0), selTexture, ScaleMode.ScaleToFit, true, selTexture.width/selTexture.height);

					// delete
					if (GUI.Button(new Rect(subRect.x+subRect.width-nDelWidth, subRect.y, nDelWidth, subRect.height), GetHelpContent("Rmv")))
					{
						spriteList[n].m_TextureGUID	= "";
						spriteList[n].m_nFrameCount = 0;
						spriteList[n].m_nFrameSize	= 0;
					}
					GUI.Box(rect, "");
				}
				EditorGUILayout.EndHorizontal();

				// Change selIndex
				Event e = Event.current;
				if (e.type == EventType.MouseDown)
					if (rect.Contains(e.mousePosition))
					{
						nClickIndex = n;
						nClickButton = e.button;
					}

				// SpriteNode ----------------------------------------------------------
				if (bClickButton == false)
				{
					if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation && showType == NcSpriteFactory.SHOW_TYPE.ALL || showType == NcSpriteFactory.SHOW_TYPE.SPRITE)
					{
						spriteList[n].m_bLoop			= EditorGUILayout.Toggle	(GetHelpContent("m_bLoop")	, spriteList[n].m_bLoop);
						spriteList[n].m_fTime			= EditorGUILayout.Slider	(GetHelpContent("m_fTime")	, spriteList[n].m_nFrameCount/spriteList[n].m_fFps, 0, 5, null);
						spriteList[n].m_fFps			= EditorGUILayout.Slider	(GetHelpContent("m_fFps")	, spriteList[n].m_nFrameCount/spriteList[n].m_fTime, 50, 1, null);
					}

					if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation && showType == NcSpriteFactory.SHOW_TYPE.ALL || showType == NcSpriteFactory.SHOW_TYPE.ANIMATION)
					{
						spriteList[n].m_nNextSpriteIndex= EditorGUILayout.Popup			("m_nNextSpriteIndex"	, spriteList[n].m_nNextSpriteIndex+1, GetSpriteNodeNames()) - 1;
						spriteList[n].m_nTestMode		= EditorGUILayout.Popup			("m_nTestMode"			, spriteList[n].m_nTestMode, NgConvert.ContentsToStrings(FxmTestControls.GetHcEffectControls_Trans(FxmTestControls.AXIS.Z)), GUILayout.MaxWidth(Screen.width));
						spriteList[n].m_fTestSpeed		= EditorGUILayout.FloatField	("m_fTestSpeed"			, spriteList[n].m_fTestSpeed);

						SetMinValue(ref spriteList[n].m_fTestSpeed, 0.01f);
						SetMinValue(ref spriteList[n].m_fTestSpeed, 0.01f);
					}

					if (showType == NcSpriteFactory.SHOW_TYPE.ALL || showType == NcSpriteFactory.SHOW_TYPE.EFFECT)
					{
						// char effect -------------------------------------------------------------
						spriteList[n].m_EffectPrefab	= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_EffectPrefab")	, spriteList[n].m_EffectPrefab, typeof(GameObject), false, null);

						rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight*0.7f));
						{
							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 0, 1), GetHelpContent("SelEffect"), (m_FxmPopupManager != null)))
								m_FxmPopupManager.ShowSelectPrefabPopup(m_Sel, n, 0, true);
							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 1, 1), GetHelpContent("ClearEffect"), (spriteList[n].m_EffectPrefab != null)))
							{
								bClickButton = true;
								spriteList[n].m_EffectPrefab = null;
							}
							GUILayout.Label("");
						}
						EditorGUILayout.EndHorizontal();

						if (spriteList[n].m_EffectPrefab != null)
						{
							if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
							{
								spriteList[n].m_nEffectFrame	= EditorGUILayout.IntSlider		(GetHelpContent("m_nEffectFrame")		, spriteList[n].m_nEffectFrame, 0, spriteList[n].m_nFrameCount, null);
								spriteList[n].m_bEffectOnlyFirst= EditorGUILayout.Toggle		(GetHelpContent("m_bEffectOnlyFirst")	, spriteList[n].m_bEffectOnlyFirst);
							}
							spriteList[n].m_fEffectSpeed		= EditorGUILayout.FloatField	("m_fEffectSpeed"		, spriteList[n].m_fEffectSpeed);
							spriteList[n].m_fEffectScale		= EditorGUILayout.FloatField	("m_fEffectScale"		, spriteList[n].m_fEffectScale);
							spriteList[n].m_EffectPos			= EditorGUILayout.Vector3Field	("m_EffectPos"			, spriteList[n].m_EffectPos, null);
							spriteList[n].m_EffectRot			= EditorGUILayout.Vector3Field	("m_EffectRot"			, spriteList[n].m_EffectRot, null);

							SetMinValue(ref spriteList[n].m_fEffectScale, 0.001f);
						}

						EditorGUILayout.Space();

						// char sound -------------------------------------------------------------
						spriteList[n].m_AudioClip		= (AudioClip)EditorGUILayout.ObjectField(GetHelpContent("m_AudioClip")		, spriteList[n].m_AudioClip, typeof(AudioClip), false, null);

						rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight*0.7f));
						{
// 							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 0, 1), GetHelpContent("SelAudio"), (m_FxmPopupManager != null)))
//								m_FxmPopupManager.ShowSelectAudioClipPopup(m_Sel);
							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 1, 1), GetHelpContent("ClearAudio"), (spriteList[n].m_AudioClip != null)))
							{
								bClickButton = true;
								spriteList[n].m_AudioClip = null;
							}
							GUILayout.Label("");
						}
						EditorGUILayout.EndHorizontal();

						if (spriteList[n].m_AudioClip != null)
						{
							if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
							{
								spriteList[n].m_nSoundFrame		= EditorGUILayout.IntSlider	(GetHelpContent("m_nSoundFrame")		, spriteList[n].m_nSoundFrame, 0, spriteList[n].m_nFrameCount, null);
								spriteList[n].m_bSoundOnlyFirst	= EditorGUILayout.Toggle	(GetHelpContent("m_bSoundOnlyFirst")	, spriteList[n].m_bSoundOnlyFirst);
							}
							spriteList[n].m_bSoundLoop			= EditorGUILayout.Toggle	(GetHelpContent("m_bSoundLoop")			, spriteList[n].m_bSoundLoop);
							spriteList[n].m_fSoundVolume		= EditorGUILayout.Slider	(GetHelpContent("m_fSoundVolume")		, spriteList[n].m_fSoundVolume, 0, 1.0f, null);
							spriteList[n].m_fSoundPitch			= EditorGUILayout.Slider	(GetHelpContent("m_fSoundPitch")		, spriteList[n].m_fSoundPitch, -3, 3.0f, null);
						}
					}
				}

				if (EditorGUI.EndChangeCheck())
					nClickIndex = n;

				selTexture = null;
			}

			// Select Node ----------------------------------------------------
			if (0 <= nClickIndex)
			{
				m_Sel.SetSprite(nClickIndex, false);
				if (m_Sel.m_bTestMode && 0 <= spriteList[nClickIndex].m_nTestMode && GetFXMakerMain())
					GetFXMakerMain().GetFXMakerControls().SetTransIndex(spriteList[nClickIndex].m_nTestMode, (4 <= spriteList[nClickIndex].m_nTestMode ? 1.8f : 1.0f), spriteList[nClickIndex].m_fTestSpeed);
				// Rotate
				if (nClickButton == 1)
					m_Sel.transform.Rotate(0, 180, 0);
				nClickIndex		= -1;
				bClickButton	= true;
			}

			m_UndoManager.CheckDirty();
		}
		// --------------------------------------------------------------
		if ((EditorGUI.EndChangeCheck() || bClickButton) && GetFXMakerMain())
			GetFXMakerMain().CreateCurrentInstanceEffect(true);
		// ---------------------------------------------------------------------
		if (GUI.tooltip != "")
			m_LastTooltip	= GUI.tooltip;
		HelpBox(m_LastTooltip);
	}

	// ----------------------------------------------------------------------------------
	string[] GetSpriteNodeNames()
	{
		List<NcSpriteFactory.NcSpriteNode>	spriteList	= m_Sel.m_SpriteList;
		string[]						retNames	= new string[(spriteList != null ? spriteList.Count : 0) + 1];

		retNames[0] = string.Format("{0} {1}", "X", "None");
		for (int n = 0; n < (spriteList != null ? spriteList.Count : 0); n++)
		{
			if (spriteList[n].m_bIncludedAtlas == false || spriteList[n].m_TextureGUID == "")
				continue;

			Texture2D	selTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(spriteList[n].m_TextureGUID), typeof(Texture2D));
			retNames[n+1] = string.Format("{0} {1}", n, selTexture.name);
		}
		return retNames;
	}

	// ----------------------------------------------------------------------------------
	void BuildSpriteAtlas(Material tarMat)
	{
		if (m_Sel.renderer == null)
		{
			Debug.LogWarning("m_Sel.renderer is nul!!!");
			return;
		}
		if (m_Sel.renderer.sharedMaterial == null)
		{
			Debug.LogWarning("m_Sel.renderer.sharedMaterial is nul!!!");
			return;
		}
		if (m_Sel.m_SpriteList == null || m_Sel.m_SpriteList.Count < 1)
			return;

		Texture2D	AtlasTexture;
		if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
			 AtlasTexture = BuildAnimationSpriteAtlas();
		else AtlasTexture = BuildUVSpriteAtlas();

		byte[]	bytes		= AtlasTexture.EncodeToPNG();
		string	pathTexture	= NgFile.TrimFileExt(AssetDatabase.GetAssetPath(tarMat)) + ".png";

		// save texture
		File.WriteAllBytes(pathTexture, bytes);
		Debug.Log(pathTexture);
 		AssetDatabase.Refresh();
//		ReimportTexture(pathTexture, m_wrapMode, m_filterMode, m_anisoLevel, m_nSpriteTextureSizes[(int)m_fSpriteTextureIndex], m_SpriteTextureFormat[(int)m_fSpriteTextureFormatIdx]);

		// Material
		tarMat.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath(pathTexture, typeof(Texture));
		AssetDatabase.SaveAssets();
	}

	Texture2D BuildUVSpriteAtlas()
	{
		Texture2D[] textures	= new Texture2D[m_Sel.m_SpriteList.Count];
		Rect[]		textureRect	= new Rect[m_Sel.m_SpriteList.Count];

		for (int n = 0; n < m_Sel.m_SpriteList.Count; n++)
		{
			if (m_Sel.m_SpriteList[n].m_bIncludedAtlas == false || m_Sel.m_SpriteList[n].m_TextureGUID == "")
			{
				textures[n] = null;
				continue;
			}

			Texture2D	selTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(m_Sel.m_SpriteList[n].m_TextureGUID), typeof(Texture2D));
			SetSourceTexture(selTexture);
			m_Sel.m_SpriteList[n].m_nFrameCount = 1;
			m_Sel.m_SpriteList[n].m_nFrameSize	= 1;
			m_Sel.m_nFrameSize = 1;
			textures[n]		= selTexture;
			textureRect[n]	= new Rect(0, 0, selTexture.width, selTexture.height);
		}

		Color		clearColor		= new Color(0, 0, 0, 0);
		Texture2D	AtlasTexture	= new Texture2D(32, 32, TextureFormat.ARGB32, false);
		Rect[]		packRects		= AtlasTexture.PackTextures(textures, 3, m_Sel.m_nMaxAtlasTextureSize);
		m_Sel.m_nTilingX			= 1;
		m_Sel.m_nTilingY			= 1;
		m_Sel.m_fTextureRatio		= AtlasTexture.width / (float)AtlasTexture.height;

		// clear
		for (int x = 0; x < AtlasTexture.width; x++)
			for (int y = 0; y < AtlasTexture.height; y++)
				AtlasTexture.SetPixel(x, y, clearColor);

		// copy
		for (int n = 0; n < m_Sel.m_SpriteList.Count; n++)
		{
			if (m_Sel.m_SpriteList[n].m_bIncludedAtlas == false || m_Sel.m_SpriteList[n].m_TextureGUID == "")
				continue;
			m_Sel.m_SpriteList[n].m_nStartFrame = 0;

			m_Sel.m_SpriteList[n].m_UvRect = packRects[n];
// 			Debug.Log(packRects[n]);
// 			Debug.Log(textureRect[n]);
// 			Debug.Log(new Rect((int)(packRects[n].x*AtlasTexture.width), (int)(packRects[n].y*AtlasTexture.height), (int)(packRects[n].width*AtlasTexture.width), (int)(packRects[n].height*AtlasTexture.height)));

			// 알파조정 처리
			Color[]	colBuf = textures[n].GetPixels();
			for (int an = 0; an < colBuf.Length; an++)
				if (m_Sel.m_SpriteList[n].m_fMaxTextureAlpha < colBuf[an].a)
					colBuf[an].a = m_Sel.m_SpriteList[n].m_fMaxTextureAlpha;

			AtlasTexture.SetPixels((int)(packRects[n].x*AtlasTexture.width), (int)(packRects[n].y*AtlasTexture.height), (int)(packRects[n].width*AtlasTexture.width), (int)(packRects[n].height*AtlasTexture.height), colBuf);
		}
		return AtlasTexture;
	}

	Texture2D BuildAnimationSpriteAtlas()
	{
		int		nTotalFrame	= 0;

		for (int n = 0; n < m_Sel.m_SpriteList.Count; n++)
		{
			if (m_Sel.m_SpriteList[n].m_bIncludedAtlas == false || m_Sel.m_SpriteList[n].m_TextureGUID == "")
				continue;

			Texture2D	selTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(m_Sel.m_SpriteList[n].m_TextureGUID), typeof(Texture2D));
			m_Sel.m_SpriteList[n].m_nFrameCount = selTexture.width / selTexture.height;
			m_Sel.m_SpriteList[n].m_nFrameSize	= selTexture.height;
			m_Sel.m_nFrameSize = m_Sel.m_SpriteList[n].m_nFrameSize;

			nTotalFrame += m_Sel.m_SpriteList[n].m_nFrameCount;
		}

		int			nTexSize		= GetTextureSize(nTotalFrame, m_Sel.m_nFrameSize);
		int			nCapSize		= m_Sel.m_nFrameSize;
		int			nMaxCount		= (nTexSize / nCapSize) * (nTexSize / nCapSize);
		int			nTexHeight		= (nTotalFrame <= nMaxCount / 2 ? nTexSize / 2 : nTexSize);
		Texture2D	AtlasTexture	= new Texture2D(nTexSize, nTexHeight, TextureFormat.ARGB32, false);
		Color		clearColor		= new Color(0, 0, 0, 0);

		m_Sel.m_nTilingX			= nTexSize / nCapSize;
		m_Sel.m_nTilingY			= nTexHeight / nCapSize;
// 		Debug.Log(nMaxCount);
// 		Debug.Log(nTexHeight);
// 		Debug.Log(m_Sel.m_nTilingY);

		// clear
		for (int x = 0; x < AtlasTexture.width; x++)
			for (int y = 0; y < AtlasTexture.height; y++)
				AtlasTexture.SetPixel(x, y, clearColor);

		// copy
		int nSaveCount = 0;
		for (int n = 0; n < m_Sel.m_SpriteList.Count; n++)
		{
			if (m_Sel.m_SpriteList[n].m_bIncludedAtlas == false || m_Sel.m_SpriteList[n].m_TextureGUID == "")
				continue;
			m_Sel.m_SpriteList[n].m_nStartFrame = nSaveCount;

			// Load Texture ---------------------------------------------------------
			Texture2D	selTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(m_Sel.m_SpriteList[n].m_TextureGUID), typeof(Texture2D));
			SetSourceTexture(selTexture);

			for (int sn = 0; sn < m_Sel.m_SpriteList[n].m_nFrameCount; sn++)
			{
				Color[] colBuf = selTexture.GetPixels(m_Sel.m_nFrameSize*sn, 0, m_Sel.m_nFrameSize, m_Sel.m_nFrameSize);

				// 알파조정 처리
				for (int an = 0; an < colBuf.Length; an++)
					if (m_Sel.m_SpriteList[n].m_fMaxTextureAlpha < colBuf[an].a)
						colBuf[an].a = m_Sel.m_SpriteList[n].m_fMaxTextureAlpha;

				AtlasTexture.SetPixels(((nSaveCount) % (nTexSize/nCapSize)) * nCapSize, nTexHeight - (((nSaveCount) / (nTexSize/nCapSize) + 1) * nCapSize), nCapSize, nCapSize, colBuf);
				nSaveCount++;
			}

			selTexture = null;
		}

		return AtlasTexture;
	}

	int GetTextureSize(int nResultTotalFrame, int nResultCaptureSize)
	{
		if (nResultTotalFrame <= 1)		return nResultCaptureSize * 1;
		if (nResultTotalFrame <= 4)		return nResultCaptureSize * 2;
		if (nResultTotalFrame <= 16)	return nResultCaptureSize * 4;
		if (nResultTotalFrame <= 64)	return nResultCaptureSize * 8;
		if (nResultTotalFrame <= 256)	return nResultCaptureSize * 16;
		if (nResultTotalFrame <= 1024)	return nResultCaptureSize * 32;
		return nResultCaptureSize * 64;
	}

	void SetSourceTexture(Texture2D tex)
	{
		string texturePath;

		texturePath = AssetDatabase.GetAssetPath(tex);
		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(texturePath);
		if (!importer.isReadable || importer.textureFormat != TextureImporterFormat.ARGB32 || importer.npotScale != TextureImporterNPOTScale.None)
		{
			importer.isReadable		= true;
			importer.maxTextureSize	= 4096;
			importer.textureFormat	= TextureImporterFormat.ARGB32;
			importer.npotScale		= TextureImporterNPOTScale.None;
			AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
		}
	}

	// ----------------------------------------------------------------------------------
	Rect GetSpriteRect(int line)
	{
		int		nLineWidth	= 100;
		int		nLineHeight	= 100;

		return new Rect(0, line * nLineHeight, nLineWidth, nLineHeight);
	}

	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcSpriteFactory(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcSpriteFactory("");
		base.HelpBox(str);
	}
}
