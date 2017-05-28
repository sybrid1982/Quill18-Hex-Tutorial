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

    public void OnPawnSelected(Pawn newPawn)
    {
        //if the new pawn isn't null, set the texts, otherwise, clear them
        currentSelectedPawn = newPawn;
        if (newPawn != null)
        {
            pawnName.text = currentSelectedPawn.GetPawnType();
            pawnMovement.text = currentSelectedPawn.GetRemainingMovement();
            pawnOwner.text = currentSelectedPawn.GetPlayer().PlayerName;
        }
        else
        {
            pawnName.text = "";
            pawnMovement.text = "";
            pawnOwner.text = "";
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
