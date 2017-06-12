using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    HexMap hexMap;
    UI_SelectedPawnDisplay selectedPawnDisplay;
    UI_PathDrawer uiPathDrawer;
    PawnKeeper pawnKeeper;
    List<Player> players;
    WorldDisplay worldDisplay;

    Player activePlayer;
    PawnComponent selectedPawnComponent;

    int startingNumberOfPlayers = 2;

	// Use this for initialization
	void Start () {
        selectedPawnComponent = null;
        selectedPawnDisplay = FindObjectOfType<UI_SelectedPawnDisplay>();
        uiPathDrawer = FindObjectOfType<UI_PathDrawer>();
	}

    public void StartPressed()
    {
        hexMap = FindObjectOfType<HexMap>();
        worldDisplay = FindObjectOfType<WorldDisplay>();
        pawnKeeper = FindObjectOfType<PawnKeeper>();
        players = new List<Player>();
        for (int i = 0; i < startingNumberOfPlayers; i++)
        {
            string newPlayerName = "Syd_" + i.ToString();
            Player newPlayer = new Player(newPlayerName);
            players.Add(newPlayer);
        }
        hexMap.StartPressed();
        worldDisplay.Initialize(hexMap);

        pawnKeeper.StartPawnkeeper();
        foreach (Player p in players)
        {
            pawnKeeper.GenerateStartPawn(p);
        }
        //right now grab the first player in the index
        //in the future could instead grab a random player index
        SetActivePlayer(players[0]);

        GetFirstPawnForActivePlayerPositionAndFocusCamera();
    }

    public void SetNumberOfPlayers(Dropdown dropDownMenu)
    {
        //index 0 is 2, and it goes up from there
        //so index 1 is 3...  so just add 2 to the dropdown's value
        startingNumberOfPlayers = dropDownMenu.value + 2;
    }

    void SetSelectedPawn(PawnComponent pawn)
    {
        selectedPawnDisplay.OnPawnSelected(pawn.pawn);
        selectedPawnComponent = pawn;
    }

    void ClearSelectedPawn()
    {
        uiPathDrawer.ClearPath();
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
        pawnTransform.SetParent(worldDisplay.GetHexGOFromHex(hex).transform);
        //Step four, hey we moved a pawn, so let's go ahead and also change what tiles are visible
        worldDisplay.DisplayMapForPlayer(pawn.GetPlayer());
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
        //This should probably be a two step loop here
        //first, we tell the pawn to move
        //then, we tell the visual to move
        //once the visual is moved (it should probably have a flag or something?)
        //then go back and see if the pawn can move another hex
        selectedPawnComponent.pawn.ExecuteMovement();
        MovePawnVisual(selectedPawnComponent.pawn, selectedPawnComponent.pawn.GetMyHex());
        
        //Update the UI elements
        uiPathDrawer.DisplayPathForPawn(selectedPawnComponent.pawn);
        selectedPawnDisplay.OnSelectedPawnUpdated();
        //Finally we should check to see if the player has any pawns with remaining movement
        //If they don't, they should be informed of this so they know that they should end their turn
        //which... they can't do yet.  Also we only have one player at the moment.
        //So, there are issues.
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

    public void EndTurn()
    {
        int playerCount = players.Count;

        int playerIndex = players.FindIndex( x => x == activePlayer);

        if(playerIndex + 1 == playerCount)
        {
            playerIndex = 0;
        } else
        {
            playerIndex++;
        }
        ClearSelectedPawn();
        SetActivePlayer(players[playerIndex]);
        activePlayer.StartTurn();
        GetFirstPawnForActivePlayerPositionAndFocusCamera();
    }

    public void GetFirstPawnForActivePlayerPositionAndFocusCamera()
    {
        Pawn pawn = GetFirstPawnForActivePlayer();
        if (pawn != null)
        {
            Vector3 pawnPosition = GetPawnPosition(pawn);
            Camera.main.GetComponent<CameraMotionHandler>().MoveToPosition(pawnPosition);
        } else
        {
            //probably make the End Turn button flash yellow or something
        }
    }

    public void SetActivePlayer(Player player)
    {
        activePlayer = player;
        player.StartTurn();
        worldDisplay.DisplayMapForPlayer(player);
    }

    public WorldDisplay GetWorldDisplay()
    {
        return worldDisplay;
    }
}
