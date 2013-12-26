using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
	NpcPlayer npc;
	//public Texture2D UpArrow, DownArrow, LeftArrow, RightArrow; 
	selection currentSel; 
	Transform currentInMove;
	RoundCounter RC; 
	Vector3 oldCamPosition = new Vector3();
	Vector3 newCamPosition = new Vector3();
	StatusMachine sMachine;
	bool moveCam = false; 
	float camSpeed = 0.3f; 
	float barWidth = 720.0f;
	int camStep = 0;
	Rect up, down, left, right; 
	public float timeSeg = 0.0f;
	Vector2 mousePos = new Vector2();
	int dragSpeed = 65;
	int scrollSpeed = 25; 
	int scrollArea = 10;
	public int levelAreaXMax = 31;
	public int levelAreaXMin = -31;
	public int levelAreaZMax = 14;
	public int levelAreaZMin = -60;
	int zoomMax = 20;
	int zoomMin = 65;
	int zoomSpeed = 25;
	float distance = 0.2f;
	int panSpeed = 50;
	int panAngleMin = 25;
    int panAngleMax = 80;
	// Use this for initialization
	void Start () {
		barWidth = Screen.height / 72.0f;
		npc = GameObject.Find("NpcPlayer").GetComponent<NpcPlayer>();
		currentSel = transform.GetComponent<selection>();
		sMachine = GameObject.Find("StatusMachine").GetComponent<StatusMachine>();
		RC = transform.GetComponent<RoundCounter>();
		up = new Rect(0.0f, 0.0f, Screen.width, barWidth); 
		down = new Rect(0.0f, Screen.height-barWidth, Screen.width, barWidth);
		left = new Rect(0.0f, 40.0f, barWidth, Screen.height-40.0f);
		right = new Rect(Screen.width-barWidth, 40.0f, barWidth, Screen.height-40.0f);
	}
	
	public void CamFollowMe(Transform currentInMove){
		newCamPosition = currentInMove.position - RC.CamOffest;
		oldCamPosition = transform.position;
		//transform.position = newCamPosition;
		moveCam = true;
	}
	
	void TranslateMainCam(float timeToReach){
		timeSeg+= Time.deltaTime/timeToReach;
		Vector3 newPos = Vector3.Lerp(oldCamPosition, newCamPosition, timeSeg);
		transform.position = newPos;
		float d = Vector3.Distance(transform.position, newCamPosition);
		if(d<0.001f){
			moveCam = false;
		}
	}
	
	void Panning(){
		Vector3 translation = Vector3.zero;
		
		// Zoom in or out
        var zoomDelta = Input.GetAxis("Mouse ScrollWheel")*zoomSpeed*Time.deltaTime;
        if (zoomDelta!=0)
        {
            translation -= Vector3.up * zoomSpeed * zoomDelta;
        }

        // Start panning camera if zooming in close to the ground or if just zooming out.
        /*var pan = camera.transform.eulerAngles.x - zoomDelta * panSpeed;
        pan = Mathf.Clamp(pan, panAngleMin, panAngleMax);
        if (zoomDelta < 0 || camera.transform.position.y < (zoomMin / 2))
        {
            camera.transform.eulerAngles = new Vector3(pan, 0, 0);
        }*/
		
		if(Input.GetMouseButton(1)) {
			translation -= new Vector3(Input.GetAxis("Mouse X") * dragSpeed * Time.deltaTime, 0, 
                               Input.GetAxis("Mouse Y") * dragSpeed * Time.deltaTime);
			currentSel.CancelCmds();
			
		}else{
			// Move camera if mouse pointer reaches screen borders
            if (Input.mousePosition.x < scrollArea)
            {
                translation += Vector3.right * -scrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.x >= Screen.width - scrollArea)
            {
                translation += Vector3.right * scrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.y < scrollArea)
            {
                translation += Vector3.forward * -scrollSpeed * Time.deltaTime;
            }

            if (Input.mousePosition.y > Screen.height - scrollArea)
            {
                translation += Vector3.forward * scrollSpeed * Time.deltaTime;
            }
		}
		
		
		var desiredPosition = camera.transform.position + translation;
        if (desiredPosition.x < levelAreaXMin || levelAreaXMax < desiredPosition.x)
        {
            translation.x = 0;
        }
        if (desiredPosition.y < zoomMax || zoomMin < desiredPosition.y)
        {
            translation.y = 0;
        }
        if (desiredPosition.z < levelAreaZMin || levelAreaZMax < desiredPosition.z)
        {
            translation.z = 0;
        }
		
		
		transform.position += translation;
	}
	
	// Update is called once per frame
	void Update () {
		if(sMachine.InGame && !sMachine.InBusy)
			Panning();
		if(moveCam)
			TranslateMainCam(0.5f);
	}
}
