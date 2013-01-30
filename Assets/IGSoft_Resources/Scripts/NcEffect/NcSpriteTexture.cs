// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcSpriteTexture : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		GameObject	m_NcSpriteFactoryPrefab	= null;
	public		int			m_nSpriteFactoryIndex	= 0;

	protected	GameObject	m_EffectObject			= null;
	protected	float		m_fTilingX				= 1;
	protected	float		m_fTilingY				= 1;
	protected	float		m_fOffsetX				= 0;
	protected	float		m_fOffsetY				= 0;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";
		if (m_NcSpriteFactoryPrefab == null || m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() == null)
			return "SCRIPT_EMPTY_SPRITEFACTORY";
		if (1 < GetEditingUvComponentCount())
			return "SCRIPT_DUPERR_EDITINGUV";
		if (renderer == null || renderer.sharedMaterial == null)
			return "SCRIPT_EMPTY_MATERIAL";

		return "";	// no error
	}
#endif

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
#if UNITY_EDITOR
		if (IsCreatingEditObject() == false)
#endif
		if (m_NcSpriteFactoryPrefab == null && gameObject.GetComponent<NcSpriteFactory>() != null)
			m_NcSpriteFactoryPrefab = gameObject;
	}

	void Start()
	{
		UpdateSpriteTexture();
	}

// 	void Update()
// 	{
// 	}

	// Control Function -----------------------------------------------------------------
	public void SetSpriteTexture(Rect uvRect)
	{
		m_fTilingX	= uvRect.width;
		m_fTilingY	= uvRect.height;
		m_fOffsetX	= uvRect.x;
		m_fOffsetY	= uvRect.y;
	}

	public bool UpdateSpriteMaterial()
	{
		if (m_NcSpriteFactoryPrefab == null)
			return false;
		if (m_NcSpriteFactoryPrefab.renderer == null || m_NcSpriteFactoryPrefab.renderer.sharedMaterial == null || m_NcSpriteFactoryPrefab.renderer.sharedMaterial.mainTexture == null)
			return false;
		if (renderer == null)
			return false;
		NcSpriteFactory	ncSpriteFactory = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		if (ncSpriteFactory == null)
			return false;
		if (m_nSpriteFactoryIndex < 0 || ncSpriteFactory.GetSpriteNodeCount() <= m_nSpriteFactoryIndex)
			return false;
		if (ncSpriteFactory.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture)
			return false;
		renderer.sharedMaterial = m_NcSpriteFactoryPrefab.renderer.sharedMaterial;
		return true;
	}

	public void UpdateSpriteTexture()
	{
		if (UpdateSpriteMaterial() == false)
			return;
		NcSpriteFactory	ncSpriteFactory = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();

		ncSpriteFactory.UpdateNcSpriteTexture(m_nSpriteFactoryIndex, this);
		ncSpriteFactory.UpdateUvScale(m_nSpriteFactoryIndex, transform);
		m_EffectObject = ncSpriteFactory.CreateSpriteEffect(m_nSpriteFactoryIndex, transform);

  		if (UpdateMeshUVs(new Rect(m_fOffsetX, m_fOffsetY, m_fTilingX, m_fTilingY)) == false)
		{
// 			Debug.Log("m_Renderer.material");
			renderer.material.mainTextureScale	= new Vector2(m_fTilingX, m_fTilingY);
			renderer.material.mainTextureOffset	= new Vector2(m_fOffsetX, m_fOffsetY);
		}
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public override void OnUpdateToolData()
	{
	}
}

