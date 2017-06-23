using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputManager : MonoBehaviour {

    [SerializeField]
    int hexLayer;
    [SerializeField]
    float maxTimeAllowedToDoubleClick = 0.5f;

    HexMap hexMap;
    GameManager gameManager;
    bool isDraggingFromPawn;
    bool mouseDownOnPawn;
    Hex lastHexDraggedOver;
    float timeSinceLastClick;
    Hex lastHexClicked;

	// Use this for initialization
	void Start () {
        hexMap = FindObjectOfType<HexMap>();
        gameManager = FindObjectOfType<GameManager>();
        isDraggingFromPawn = false;
        mouseDownOnPawn = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        EvaluateMouseScrollWheel();

        EvaluateLeftMouseClick();

        EvaluateLeftMouseDrag();

        EvaluateLeftMouseRelease();
    }

    private void AdvanceTimeSinceLastClick()
    {
        timeSinceLastClick += Time.deltaTime;
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
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) 
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
                    lastHexDraggedOver = hit.collider.GetComponentInParent<HexComponent>().hex;
                    //note that we clicked on a pawn
                    mouseDownOnPawn = true;
                }
                else if (hit.collider.GetComponentInParent<HexComponent>() != null)
                {
                    HexComponent hexComponent = hit.collider.GetComponentInParent<HexComponent>();
                    if(lastHexClicked == hexComponent.hex && timeSinceLastClick <= maxTimeAllowedToDoubleClick)
                    {
                        //handle double click behavior
                        //Which should be moving the pawn to the clicked hex
                    } else
                    {
                        gameManager.HexClicked(hexComponent.hex);
                    }
                    mouseDownOnPawn = false;
                    lastHexClicked = hexComponent.hex;
                }
            }
            timeSinceLastClick = 0.0f;
        }
    }

    private void EvaluateLeftMouseDrag()
    {
        //if we aren't clicking and we didn't start that click on a pawn, we don't care
        if (Input.GetMouseButton(0) == false || mouseDownOnPawn == false)
            return;

        //we are clicking, and we started clicking on a pawn
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
            //third, we are now dragging, as we've moved the mouse off the tile that had the pawn initially
            isDraggingFromPawn = true;
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
            mouseDownOnPawn = false;
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
