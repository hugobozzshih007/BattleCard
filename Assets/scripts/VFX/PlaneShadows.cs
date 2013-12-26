using UnityEngine;
using System.Collections.Generic;
//[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]  
[AddComponentMenu("Ground/Plane Projector")]

public class PlaneShadows : MonoBehaviour {
    public Transform plane;
    Renderer objToProjectRenderer;
    private GameObject _objShadow;
    public Material SideOne, SideTwo, SideThree;
	public Material StartMat; 
    private Material _shadowMaterial;
    public Light theLight;
    public Camera cam;
    private Matrix4x4 mat = new Matrix4x4();
    private Vector3 L, n, E;
    private float c, d;
 	private MeshRenderer shadowMR;
    public void SetUp (){
        // Create a copy of the obj to project
		objToProjectRenderer = transform.renderer;
        _objShadow = new GameObject(objToProjectRenderer.gameObject.name + " shadow");
        _objShadow.hideFlags = HideFlags.HideInHierarchy;
 
        /*if(objToProjectRenderer.GetType() == typeof(MeshRenderer)){*/
            _objShadow.AddComponent<MeshFilter>();
			_objShadow.gameObject.layer = 17;
            _objShadow.GetComponent<MeshFilter>().sharedMesh = objToProjectRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
 
            shadowMR = _objShadow.AddComponent<MeshRenderer>();
            shadowMR.sharedMaterial = StartMat;
            shadowMR.castShadows = false;
            shadowMR.receiveShadows = false;
 
            _shadowMaterial = shadowMR.sharedMaterial;
        
        /*else{
            if(objToProjectRenderer.GetType() == typeof(SkinnedMeshRenderer)){
                SkinnedMeshRenderer objectSMR = (SkinnedMeshRenderer)objToProjectRenderer;
                SkinnedMeshRenderer shadowSMR = _objShadow.AddComponent<SkinnedMeshRenderer>();
 
                shadowSMR.sharedMesh = objectSMR.sharedMesh;
                shadowSMR.bones = objectSMR.bones;
                shadowSMR.sharedMaterial = SideThree;
                shadowSMR.castShadows = false;
                shadowSMR.receiveShadows = false;
 
                _shadowMaterial = shadowSMR.sharedMaterial;
            }
        }*/
		projectShape();
    }
 
    void Start(){
        SetUp ();
    }
 	
	public void ChangeShadowMaterial(int side){
		Material mat = SideThree;  
		switch(side){
			case 1:
				mat = SideOne;
				break;
			case 2:
				mat = SideTwo;
				break;
			case 3:
				mat = SideThree;
				break;
			default:
				break;
		}
		shadowMR.sharedMaterial = mat;
		_shadowMaterial = shadowMR.sharedMaterial;
		projectShape();
	}
	
	void projectShape(){
		if(_objShadow){
            _objShadow.transform.position = objToProjectRenderer.transform.position;
            _objShadow.transform.rotation = objToProjectRenderer.transform.rotation;
            _objShadow.transform.localScale = objToProjectRenderer.transform.localScale;        
 
            switch(theLight.type){
                case LightType.Point:
                    // Calculate the projection matrix
                    // Let L be the position of the light
                    // P the position of a vertex of the object we want to shadow
                    // E a point of the plane (not seen in the figure)
                    // n the normal vector of the plane
 
                    L = theLight.transform.position;
                    n = -plane.up;
                    E = plane.position;
 
                    d = Vector3.Dot(L, n);
                    c = Vector3.Dot(E, n) - d;
 
                    mat[0, 0] = L.x*n.x + c;
                    mat[1, 0] = L.y*n.x;
                    mat[2, 0] = L.z*n.x;
                    mat[3, 0] = n.x;
 
                    mat[0, 1] = L.x*n.y;
                    mat[1, 1] = L.y*n.y + c;
                    mat[2, 1] = L.z*n.y;
                    mat[3, 1] = n.y;
 
                    mat[0, 2] = L.x*n.z;
                    mat[1, 2] = L.y*n.z;
                    mat[2, 2] = L.z*n.z + c;
                    mat[3, 2] = n.z;
 
                    mat[0, 3] = -L.x * (c+d);
                    mat[1, 3] = -L.y * (c+d);
                    mat[2, 3] = -L.z * (c+d);
                    mat[3, 3] = -d;
                break;
 
                case LightType.Spot:
                    goto case LightType.Directional;
 
                case LightType.Directional:
                    // Calculate the projection matrix
                    // Let L be the direction of the light
                    // P the position of a vertex of the object we want to shadow
                    // E a point of the plane (not seen in the figure)
                    // n the normal vector of the plane
 
                    L = theLight.transform.forward;
                    n = -plane.up;
                    E = plane.position;
 
                    d = Vector3.Dot(L, n);
                    c = Vector3.Dot(E, n);
 
                    mat[0, 0] = d-n.x*L.x;
                    mat[1, 0] = -n.x*L.y;
                    mat[2, 0] = -n.x*L.z;
                    mat[3, 0] = 0;
 
                    mat[0, 1] = -n.y*L.x;
                    mat[1, 1] = d-n.y*L.y;
                    mat[2, 1] = -n.y*L.z;
                    mat[3, 1] = 0;
 
                    mat[0, 2] = -n.z*L.x;
                    mat[1, 2] = -n.z*L.y;
                    mat[2, 2] = d-n.z*L.z;
                    mat[3, 2] = 0;
 
                    mat[0, 3] = c*L.x;
                    mat[1, 3] = c*L.y;
                    mat[2, 3] = c*L.z;
                    mat[3, 3] = d;
                break;
            }
 
            Shader.SetGlobalMatrix("_projectionMatrix", mat);
            Shader.SetGlobalMatrix("_viewInv", cam.cameraToWorldMatrix);
            Shader.SetGlobalMatrix("_view", cam.worldToCameraMatrix);
            _shadowMaterial.SetVector("_planeNormal", plane.up);
        }
	}
	
    void Update(){  
    }
}
