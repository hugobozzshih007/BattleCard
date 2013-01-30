// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NcSpriteFactory : NcEffectBehaviour
{
	// class ------------------------------------------------------------------------
	[System.Serializable]
	public class NcSpriteNode
	{
		public		bool			m_bIncludedAtlas	= true;
		public		string			m_TextureGUID		= "";
 		public		string			m_TextureName		= "";
		public		float			m_fMaxTextureAlpha	= 1.0f;

		// NcSpriteAnimation
		public		bool			m_bLoop				= false;
		public		float			m_fFps				= 20;
		public		float			m_fTime				= 0;

		// 자동설정
		public		Rect			m_UvRect;
		public		int				m_nStartFrame		= 0;
		public		int				m_nFrameCount		= 0;
		public		int				m_nFrameSize		= 0;

		// char animation
		public		int				m_nNextSpriteIndex	= -1;
		public		int				m_nTestMode			= 0;
		public		float			m_fTestSpeed		= 1;
		// char effect
		public		GameObject		m_EffectPrefab		= null;
		public		int				m_nEffectFrame		= 0;
		public		bool			m_bEffectOnlyFirst	= true;
		public		float			m_fEffectSpeed		= 1;
		public		float			m_fEffectScale		= 1;
		public		Vector3			m_EffectPos			= Vector3.zero;
		public		Vector3			m_EffectRot			= Vector3.zero;
		// char sound
		public		AudioClip		m_AudioClip			= null;
		public		int				m_nSoundFrame		= 0;
		public		bool			m_bSoundOnlyFirst	= true;
		public		bool			m_bSoundLoop		= false;
		public		float			m_fSoundVolume		= 1;
		public		float			m_fSoundPitch		= 1;

		// function
		public NcSpriteNode GetClone()
		{
			return null;
		}
	}
	[SerializeField]

	// Attribute ------------------------------------------------------------------------
	public		enum SPRITE_TYPE		{NcSpriteTexture, NcSpriteAnimation};
	public		SPRITE_TYPE				m_SpriteType;
	public		List<NcSpriteNode>		m_SpriteList;
	public		int						m_nCurrentIndex;
// 	public		Material				m_AtlasMaterial;
	public		int						m_nMaxAtlasTextureSize	= 2048;

	// NcUvAnimation
	public		float					m_fUvScale;
	public		float					m_fTextureRatio;

	// 자동설정
	public		int						m_nFrameSize;
	public		int						m_nTilingX;
	public		int						m_nTilingY;

	public		GameObject				m_CurrentEffect;
	public		NcAttachSound			m_CurrentSound;

	// Internal
	protected	bool					m_bEndSprite		= true;

	// ShowOption
	public		enum SHOW_TYPE			{NONE, ALL, SPRITE, ANIMATION, EFFECT};
	public		SHOW_TYPE				m_ShowType			= SHOW_TYPE.SPRITE;
	public		bool					m_bShowEffect		= true;
	public		bool					m_bTestMode			= true;
	public		bool					m_bSequenceMode		= false;

//	[HideInInspector]

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		return "";	// no error
	}
#endif

