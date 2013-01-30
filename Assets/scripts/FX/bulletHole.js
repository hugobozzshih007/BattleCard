#pragma strict
// Attach this script to a camera and it will paint black pixels in 3D 
// on whatever the user clicks. Make sure the mesh you want to paint 
// on has a mesh collider attached.
function Update () {
    // Only when we press the mouse
    if (!Input.GetMouseButton (0))
        return;

    // Only if we hit something, do we continue
    var hit : RaycastHit;
    if (!Physics.Raycast (camera.ScreenPointToRay(Input.mousePosition), hit))
        return;

    // Just in case, also make sure the collider also has a renderer
    // material and texture. Also we should ignore primitive colliders.
    var renderer : Renderer = hit.collider.renderer;
       var meshCollider = hit.collider as MeshCollider;
    if (renderer == null || renderer.sharedMaterial == null ||
        renderer.sharedMaterial.mainTexture == null || meshCollider == null)
        return;
 
    // Now draw a pixel where we hit the object
    var tex : Texture2D = renderer.material.mainTexture;
    var pixelUV = hit.textureCoord;
    pixelUV.x *= tex.width;
    pixelUV.y *= tex.height;
    
    tex.SetPixel(pixelUV.x, pixelUV.y, Color.black);
 
    tex.Apply();
}