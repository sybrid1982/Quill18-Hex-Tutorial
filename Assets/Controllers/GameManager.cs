using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    HexMap hexMap;
    PawnComponent selectedPawnComponent;

	// Use this for initialization
	void Start () {
        selectedPawnComponent = null;
        hexMap = FindObjectOfType<HexMap>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetSelectedPawn(PawnComponent pawn)
    {
        selectedPawnComponent = pawn;
    }

    void ClearSelectedPawn()
    {
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

    void MovePawnVisual(Hex hex)
    {
        Transform pawnTransform = selectedPawnComponent.gameObject.transform;
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
}
