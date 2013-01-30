// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcSpriteAnimation : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		enum PLAYMODE	{DEFAULT, INVERSE, PINGPONG, RANDOM, SELECT};
	public		PLAYMODE		m_PlayMode			= 0;
	public		float			m_fDelayTime;
	public		int				m_nStartFrame		= 0;
	public		int				m_nFrameCount		= 0;
	public		int				m_nSelectFrame		= 0;
	public		bool			m_bLoop				= true;
	public		bool			m_bAutoDestruct		= false;
	public		float			m_fFps				= 10;
	public		int				m_nTilingX			= 2;
	public		int				m_nTilingY			= 2;

	[HideInInspector]
	public		NcSpriteFactory	m_NcSpriteFactory	= null;
	protected	string			m_funcStartSprite	= "OnAnimationStartFrame";
	protected	string			m_funcEndSprite		= "OnEndSprite";

	[HideInInspector]
	public		bool			m_bBuildSpriteObj	= false;

	protected	Vector2			m_size;
	protected	Renderer		m_Renderer;
	protected	float			m_fStartTime;
	protected	int				m_nLastIndex		= -999;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";
		if (1 < GetEditingUvComponentCount())
			return "SCRIPT_DUPERR_EDITINGUV";
		if (renderer == null || renderer.sharedMaterial == null)
			return "SCRIPT_EMPTY_MATERIAL";

		return "";	// no error
	}
#endif

	public float GetDurationTime()
	{
		return (m_PlayMode == PLAYMODE.PINGPONG ? m_nFrameCount*2-1 : m_nFrameCount) / m_fFps;
	}

	public int GetShowIndex()
	{
		return m_nLastIndex + m_nStartFrame;
	}

	public void Restart()
	{
		m_nLastIndex	= -1;
		enabled			= true;
		Start();
	}

	public void SetSelectFrame(int nSelFrame)
	{
		m_nSelectFrame = nSelFrame;
		SetIndex(m_nSelectFrame);
	}

	// Loop Function --------------------------------------------------------------------
	void Start()
	{
		m_size			= new Vector2(1.0f / m_nTilingX, 1.0f / m_nTilingY);
		m_Renderer		= renderer;

		m_fStartTime	= GetEngineTime();
		m_nFrameCount	= (m_nFrameCount <= 0) ? m_nTilingX * m_nTilingY : m_nFrameCount;
		if (m_Renderer == null)
		{
			enabled = false;
			return;
		}

		if (m_PlayMode == PLAYMODE.SELECT)
		{
			SetIndex(m_nSelectFrame);
		} else {
			if (0 < m_fDelayTime)
			{
				m_Renderer.enabled = false;
				return;
			}
			if (m_PlayMode == PLAYMODE.RANDOM)
				SetIndex(Random.Range(0, m_nFrameCount-1));
			else SetIndex(0);
		}
	}

	void Update()
	{
		if (m_PlayMode == PLAYMODE.SELECT)
			return;
		if (m_Renderer == null || m_nTilingX * m_nTilingY == 0)
			return;

		if (m_fDelayTime != 0)
		{
			if (GetEngineTime() < m_fStartTime + m_fDelayTime)
				return;
			m_fDelayTime = 0;
			m_fStartTime = GetEngineTime();
			m_Renderer.enabled = true;
		}

		if (m_PlayMode != PLAYMODE.RANDOM)
		{
			int nIndex = (int)((GetEngineTime() - m_fStartTime) * m_fFps);

			if (nIndex == 0)
			{
				if (m_NcSpriteFactory != null)
					m_NcSpriteFactory.OnAnimationStartFrame(this);
//				if (m_funcStartSprite != "")
//					gameObject.SendMessage(m_funcStartSprite, this, SendMessageOptions.DontRequireReceiver);
			}

			if (m_NcSpriteFactory != null && m_nFrameCount <= 0)
				m_NcSpriteFactory.OnAnimationLastFrame(this, 0);
			else {
				if ((m_PlayMode == PLAYMODE.PINGPONG ? m_nFrameCount*2-1 : m_nFrameCount) <= nIndex)	// first loop
				{
					if (m_bLoop == false)
					{
						if (m_NcSpriteFactory != null)
							if (m_NcSpriteFactory.OnAnimationLastFrame(this, 1))
								return;
// 						if (m_funcEndSprite != "")
//							gameObject.SendMessage(m_funcEndSprite, this, SendMessageOptions.DontRequireReceiver);

						if (m_bAutoDestruct)
						{
							Destroy(gameObject);
							return;
						}
						enabled = false;
						return;
					} else {
						if (m_PlayMode == PLAYMODE.PINGPONG)
						{
							if (m_NcSpriteFactory != null && nIndex % (m_nFrameCount*2-2) == 1)
								if (m_NcSpriteFactory.OnAnimationLastFrame(this, nIndex / (m_nFrameCount*2-1)))
									return;
						} else {
							if (m_NcSpriteFactory != null && nIndex % m_nFrameCount == 0)
								if (m_NcSpriteFactory.OnAnimationLastFrame(this, nIndex / m_nFrameCount))
									return;
						}
					}
				}
				SetIndex(nIndex);
			}
		}
	}

	// Control Function -----------------------------------------------------------------
	void SetIndex(int nIndex)
	{
		if (m_Renderer != null)
		{
			int	nSetIndex  = nIndex;
			int nLoopCount = nIndex / m_nFrameCount;

			switch (m_PlayMode)
			{
				case PLAYMODE.DEFAULT:	nSetIndex = nIndex % m_nFrameCount;	break;
				case PLAYMODE.INVERSE:	nSetIndex = m_nFrameCount - (nSetIndex % m_nFrameCount) - 1;	break;
				case PLAYMODE.PINGPONG:
					{
						nLoopCount = (nSetIndex / (m_nFrameCount * 2 - (nSetIndex == 0 ? 1 : 2)));
						nSetIndex  = (nSetIndex % (m_nFrameCount * 2 - (nSetIndex == 0 ? 1 : 2)));
						if (m_nFrameCount <= nSetIndex)
							nSetIndex = m_nFrameCount - (nSetIndex % m_nFrameCount) - 2;
						break;
					}
				case PLAYMODE.RANDOM:	break;
				case PLAYMODE.SELECT:	nSetIndex = nIndex % m_nFrameCount;		break;
			}

			if (nSetIndex == m_nLastIndex)
				return;

			int		uIndex = (nSetIndex + m_nStartFrame) % m_nTilingX;
			int		vIndex = (nSetIndex + m_nStartFrame) / m_nTilingX;
			Vector2 offset = new Vector2(uIndex * m_size.x, 1.0f - m_size.y - vIndex * m_size.y);

  			if (UpdateMeshUVs(new Rect(offset.x, offset.y, m_size.x, m_size.y)) == false)
			{
// 				Debug.Log("m_Renderer.material");
				m_Renderer.material.mainTextureOffset	= offset;
				m_Renderer.material.mainTextureScale	= m_size;
			}

			if (m_NcSpriteFactory != null)
				m_NcSpriteFactory.OnAnimationChangingFrame(this, m_nLastIndex, nSetIndex, nLoopCount);

			m_nLastIndex = nSetIndex;
		}
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime *= fSpeedRate;
		m_fFps	*= fSpeedRate;
	}
}

