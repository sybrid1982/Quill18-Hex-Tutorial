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
    Vector3 newPosition;
    HexComponent[] hexes;
    HexMap hexMap;

    float maxCameraUp;
    float maxCameraRight;

    // Use this for initialization
    void Start () {
        oldPosition = newPosition = this.transform.position;
        hexMap = FindObjectOfType<HexMap>();
        maxCameraUp = (hexMap.NumRows() * 1.5f) - 5;
	}
    

	// Update is called once per frame
	void Update () {

        //Now the various move scripts can tell the camera to move
        MoveCamera();
        AdjustCameraAngle();
        //should be the last thing atm
        CheckIfCameraMoved();
	}

    public void PanToHex(Hex hex)
    {
        //TODO:  Move Camera to Hex
    }

    public void MoveToPosition(Vector3 position)
    {
        position.y = this.transform.position.y;
        position.z += -5;
        newPosition = position;
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

    public void SetMoveTargetPosition(int upDown, int leftRight)
    {
        float newX = transform.position.x + (leftRight * Time.deltaTime);
        float newZ = transform.position.z + (upDown * Time.deltaTime);

        newZ = Mathf.Clamp(newZ, -10, maxCameraUp);
        //TODO:  if you get too far to the right, reset the camera's position to 0 on the x axis

        newPosition.x = newX;
        newPosition.z = newZ;
    }

    public void ZoomCamera(int direction)
    {
        float newY = transform.position.y + (direction * cameraZoomSpeed);

        newY = Mathf.Clamp(newY, maxCameraZoomIn, maxCameraZoomOut);

        newPosition.y = newY;
    }

    void AdjustCameraAngle()
    {
        // Change camera angle
        float lowZoom = maxCameraZoomIn + 3;
        float highZoom = maxCameraZoomOut - 10;

        Camera.main.transform.rotation = Quaternion.Euler(
            Mathf.Lerp(30, 75, Camera.main.transform.position.y / maxCameraZoomOut),
            Camera.main.transform.rotation.eulerAngles.y,
            Camera.main.transform.rotation.eulerAngles.z
        );
    }

    void MoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
    }

}
