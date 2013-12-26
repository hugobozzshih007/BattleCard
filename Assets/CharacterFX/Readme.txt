Update 1.2

Updates:

Hologram shader - Mix in diffuse texture, Disabling scan lines
Ghost shader - non bump mapped version added
Statue shaders - rewrite - less complicated, plus parameter to tighten blending at edges

Added Controller classes for helper scripts - coordinate changing of multiple game objects
  StatueController 
  StoneController
  BeamController 

Updated help scripts to work with multi-material meshes.

Statue Shader
  This shader lerps between normal Diffuse/Diffuse Bumped, and a projected stone effect, with optional "grunge" droppings/dirt accumulation on the top. A helper
  is included (along with a sound clip) to turn someone to stone, and back to flesh. The bump map is used in both the diffuse and statue modes.
Shader Parameters:
  Color: Standard color 
  Maintex: The diffuse Texture
  BumpMap: The normal map (only in the bump mapped version)
  BaseTex: The base "clean" statue texture.
  GrungeTex: The dirt accumulation
  Scale: The scaling in world space of the textures
  DiffuseAmount: The amount of the affect. 0 = completely statue. 1 = normal. This value can range from 0-1.
  GrungeAmount: The amount that the dirt spreads. if you want a completely clean statue, you should assign the grunge texture the same as the base.
  *New - Tighten:  Controls the transition between statue and grunge.

Hologram Shader:
  This shader is a diffuse/bump-mapped shader that displays the models as if it were a Hologram.
Shader Parameters:
  Rim Color is the color of the effect.
  Rim Power is the amount of Rim Shading - the higher the value, the more the edges are enhanced.
  Clip Power is the amount of screen-space clipping. This can be varied to produce a flicker effect. *New: Set to max to disable
  Brightness is the overall brightness of the effect. 
  *New - Diffuse Amount: The amount of Diffuse to mix into the hologram.

Ghost Shader:
  This shader is a diffuse/bump-mapped shader that mimics the spectral enemies of some popular 3d games. 
Shader Parameters:
  Rim Color is the color of the effect.
  Rim Power is the amount of Rim Shading - the higher the value, the more the edges are enhanced.
  Brightness is the overall brightness of the effect. 

Disintegrate Shader:
  There are two disintegration shaders - one bump mapped, and one not. This shader allows you to disintegrate
  a model based on the "effect amount" property.
Shader Parameters:
  Effect Amount: The amount of disintegration.
  Effect Map: A control bitmap that allows you to specify the pattern of the disintegration. Several samples are provided.
  Edge color: the color at the edge of the disintegration effect. This color is affected by shadows.
  Edge Emission: The emissive color - this color is not affected by shadows. Mix up edge & emission for a nice effect.
  Edge Range: The size of the edge - varies based on the input bitmap
  Tile Factor: The amount of tiling of the effect map.

Character Shader:
  There are three shader variations included. Two with a more game-like lighting effect (lambert shading with wrapping), bumped and diffuse, and one standard 
  BlinnPhong bumped version. This shader allows you to adjust colors on model in real time, allowing for customization of hair, skin and eye color. The colors
  are multiplied in to an accurate grayscale of the texture color.
  
Shader Parameters:
  MaskTex: This texture defines the "mask" that determines where the hair, skin, and eyes are in the diffuse texture. Red defines the eyes, Green defines the skin,
  and Blue defines the hair. See the sample scene included for an example of a mask texture for Kyle the Robot. (Note: we realize that Kyle doesn't actually
  have hair, eyes and skin - we made our best guess)
  EyeColor: The new color for the eyes
  HairColor: The new color for the hair
  SkinColor: The new color for the skin  

Sample scripts for adjusting the colors, disintegration and hologram effects are included (customcolors, beam.cs and hologram.cs). 
See the sample scene included for usage.