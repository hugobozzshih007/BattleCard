using UnityEngine;


public class EdgeShowUp : ImageEffectBase {
	public Color SideAColor, SideBColor;
	GeneralSelection currentSel;  
	// Use this for initialization
	void Start () {
		camera.depthTextureMode = DepthTextureMode.DepthNormals;
		currentSel = Camera.main.GetComponent<GeneralSelection>();
	}
	
	Color ChooseColor(){
		Color mainColor = SideAColor;
		int side = 1;
		if(currentSel.ChessInSelection!=null)
			side = currentSel.ChessInSelection.GetComponent<CharacterProperty>().Player;
		if(side==1){
			mainColor = SideAColor; 
		}else{
			mainColor = SideBColor;
		}
		if(currentSel.NpcPlaying)
			mainColor = SideBColor;
		return mainColor;
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Color rightColor = ChooseColor();
		material.SetColor("_Color",rightColor);
		Graphics.Blit (source, destination, material);
	}
}
