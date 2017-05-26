using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    HexMap hexMap;
    PawnComponent selectedPawnComponent;
    UI_SelectedPawnDisplay selectedPawnDisplay;
    List<Player> players;
    Player activePlayer;

	// Use this for initialization
	void Start () {
        selectedPawnComponent = null;
        selectedPawnDisplay = FindObjectOfType<UI_SelectedPawnDisplay>();
	}

    public void StartPressed()
    {
        hexMap = FindObjectOfType<HexMap>();
        players = new List<Player>();
        string newPlayerName = "Syd";
        Player newPlayer = new Player(newPlayerName);
        players.Add(newPlayer);
        activePlayer = newPlayer;

        hexMap.StartPressed();
        activePlayer.StartTurn();
    }
	// Update is called once per frame
	void Update () {
		
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
        Debug.Log("PawnClicked on GameManager triggered");
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
            selectedPawnComponent.pawn.SetMoveTarget(hex);
        }
    }

    void MovePawnVisual(Pawn pawn, Hex hex)
    {
        GameObject pawnGO = hexMap.GetPawnGOFromPawn(pawn);
        Transform pawnTransform = pawnGO.transform;
        //going to do a very hacky move for now - teleport to the tile clicked
        //step one, unparent the selectedPawnComponent from the tile it is currently on
        pawnTransform.SetParent(null);
        //step two, move the pawn to the new location
        pawnTransform.position = hex.PositionFromCamera(Camera.main.transform.position, hexMap.NumRows(), hexMap.NumColumns());
        //Step three, tell the pawn it has moved to a new location
        selectedPawnComponent.pawn.SetMyHex(hex);
        //step four, parent the pawn to the new hex
        pawnTransform.SetParent(hexMap.GetHexGOFromHex(hex).transform);
    }

    public Player GetActivePlayer()
    {
        return activePlayer;
    }

    public List<Player> GetPlayers()
    {
        return players;
    }

}
