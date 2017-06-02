using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    HexMap hexMap;
    PawnComponent selectedPawnComponent;
    UI_SelectedPawnDisplay selectedPawnDisplay;
    UI_PathDrawer uiPathDrawer;
    List<Player> players;
    Player activePlayer;
    PawnKeeper pawnKeeper;

	// Use this for initialization
	void Start () {
        selectedPawnComponent = null;
        selectedPawnDisplay = FindObjectOfType<UI_SelectedPawnDisplay>();
        uiPathDrawer = FindObjectOfType<UI_PathDrawer>();
	}

    public void StartPressed()
    {
        hexMap = FindObjectOfType<HexMap>();
        pawnKeeper = FindObjectOfType<PawnKeeper>();
        players = new List<Player>();
        string newPlayerName = "Syd";
        Player newPlayer = new Player(newPlayerName);
        players.Add(newPlayer);
        activePlayer = newPlayer;

        hexMap.StartPressed();
        pawnKeeper.StartPawnkeeper();
        foreach (Player p in players)
        {
            pawnKeeper.GenerateStartPawn(p);
        }
        activePlayer.StartTurn();
    }

    void SetSelectedPawn(PawnComponent pawn)
    {
        selectedPawnDisplay.OnPawnSelected(pawn.pawn);
        selectedPawnComponent = pawn;
    }

    void ClearSelectedPawn()
    {
        selectedPawnDisplay.OnPawnSelected(null);
        selectedPawnComponent = null;
    }

    public PawnComponent GetSelectedPawn()
    {
        
        return selectedPawnComponent;
    }

    public void PawnClicked(PawnComponent pawnComponent)
    {
        //Debug.Log("PawnClicked on GameManager triggered");
        if(selectedPawnComponent == pawnComponent)
        {
            ClearSelectedPawn();
        }
        else
        {
            SetSelectedPawn(pawnComponent);
        }
    }

    public void HexClicked(Hex hex)
    {
        if(selectedPawnComponent != null)
        {
            if (selectedPawnComponent.pawn.SetMoveTarget(hex))
            {
                uiPathDrawer.DisplayPathForPawn(selectedPawnComponent.pawn);
            }
        }
    }

    void MovePawnVisual(Pawn pawn, Hex hex)
    {
        GameObject pawnGO = pawnKeeper.GetPawnGOFromPawn(pawn);
        Transform pawnTransform = pawnGO.transform;
        //going to do a very hacky move for now - teleport to the tile clicked
        //step one, unparent the selectedPawnComponent from the tile it is currently on
        pawnTransform.SetParent(null);
        //step two, move the pawn to the new location
        pawnTransform.position = hex.PositionFromCamera(Camera.main.transform.position, hexMap.NumRows(), hexMap.NumColumns());
        //Step three, set the pawn's parent to the new tile
        pawnTransform.SetParent(hexMap.GetHexGOFromHex(hex).transform);
    }

    public void SelectedPawnDrag(Hex hexToDrawTo)
    {
        
        if (selectedPawnComponent == null)
        {
            //this shouldn't be called in that case, so for now just put in a log statement 'cause that's weird
            Debug.LogWarning("Somehow you've asked a non-existent pawn to pathfind");
        } else
        {
            //Is there a path to that tile?
            if (selectedPawnComponent.pawn.PathfindToTile(hexToDrawTo))
                //then draw it
                uiPathDrawer.DisplayPathForPawn(selectedPawnComponent.pawn, true);
        }
    }

    public void LockInSelectedPawnPath()
    {
        if(selectedPawnComponent!=null)
            selectedPawnComponent.pawn.SetTempPathToCurrentPath();
    }

    //I've seperated the execution from the lock-in because in the future we will want to be able to
    //tell the pawn to execute its movement without having to mess with its temp path if it already
    //has a path to follow and the user doesn't want it changed
    public void ExecuteMoveForSelectedPawn()
    {
        if (selectedPawnComponent == null)
        {
            Debug.LogWarning("Trying to move a pawn when we don't have one selected in the game manager");
            return;
        }
        selectedPawnComponent.pawn.ExecuteMovement();
        MovePawnVisual(selectedPawnComponent.pawn, selectedPawnComponent.pawn.GetMyHex());
        //Update the UI elements
        uiPathDrawer.DisplayPathForPawn(selectedPawnComponent.pawn);
        selectedPawnDisplay.OnSelectedPawnUpdated();
    }

    public Player GetActivePlayer()
    {
        return activePlayer;
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

    public Pawn GetFirstPawnForActivePlayer()
    {
        Pawn pawn = activePlayer.GetPawnWithRemainingMove();
        return pawn;
    }

    public Vector3 GetPawnPosition(Pawn pawn)
    {
        Vector3 pawnPosition = pawnKeeper.GetPawnGOFromPawn(pawn).transform.position;
        return pawnPosition;
    }
}
