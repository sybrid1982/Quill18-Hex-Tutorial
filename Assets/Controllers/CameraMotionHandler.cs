using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotionHandler : MonoBehaviour {

    [SerializeField]
    float cameraMoveSpeed;
    [SerializeField]
    float cameraZoomSpeed;
    [SerializeField]
    float maxCameraZoomIn;
    [SerializeField]
    float maxCameraZoomOut;

    Vector3 oldPosition;
    HexComponent[] hexes;
    HexMap hexMap;

    float maxCameraUp;
    float maxCameraRight;

    // Use this for initialization
    void Start () {
        oldPosition = this.transform.position;
        hexMap = FindObjectOfType<HexMap>();
        maxCameraUp = (hexMap.NumRows() * 1.5f) - 5;
	}
    

	// Update is called once per frame
	void Update () { 
        
        //should be the last thing atm
        CheckIfCameraMoved();
	}

    public void PanToHex(Hex hex)
    {
        //TODO:  Move Camera to Hex
    }

    public void MoveToPosition(Vector3 position)
    {
        Debug.Log("Asked to move to a new position");
        position.y = this.transform.position.y;
        position.z += -5;
        this.transform.position = position;
    }

    void CheckIfCameraMoved()
    {
        if(oldPosition != this.transform.position)
        {
            oldPosition = this.transform.position;

            if(hexes == null)
                hexes = GameObject.FindObjectsOfType<HexComponent>();

            foreach (HexComponent hexC in hexes)
            {
                hexC.UpdatePosition();
            }
        }
    }

    public void MoveCamera(int upDown, int leftRight)
    {
        float newX = transform.position.x + (leftRight * Time.deltaTime * cameraMoveSpeed);
        float newZ = transform.position.z + (upDown * Time.deltaTime * cameraMoveSpeed);

        newZ = Mathf.Clamp(newZ, -10, maxCameraUp);
        //TODO:  if you get too far to the right, reset the camera's position to 0 on the x axis

        transform.position = new Vector3(newX, transform.position.y, newZ);
    }

    public void ZoomCamera(int direction)
    {
        float newY = transform.position.y + (direction * Time.deltaTime * cameraZoomSpeed);

        newY = Mathf.Clamp(newY, maxCameraZoomIn, maxCameraZoomOut);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

}
