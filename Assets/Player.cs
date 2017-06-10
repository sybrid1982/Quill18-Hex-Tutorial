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
        //Idea:  At the start of your first turn,
        //you need to poll each of your starting unit(s),
        //to find out what hexes you can see.  For now,
        //assuming you see hex your unit is in and all of that
        //hexes neighbors.  Thankfully, we can ask the pawn what space
        //it is in and what neighbors it has
        if (myPawns.Count > 0)
            foreach (Pawn p in myPawns)
            {
                revealedHexes.Add(p.GetMyHex());
                currentlyVisibleHexes.Add(p.GetMyHex());
                foreach (Hex neighbor in p.GetMyHex().GetNeighbors())
                {
                    revealedHexes.Add(neighbor);
                    currentlyVisibleHexes.Add(neighbor);
                }
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
    }

    public void PawnMoved(Pawn p)
    {
        //When a pawn moves, different hexes become visible and some undiscovered ones
        //can become discovered
        //Might need the old hex to know which spaces to first mark as invisible
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
    
}
