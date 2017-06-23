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

    const float CAMERA_Z_ADJUST = -5;

    // Use this for initialization
    void Start () {
        oldPosition = newPosition = this.transform.position;
        hexMap = FindObjectOfType<HexMap>();
        maxCameraUp = (hexMap.NumRows() * 1.5f) - 5;
	}
    
	void Update () {
        //Various move scripts can tell the camera to move
        MoveCamera();
        AdjustCameraAngle();
        //should be the last thing atm
        CheckIfCameraMoved();
	}

    public void PanToHex(Hex hex)
    {
        //TODO:  Move Camera to Hex
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        targetPosition.y = this.transform.position.y;
        targetPosition.z += CAMERA_Z_ADJUST;
        newPosition = targetPosition;
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
        float newX, newZ;
        GetNewXandNewZ(upDown, leftRight, out newX, out newZ);

        newZ = Mathf.Clamp(newZ, -10, maxCameraUp);
        //TODO:  if you get too far to the right, reset the camera's position to 0 on the x axis

        newPosition.x = newX;
        newPosition.z = newZ;
    }

    private void GetNewXandNewZ(int upDown, int leftRight, out float newX, out float newZ)
    {
        newX = transform.position.x + (leftRight * cameraMoveSpeed * Time.deltaTime);
        newZ = transform.position.z + (upDown * cameraMoveSpeed * Time.deltaTime);
    }

    public void ZoomCamera(int direction)
    {
        float newY = transform.position.y + (direction * cameraZoomSpeed);

        newY = Mathf.Clamp(newY, maxCameraZoomIn, maxCameraZoomOut);

        newPosition.y = newY;
    }

    void AdjustCameraAngle()
    {
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
