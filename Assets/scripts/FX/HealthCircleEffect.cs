using UnityEngine;

[ExecuteInEditMode]

 

[AddComponentMenu("Image Effects/HealthCircle")]

public class HealthCircleEffect : ImageEffectBase {

    public float radius = 0.5f;

    public Color colorGood;

    public Color colorBad;

    public float health = 0.5f;

    

    public Texture rampTexture;
	public Texture TopTexture; 

    public float    m_angle = 0.2f;

    

    void Start()

    {

        setHealth( health );

    }

    //we except health to be from 0 to 1.

    public void setHealth(float health)

    {

        m_angle = health * 360f;

    }

    void OnRenderImage (RenderTexture source, RenderTexture destination) {

        material.SetColor("m_colorGood",colorGood);

        material.SetColor("m_colorBad",colorBad);

        material.SetFloat("m_angle",m_angle);

        material.SetFloat("m_radius", radius);

        material.SetTexture("rampTexture",rampTexture);
		
		material.SetTexture("TopTexture" , TopTexture);

        Graphics.Blit (source, destination, material);

    }

}