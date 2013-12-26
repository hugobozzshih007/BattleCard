using UnityEngine;

[ExecuteInEditMode]

 

[AddComponentMenu("Image Effects/CDBarEffect")]

public class SkillCDBarEffect : ImageEffectBase {
	
    public float radius = 0.5f;
   	
	public Color colorGood;

    public Color colorBad;
	
	public Color colorSide; 
	
	public Texture TopTexture; 

    public float    m_angle = 180.0f;

    void Start(){

    }

    void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetColor("m_colorGood",colorGood);

        material.SetColor("m_colorBad",colorBad);
		
		material.SetColor("m_colorSide",colorSide);

        material.SetFloat("m_angle",m_angle);

        material.SetFloat("m_radius", radius);
		
		material.SetTexture("TopTexture" , TopTexture);

        Graphics.Blit (source, destination, material);

    }

}