using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    HashSet<Hex> revealedHexes;
    HashSet<Hex> currentlyVisibleHexes;

    bool startedFirstTurn = false;

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
        revealedHexes = new HashSet<Hex>();
        currentlyVisibleHexes = new HashSet<Hex>();
        playerName = name;
    }

    public void StartTurn()
    {
        if (myPawns.Count > 0)
            foreach (Pawn p in myPawns)
            {
                p.BeginPawnTurn();
            }

        if (startedFirstTurn == false)
            StartFirstTurn();
    }

    public void StartFirstTurn()
    {
        if (myPawns.Count > 0)
            foreach (Pawn p in myPawns)
            {
                SetHexesVisible(p.GetVisibleHexes());
            }
        startedFirstTurn = true;
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
        p.pawnMoveEvent += PawnMoved;
    }

    public void PawnMoved(Pawn p)
    {
        /*currently not going to use passed pawn p
         At least not in this function (likely will for animating movement later)
         This function will first clear the visible hexes list, then cycle through
         every pawn and get their visible hexes*/
        currentlyVisibleHexes.Clear();
        foreach (Pawn pawn in myPawns)
        {
            SetHexesVisible(pawn.GetVisibleHexes());
        }
        //World display needs to know that we've changed what is visible as well
        //Pass through GameManager who will then inform WorldDisplay

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

    public Hex[] GetVisibleHexes()
    {
        return currentlyVisibleHexes.ToArray();
    }

    public Hex[] GetRevealedButNotVisibleHexes()
    {
        //These hexes will ideally have some sort of 'fog of war' element
        //applied to them.  For now maybe just make their material half alpha?
        List<Hex> returnedHexes = new List<Hex>();

        foreach(Hex h in revealedHexes)
        {
            if (currentlyVisibleHexes.Contains(h))
            {
                //Then don't add it to the list we're building
            } else
            {
                returnedHexes.Add(h);
            }
        }
        return returnedHexes.ToArray();
    }
    
    void SetHexesVisible(Hex[] hexes)
    {
        foreach (Hex hex in hexes)
        {
            currentlyVisibleHexes.Add(hex);
            revealedHexes.Add(hex);
        }
    }
}
