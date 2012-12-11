@script ExecuteInEditMode

var mainColor: Color;

class EdgeDetectEffectNormals extends ImageEffectBase
{	
	function Start() {
		camera.depthTextureMode = DepthTextureMode.DepthNormals;
	}
		
	function OnRenderImage (source : RenderTexture, destination : RenderTexture)
	{
		material.color = mainColor;
		ImageEffects.BlitWithMaterial (material, source, destination);
	}
}
