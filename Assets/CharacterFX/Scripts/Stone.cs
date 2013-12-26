using UnityEngine;
using System.Collections;

public class Stone : MonoBehaviour
{
	public AudioSource StoneSource;
	public AudioClip StoneSound; //crackle
	public Renderer SourceRenderer;
	#if UNITY_4_0
		public Animator SourceAnimator;
	    private float AnimSpeed = 1.0f;
	#endif
	public Animation SourceAnimation;
	
	private bool  IsStone = false;
	private Material[] EffectMaterials;	
	public float EffectLength = 0.0f;	
	
	void Start () 
	{
		EffectMaterials = SourceRenderer.materials;
	}
		
	public void OnDestroy()
	{
		foreach(Material m in EffectMaterials)
		{
			Destroy (m);
		}
	}
	
	private void SetMaterialParms(float amount)
	{
		foreach(Material m in EffectMaterials)
		{
			if (m.shader.name.Contains("Character/Statue"))
			{
				m.SetFloat("_DiffuseAmount",amount);
			}
		}	
	}	
	
	private float StartEffect()
	{
		if (StoneSource != null && StoneSound != null)
		{
			EffectLength = StoneSound.length;
			StoneSource.PlayOneShot(StoneSound);
		}
		return EffectLength;
	}
	
	private IEnumerator doTurnToStone()
	{
		if (!IsStone) 
		{
			IsStone = true;
			#if UNITY_4_0
			if (SourceAnimator != null)
			{
				AnimSpeed = SourceAnimator.speed;
				SourceAnimator.speed = 0;
			}
			#endif
			if (SourceAnimation != null)
			{
				foreach(AnimationState ast in SourceAnimation.animation)
				{
					ast.speed = 0.0f;
				}
			}
		
			float LengthLeft = StartEffect();
				
			while(LengthLeft > 0.0f)
	    	{
				float pos = LengthLeft / EffectLength;
				SetMaterialParms(pos);
    	    	yield return null;
				LengthLeft -= Time.deltaTime;
    		}
			SetMaterialParms(0.0f);
		}
	}

	private IEnumerator doStoneToFlesh()
	{
		if (IsStone) 
		{
			float LengthLeft = StartEffect();
				
			while(LengthLeft > 0.0f)
    		{
				float pos = 1.0f - (LengthLeft / EffectLength);
				SetMaterialParms (pos);
        		yield return null;
				LengthLeft -= Time.deltaTime;
    		}
			SetMaterialParms(1.0f);
			#if UNITY_4_0
			if (SourceAnimator != null)
			{
				SourceAnimator.speed = AnimSpeed;
			}
			#endif
			if (SourceAnimation != null)
			{
				foreach(AnimationState ast in SourceAnimation.animation)
				{
					ast.speed = 1.0f;
				}
			}
			
			IsStone = false;
		}
	}

	public void TurnToStone()
	{
		StartCoroutine(doTurnToStone());
	}

	public void StoneToFlesh()
	{
		StartCoroutine(doStoneToFlesh());
	}
	
	public void TurnToStone(float length)
	{
		EffectLength = length;
		TurnToStone();
	}

	public void StoneToFlesh(float length)
	{
		EffectLength = length;
		StoneToFlesh();
	}
}

