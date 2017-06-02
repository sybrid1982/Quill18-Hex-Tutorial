using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pawn is the class for units that move on the game map, like characters
//or enemies or whatever.  If it can move (and probably later attack), 
//it's a pawn.

public class Pawn {

    //Constructor for actual game pawns
    //Needs a pawn prototype to clone and a start hex
    //Where it should be initially placed
    public Pawn(Pawn prototypePawn, Hex startHex, Player owningPlayer)
    {
        this.myHex = startHex;
        this.pawnType = prototypePawn.pawnType;
        this.movement = prototypePawn.movement;
        this.owningPlayer = owningPlayer;
    }

    //Constructor for pawn prototypes
    //This one should take every stat for a type of pawn
    //and keep it, but should never be instantiated itself
    public Pawn(string pawnType, int passedMovement)
    {
        this.pawnType = pawnType;
        this.movement = passedMovement;
    }
    
    Hex myHex;
    //targetHex is the final stop for a pathfinding run
    Hex targetHex;
    Hex tempHex;
    //currentGoalHex is the next hex the pawn wants to move to
    Hex currentGoalHex;
    //similarly this is for pathfinding
    HexPath myHexPath;
    HexPath tempHexPath;
    //movement is how many hexes this unit can move in a turn
    int movement;
    //meanwhile remaining movement is how many hexes this unit can move at this moment
    int remainingMovement;
    //And who owns this pawn
    Player owningPlayer;

    //pawnType is just a way of knowing what kind of pawn a given
    //pawn is
    //This may never come up
    string pawnType;

    //probably need a delegate to control pawn movement - ideally
    //the code here should handle all actual movement, and then
    //the delegate will say "hey, I've moved so go ahead and move
    //my displayed position."
    public delegate void PawnMovesToNewHex(Pawn pawn, Hex hex);
    public event PawnMovesToNewHex pawnMoveEvent;

    public void SetMyHex(Hex newHex)
    {
        myHex = newHex;
        //pawnMoveEvent(this, newHex);
    }
    public Hex GetMyHex()
    {
        return myHex;
    }

    public bool SetMoveTarget(Hex hex)
    {
        targetHex = hex;
        //Ok, so, this pawn will need a hexpath made for it then
        myHexPath = new HexPath(myHex.hexMap, myHex, targetHex);
        //if we've got a valid move path, let the game manager know
        if (myHexPath != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetTempPathToCurrentPath()
    {
        targetHex = tempHex;
        myHexPath = new HexPath(myHex.hexMap, myHex, targetHex);
    }

    public bool PathfindToTile(Hex hex)
    {
        tempHex = hex;
        tempHexPath = new global::HexPath(myHex.hexMap, myHex, tempHex);
        //if this worked, let the game manager know to draw it
        if (myHexPath != null)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void ExecuteMovement()
    {
        //while we have movement left this turn AND there are hexes left to move to
        //if we have remaining movement but no tiles left to move to there's no point in trying to move further
        //and if we have tiles left to move to and are out of movement, we shouldn't be moving
        while(remainingMovement > 0 && myHexPath.Length() > 0)
        {
            //get the next hex to move to
            currentGoalHex = myHexPath.GetNextHex();
            //move to that hex
            SetMyHex(currentGoalHex);
            //TODO: IMPLEMENT ABOVE
            //Deplete remaining movement
            remainingMovement -= currentGoalHex.MovementCost;
        }
    }

    public string GetPawnType()
    {
        return pawnType;
    }

    public string GetRemainingMovementString()
    {
        string movementString;
        movementString = remainingMovement + " / " + movement;
        return movementString;
    }

    public int GetRemainingMovement()
    {
        return remainingMovement;
    }

    public Player GetPlayer()
    {
        return owningPlayer;
    }

    public void BeginPawnTurn()
    {
        Debug.Log("Begin Pawn Turn activated");
        remainingMovement = movement;
    }



    public Stack<Hex> CopyMovementHexStack(bool wantTempPath)
    {
        Stack<Hex> myStack;
        if (wantTempPath)
        {
            myStack = tempHexPath.ClonePathStack();
        }
        else
        {
            myStack = myHexPath.ClonePathStack();
        }

        return myStack;
    }
}
