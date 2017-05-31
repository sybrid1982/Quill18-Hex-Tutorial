using System.Collections;
using System.Collections.Generic;

//The player class stores information like "what pawns does this player control
//It has functions for:
//starting the player's turn (which should go through and refresh all of that player's stuff
//ending the player's turn
//getting a list of the player's stuff
//asking a player if a thing belongs to that player
//probably more stuff later as we add stuff to the game but right now there's only pawns

public class Player  {
    List<Pawn> myPawns;
    string playerName;

    public string PlayerName
    {
        get
        {
            return playerName;
        }

        protected set
        {
            playerName = value;
        }
    }

    public Player(string name)
    {
        myPawns = new List<Pawn>();
        playerName = name;
    }

    public void StartTurn()
    {
        if (myPawns.Count > 0)
            foreach (Pawn p in myPawns)
                p.BeginPawnTurn();
    }

    public void EndTurn()
    {
        //at this point I'm not sure if this will do anything
    }

    public List<Pawn> GetMyPawns()
    {
        return myPawns;
    }

    public bool IsThisYourPawn(Pawn pawn)
    {
        if( myPawns.Contains(pawn))
            return true;
        else
            return false;
    }

    public void AddPawn(Pawn p)
    {
        myPawns.Add(p);
    }

    public Pawn GetPawnWithRemainingMove()
    {
        foreach (Pawn p in myPawns)
        {
            if (p.GetRemainingMovement() > 0)
            {
                return p;
            }
        }
        //null means there are no pawns with remaining movement
        return null;
    }
}
