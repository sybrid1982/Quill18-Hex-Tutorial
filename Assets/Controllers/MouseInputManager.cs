using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : MonoBehaviour {

    [SerializeField]
    int hexLayer;

    HexMap hexMap;
    GameManager gameManager;
    bool isDraggingFromPawn;
    Hex lastHexDraggedOver;

	// Use this for initialization
	void Start () {
        hexMap = FindObjectOfType<HexMap>();
        gameManager = FindObjectOfType<GameManager>();
        isDraggingFromPawn = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        EvaluateMouseScrollWheel();

        EvaluateLeftMouseClick();

        EvaluateLeftMouseDrag();

        EvaluateLeftMouseRelease();
    }

    private static void EvaluateMouseScrollWheel()
    {
        float mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheelScroll < 0)
        {
            Camera.main.GetComponent<CameraMotionHandler>().ZoomCamera(1);
        }
        else if (mouseWheelScroll > 0)
        {
            Camera.main.GetComponent<CameraMotionHandler>().ZoomCamera(-1);
        }
    }

    private void EvaluateLeftMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseSpot = Input.mousePosition;
            //Send this to the HexMap
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(mouseSpot), out hit))
            {
                if (hit.collider.GetComponentInParent<PawnComponent>() != null)
                {
                    //Debug.Log("Clicked a pawn");
                    PawnComponent pawnComponent = hit.collider.GetComponentInParent<PawnComponent>();
                    gameManager.PawnClicked(pawnComponent);
                    //Ok, so, plan:
                    //While the mouse is down, after clicking on a pawn (with movement?), check to see if it is over a (new) tile
                    //if it is over a new tile, pathfind from the pawn to that new tile
                    //then draw the path
                    //if the mouse is released while over a tile and dragging a pawn, then start moving along the path
                    //if a key (escape for now) is pressed to abort the drag, then cancel the drag without starting the move
                    isDraggingFromPawn = true;
                    lastHexDraggedOver = hit.collider.GetComponentInParent<HexComponent>().hex;
                }
                else if (hit.collider.GetComponentInParent<HexComponent>() != null)
                {
                    HexComponent hexComponent = hit.collider.GetComponentInParent<HexComponent>();
                    gameManager.HexClicked(hexComponent.hex);
                }
            }
        }
    }

    private void EvaluateLeftMouseDrag()
    {
        //if we aren't dragging, get out right away
        if (!isDraggingFromPawn)
            return;


        Vector3 mouseSpot = Input.mousePosition;
        RaycastHit hit;

        //create a mask so we only ask about being over a hex and not pawns or
        //whatever else we end up with on the map
        int layerMask = CreateLayerMask(hexLayer);

        //Are we over a hex?
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mouseSpot), out hit, 100f, layerMask))
        {
            
            Hex hexMouseIsOver = hit.collider.GetComponentInParent<HexComponent>().hex;
            //is this the same hex we evaluated earlier?
            if (hexMouseIsOver == lastHexDraggedOver)
            {
                return;
            }
            //if it isn't, then we need to do a few things
            //first, set lasthexdraggedover to this hex
            lastHexDraggedOver = hexMouseIsOver;
            //second, tell the gamemanager that the drag is over a new tile
            gameManager.SelectedPawnDrag(lastHexDraggedOver);

        }
    }

    private void EvaluateLeftMouseRelease()
    {
        //if we aren't dragging, we don't care
        if (isDraggingFromPawn == false)
            return;
        //else we were dragging and should check if the drag ended
        if (Input.GetMouseButtonUp(0))
        {
            isDraggingFromPawn = false;
            gameManager.LockInSelectedPawnPath();
            gameManager.ExecuteMoveForSelectedPawn();
        }
    }

    private int CreateLayerMask(int layer)
    {
        int layermask = 1 << layer;
        return layermask;
    }

    public void AbortDrag()
    {
        isDraggingFromPawn = false;
    }
}