// 	public float GetDurationTime()
// 	{
// 		return (m_PlayMode == PLAYMODE.PINGPONG ? m_nFrameCount*2-2 : m_nFrameCount) / m_fFps;
// 	}
// 
// 	public int GetShowIndex()
// 	{
// 		return m_nLastIndex + m_nStartFrame;
// 	}

	public NcSpriteNode GetSpriteNode(int nIndex)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
			return null;
		return m_SpriteList[nIndex] as NcSpriteNode;
	}

	public NcSpriteNode SetSpriteNode(int nIndex, NcSpriteNode newInfo)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
			return null;
		NcSpriteNode	oldSpriteNode = m_SpriteList[nIndex] as NcSpriteNode;
		m_SpriteList[nIndex] = newInfo;
		return oldSpriteNode;
	}

	public int AddSpriteNode()
	{
		NcSpriteNode	SpriteNode	= new NcSpriteNode();

		if (m_SpriteList == null)
			m_SpriteList = new List<NcSpriteNode>();
		m_SpriteList.Add(SpriteNode);
		return m_SpriteList.Count-1;
	}

	public int AddSpriteNode(NcSpriteNode addSpriteNode)
	{
		if (m_SpriteList == null)
			m_SpriteList = new List<NcSpriteNode>();
		m_SpriteList.Add(addSpriteNode.GetClone());
		return m_SpriteList.Count-1;
	}

	public void DeleteSpriteNode(int nIndex)
	{
		if (m_SpriteList == null || nIndex < 0 || m_SpriteList.Count <= nIndex)
			return;
		m_SpriteList.Remove(m_SpriteList[nIndex]);
	}

	public void ClearAllSpriteNode()
	{
		if (m_SpriteList == null)
			return;
		m_SpriteList.Clear();
	}

	public int GetSpriteNodeCount()
	{
		if (m_SpriteList == null)
			return 0;
		return m_SpriteList.Count;
	}

	public NcSpriteNode GetCurrentSpriteNode()
	{
		if (m_SpriteList == null || m_SpriteList.Count < 1)
			return null;
		return m_SpriteList[m_nCurrentIndex];
	}

	// Loop Function --------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	public NcEffectBehaviour SetSprite(int nNodeIndex)
	{
		return SetSprite(nNodeIndex, true);
	}

	public NcEffectBehaviour SetSprite(int nNodeIndex, bool bRunImmediate)
	{
		if (m_SpriteList == null || nNodeIndex < 0 || m_SpriteList.Count <= nNodeIndex)
			return null;

		if (bRunImmediate)
			OnChangingSprite(m_nCurrentIndex, nNodeIndex);
		m_nCurrentIndex = nNodeIndex;

		if (m_SpriteType == SPRITE_TYPE.NcSpriteAnimation)
		{
			NcSpriteAnimation spriteCom = GetComponent<NcSpriteAnimation>();
			spriteCom.m_NcSpriteFactory	= this;
			spriteCom.m_bBuildSpriteObj	= false;
			spriteCom.m_bAutoDestruct	= false;
			spriteCom.m_nStartFrame		= m_SpriteList[nNodeIndex].m_nStartFrame;
			spriteCom.m_nFrameCount		= m_SpriteList[nNodeIndex].m_nFrameCount;
			spriteCom.m_fFps			= m_SpriteList[nNodeIndex].m_fFps;
			spriteCom.m_nTilingX		= m_nTilingX;
			spriteCom.m_nTilingY		= m_nTilingY;
//			spriteCom.m_PlayMode		= NcSpriteAnimation.PLAYMODE.DEFAULT;
			spriteCom.m_bLoop			= m_SpriteList[nNodeIndex].m_bLoop;

			if (bRunImmediate)
				spriteCom.Restart();
			return spriteCom;
		}
		if (m_SpriteType == SPRITE_TYPE.NcSpriteTexture)
		{
			NcSpriteTexture uvCom	= GetComponent<NcSpriteTexture>();
			if (uvCom != null)
			{
				uvCom.m_nSpriteFactoryIndex = nNodeIndex;
				UpdateNcSpriteTexture(nNodeIndex, uvCom);
				if (bRunImmediate)
				{
					UpdateUvScale(nNodeIndex, transform);
					CreateEffectObject();
				}
			}
			return uvCom;
		}

		return null;
	}

	public int GetCurrentSpriteIndex()
	{
		return m_nCurrentIndex;
	}

	public bool IsEndSprite()
	{
		return m_bEndSprite;
	}

	void CreateEffectObject()
	{
		if (m_bShowEffect == false)
			return;
		DestroyEffectObject();
// 		Debug.Log("CreateEffectObject() - new = " + ncSpriteNode.m_EffectPrefab);

		// Notify EffectFrame
		if (transform.parent != null)
			transform.parent.SendMessage("OnSpriteListEffectFrame", m_SpriteList[m_nCurrentIndex], SendMessageOptions.DontRequireReceiver);

		m_CurrentEffect = CreateSpriteEffect(m_nCurrentIndex, transform);

		// Notify CreateEffectInstance
		if (transform.parent != null)
			transform.parent.SendMessage("OnSpriteListEffectInstance", m_CurrentEffect, SendMessageOptions.DontRequireReceiver);
	}

	public void UpdateNcSpriteTexture(int nSrcSpriteIndex, NcSpriteTexture toNcSpriteTex)
	{
// 		Debug.Log(toNcSpriteTex);
// 		Debug.Log(nSrcSpriteIndex);
		toNcSpriteTex.SetSpriteTexture(m_SpriteList[nSrcSpriteIndex].m_UvRect);
	}

	public void UpdateUvScale(int nSrcSpriteIndex, Transform targetTrans)
	{
		Vector3	scale;
		if (1 <= m_fTextureRatio)
			 scale = new Vector3(m_SpriteList[nSrcSpriteIndex].m_UvRect.width * m_fUvScale * m_fTextureRatio, m_SpriteList[nSrcSpriteIndex].m_UvRect.height * m_fUvScale, 1);
		else scale = new Vector3(m_SpriteList[nSrcSpriteIndex].m_UvRect.width * m_fUvScale, m_SpriteList[nSrcSpriteIndex].m_UvRect.height * m_fUvScale / m_fTextureRatio, 1);
		targetTrans.localScale	= Vector3.Scale(targetTrans.localScale, scale);
	}

	public GameObject CreateSpriteEffect(int nSrcSpriteIndex, Transform parentTrans)
	{
		GameObject createEffect = null;

		if (m_SpriteList[nSrcSpriteIndex].m_EffectPrefab != null)
		{
			// Create BaseGameObject
			createEffect	= CreateGameObject("Effect_" + m_SpriteList[nSrcSpriteIndex].m_EffectPrefab.name);
			if (createEffect == null)
				return null;

			// Change Parent
			ChangeParent(parentTrans, createEffect.transform, true, null);

			NcAttachPrefab	attachCom		= createEffect.AddComponent<NcAttachPrefab>();
// 			attachCom.m_fDelayTime			= 0;
			attachCom.m_AttachPrefab		= m_SpriteList[nSrcSpriteIndex].m_EffectPrefab;
			attachCom.m_fPrefabSpeed		= m_SpriteList[nSrcSpriteIndex].m_fEffectSpeed;
			attachCom.m_bDetachParent		= true;
			attachCom.CreateAttachPrefabImmediately();

// 			createEffect.transform.localScale *= ncSpriteNode.m_fEffectScale;
// 			createEffect.transform.Translate(ncSpriteNode.m_EffectPos, Space.Self);
// 			createEffect.transform.Rotate(ncSpriteNode.m_EffectRot, Space.Self);
			createEffect.transform.localScale	*= m_SpriteList[nSrcSpriteIndex].m_fEffectScale;
			createEffect.transform.localPosition += m_SpriteList[nSrcSpriteIndex].m_EffectPos;
			createEffect.transform.localRotation *= Quaternion.Euler(m_SpriteList[nSrcSpriteIndex].m_EffectRot);
		}
		return createEffect;
	}

	void DestroyEffectObject()
	{
//  		Debug.Log("DestroyEffectObject - " + m_CurrentEffect);
 		if (m_CurrentEffect != null)
 			Destroy(m_CurrentEffect);
		m_CurrentEffect = null;
	}

	void CreateSoundObject(NcSpriteNode ncSpriteNode)
	{
// 		Debug.Log("CreateSoundObject");
		if (m_bShowEffect == false)
			return;

		if (ncSpriteNode.m_AudioClip != null)
		{
			if (m_CurrentSound == null)
				m_CurrentSound = gameObject.AddComponent<NcAttachSound>();

			m_CurrentSound.m_AudioClip	= ncSpriteNode.m_AudioClip;
			m_CurrentSound.m_bLoop		= ncSpriteNode.m_bSoundLoop;
			m_CurrentSound.m_fVolume	= ncSpriteNode.m_fSoundVolume;
			m_CurrentSound.m_fPitch		= ncSpriteNode.m_fSoundPitch;
			m_CurrentSound.enabled		= true;
			m_CurrentSound.Replay();
		}
	}

	// Event Function -------------------------------------------------------------------
	// 변경 중일때...
	public void OnChangingSprite(int nOldNodeIndex, int nNewNodeIndex)
	{
// 		Debug.Log("OnChangingSprite() - nOldNodeIndex = " + nOldNodeIndex + ", nNewNodeIndex = " + nNewNodeIndex);

		m_bEndSprite = false;
		DestroyEffectObject();
	}

	// 첫 frame 시작할때...
	public void OnAnimationStartFrame(NcSpriteAnimation spriteCom)
	{
	}

	// frame 변경될 때 마다... (frame skip 되기도 함)
	public void OnAnimationChangingFrame(NcSpriteAnimation spriteCom, int nOldIndex, int nNewIndex, int nLoopCount)
	{
// 		Debug.Log("OnAnimationChangingFrame() - nOldIndex = " + nOldIndex + ", nNewIndex = " + nNewIndex + ", nLoopCount = " + nLoopCount + ", m_nCurrentIndex = " + m_nCurrentIndex);

		if (m_SpriteList[m_nCurrentIndex].m_EffectPrefab != null)
		{
			if ((nOldIndex < m_SpriteList[m_nCurrentIndex].m_nEffectFrame || nNewIndex <= nOldIndex) && m_SpriteList[m_nCurrentIndex].m_nEffectFrame <= nNewIndex)
				if (nLoopCount == 0 || m_SpriteList[m_nCurrentIndex].m_bEffectOnlyFirst == false)
					CreateEffectObject();
		}
		if (m_SpriteList[m_nCurrentIndex].m_AudioClip != null)
		{
			if ((nOldIndex < m_SpriteList[m_nCurrentIndex].m_nSoundFrame || nNewIndex <= nOldIndex)  && m_SpriteList[m_nCurrentIndex].m_nSoundFrame <= nNewIndex)
				if (nLoopCount == 0 || m_SpriteList[m_nCurrentIndex].m_bSoundOnlyFirst == false)
					CreateSoundObject(m_SpriteList[m_nCurrentIndex]);
		}
	}

	// 마지막 frame시간 지나면..(다음 loop 첫프레임이 되면), ret가 참이면 애니변경됨
	public bool OnAnimationLastFrame(NcSpriteAnimation spriteCom, int nLoopCount)
	{
		m_bEndSprite = true;
// 		DestroyEffectObject();

		if (m_bSequenceMode)
		{
			if (m_nCurrentIndex < GetSpriteNodeCount() - 1)
			{
				if ((m_SpriteList[m_nCurrentIndex].m_bLoop ? 3 : 1) == nLoopCount)
				{
					SetSprite(m_nCurrentIndex+1);
					return true;
				}
			} else SetSprite(0);
		} else {
			NcSpriteAnimation ncTarSprite = (NcSpriteAnimation)SetSprite(m_SpriteList[m_nCurrentIndex].m_nNextSpriteIndex);
			if (ncTarSprite != null)
			{
				ncTarSprite.Restart();
				return true;
			}
		}
		return false;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
// 		m_fFps	*= fSpeedRate;
	}
}

