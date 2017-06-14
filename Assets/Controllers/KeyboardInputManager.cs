using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : MonoBehaviour {

    KeyCode up;
    KeyCode down;
    KeyCode left;
    KeyCode right;

    KeyCode focusOnUnit;

    GameManager gameManager;

    bool keysSetInMenu = false;

    CameraMotionHandler cameraMotionHandler;

	// Use this for initialization
	void Start () {
        if (!keysSetInMenu)
        {
            up = KeyCode.W;
            down = KeyCode.S;
            left = KeyCode.A;
            right = KeyCode.D;
            focusOnUnit = KeyCode.F;
        }
        gameManager = FindObjectOfType<GameManager>();
        cameraMotionHandler = Camera.main.GetComponent<CameraMotionHandler>();
	}
	
	// Update is called once per frame
	void Update () {
        int upDown = 0;
        int leftRight = 0;
        if (Input.GetKey(up))
        {
            upDown = 1;
        } else if (Input.GetKey(down))
        {
            upDown = -1;
        }
        if (Input.GetKey(left))
        {
            leftRight = -1;
        }
        else if (Input.GetKey(right))
        {
            leftRight = 1;
        }
        if (upDown != 0 || leftRight != 0)
        {
            cameraMotionHandler.MoveCamera(upDown, leftRight);
        }
        if (Input.GetKeyDown(focusOnUnit))
        {
            gameManager.GetFirstPawnForActivePlayerPositionAndFocusCamera();
        }

    }
}
