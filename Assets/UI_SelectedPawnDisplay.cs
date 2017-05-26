using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectedPawnDisplay : MonoBehaviour {
    [SerializeField]
    Text pawnName;
    [SerializeField]
    Text pawnMovement;
    [SerializeField]
    Text pawnOwner;

    Pawn currentSelectedPawn;

    void Start()
    {

    }

    public void OnPawnSelected(Pawn newPawn)
    {
        //if we pass null, clear the texts
        currentSelectedPawn = newPawn;
        if (newPawn == null)
        {
            pawnName.text = "";
            pawnMovement.text = "";
            pawnOwner.text = "";
        }
        else
        {
            pawnName.text = currentSelectedPawn.GetPawnType();
            pawnMovement.text = currentSelectedPawn.GetRemainingMovement();
            pawnOwner.text = currentSelectedPawn.GetPlayer().PlayerName;
        }       
    }

    public void OnSelectedPawnUpdated()
    {
        if (currentSelectedPawn != null)
        {
            pawnName.text = currentSelectedPawn.GetPawnType();
            pawnMovement.text = currentSelectedPawn.GetRemainingMovement();
        }
    }
}
