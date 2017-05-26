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
    //currentGoalHex is the next hex the pawn wants to move to
    Hex currentGoalHex;
    //similarly this is for pathfinding
    HexPath myHexPath;
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
        pawnMoveEvent(this, newHex);
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
            //Debug.Log("Path found!");
            return true;
        }
        else
        {
            //Debug.Log("No path found!");
            return false;
        }
    }

    public void ExecuteMovement()
    {
        //while we have movement left this turn
        while(remainingMovement > 0)
        {
            //get the next hex to move to
            currentGoalHex = myHexPath.GetNextHex();
            //move to that hex

            //TODO: IMPLEMENT ABOVE
            //Deplete remaining movement
            remainingMovement--;
            //Check if we've arrived
            if(currentGoalHex == targetHex)
            {
                continue;
            }
        }
    }

    public string GetPawnType()
    {
        return pawnType;
    }

    public string GetRemainingMovement()
    {
        string movementString;
        movementString = remainingMovement + " / " + movement;
        return movementString;
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

    public Stack<Hex> CopyMovementHexStack()
    {
        Stack<Hex> myStack = myHexPath.ClonePathStack();
        return myStack;
    }
}
