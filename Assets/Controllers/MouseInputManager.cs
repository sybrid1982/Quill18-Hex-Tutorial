using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : MonoBehaviour {

    HexMap hexMap;
    GameManager gameManager;

	// Use this for initialization
	void Start () {
        hexMap = FindObjectOfType<HexMap>();
        gameManager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        float mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheelScroll<0)
        {
            Camera.main.GetComponent<CameraMotionHandler>().ZoomCamera(1);
        } else if (mouseWheelScroll > 0)
        {
            Camera.main.GetComponent<CameraMotionHandler>().ZoomCamera(-1);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseSpot = Input.mousePosition;
            //Send this to the HexMap
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(mouseSpot), out hit))
            {
                if (hit.collider.GetComponentInParent<PawnComponent>() != null)
                {
                    Debug.Log("Clicked a pawn");
                    PawnComponent pawnComponent = hit.collider.GetComponentInParent<PawnComponent>();
                    gameManager.PawnClicked(pawnComponent);
                }
                else if (hit.collider.GetComponentInParent<HexComponent>() != null)
                {
                    Debug.Log("clicked a hex");
                    HexComponent hexComponent = hit.collider.GetComponentInParent<HexComponent>();
                    gameManager.HexClicked(hexComponent.hex);
                }
                
            }
            
        }
	}
}
