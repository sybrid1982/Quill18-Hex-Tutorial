using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:  Hexmap currently handles both data and the display of that data
//DEFINITELY NEED TO CHANGE THAT
//If HexMap just handled knowing about the map stuff, then could have a 
//HexMapDisplay object whose job it was to display hexes.  For instance, when
//Passed a player, the HexMapDisplay could set all the visuals to black EXCEPT
//The ones which the player can see or has seen at some point
public class HexMap : ScriptableObject {

    [SerializeField]
    int numRows = 20;
    [SerializeField]
    int numColumns = 40;

    public float HeightMountain = 1.5f;
    public float HeightHill = 1.0f;
    public float HeightFlat = 0.0f;

    public float MoistureRainforest = 1f;
    public float MoistureForest = .75f;
    public float MoistureGrassland = 0.5f;
    public float MoisturePlains = 0.25f;

    private Hex[,] hexes;
    private List<Hex> walkableHexes;
    
    public HexGraph hexGraph;

    public int NumRows()
    {
        return numRows;
    }

    public int NumColumns()
    {
        return numColumns;
    }

    public void StartPressed()
    {
        walkableHexes = new List<Hex>();
        GenerateMap();
    }

    virtual public void GenerateMap()
    {
        hexes = new Hex[NumColumns(), NumRows()];
        for (int column = 0; column < numColumns; column++)
        {
            GenerateColumn(column);
        }
    }

    private void GenerateColumn(int column)
    {
        for (int row = 0; row < numRows; row++)
        {
            GenerateHex(column, row);
        }
    }

    private void GenerateHex(int column, int row)
    {
        Hex h = new Hex(this, column, row);

        h.Elevation = -0.5f;
        hexes[column, row] = h;
        
        //Generate Neighbors for the tile
        GenerateNeighbors(h);
    }

    void GenerateNeighbor(Hex h, Vector2 neighborToCheck, int directionIndex)
    {
        Hex hexToCheck = GetHexAt(neighborToCheck);
        if (hexToCheck != null)
        {
            Direction directionOfNeighbor = (Direction)directionIndex;
            h.SetNeighbor(hexToCheck, directionOfNeighbor);
            hexToCheck.SetNeighbor(h, ReverseDirection(directionOfNeighbor));
        }
    }

    Direction ReverseDirection(Direction directionToReverse)
    {
        int directionIndex = (int)(directionToReverse);
        if (directionIndex >= 3)
        {
            return (Direction)(directionIndex - 3);
        }
        else
        {
            return (Direction)(directionIndex + 3);
        }
    }

    void GenerateNeighbors(Hex h)
    {
        //When a new hex is created there are three easy places to check for neighbors
        //To the left of the hex, the bottom-left of the hex, and bottom-right
        //neighbor to the left
        Vector2[] potentialNeighborCoords = {   new Vector2 (h.Q - 1, h.R + 1),     //upper left
                                                new Vector2 (h.Q - 1, h.R),         //left
                                                new Vector2 (h.Q, h.R - 1)          //lower left
                                                };    
        for (int i = 0; i < 3; i++)
        {
            GenerateNeighbor(h, potentialNeighborCoords[i], i+2);
        }
        //HOWEVER, if this is the last tile in a row, there are two
        //other neighbors to consider to wrap the map
        if(h.Q == numColumns - 1)
        {
            GenerateWrapNeighbors(h);
        }
    }

    private void GenerateWrapNeighbors(Hex h)
    {
        Hex rightH = GetHexAt(0, h.R);
        if (rightH != null)
        {
            h.SetNeighbor(rightH, Direction.RIGHT);
            rightH.SetNeighbor(h, Direction.LEFT);
        }
        Hex lowerRightH = GetHexAt(0, h.R - 1);
        if(lowerRightH !=null)
        {
            h.SetNeighbor(lowerRightH, Direction.LOWER_RIGHT);
            lowerRightH.SetNeighbor(h, Direction.UPPER_LEFT);
        }
    }


    //Returns the hex at coordinates Q, R
    //If Q,R is out of the map bounds, returns null
    //if Q is greater than the number of columns or less than zero,
    //then we can return what hex that would be from wrapping
    //IE: if we have 30 columns and ask for 32, 5 return 2, 5
    public Hex GetHexAt(int Q, int R)
    {
        if(hexes == null)
        {
            throw new UnityException("Hexes array not yet instantiated, you done goofed!");
        }

        Q = Q % numColumns;
        if (Q < 0)
            Q += numColumns;

        try
        {
            return hexes[Q, R];
        } catch
        {
            return null;
        }
    }

    public Hex GetHexAt(Vector2 coords)
    {

        return GetHexAt((int)coords.x, (int)coords.y);

    }

    public Hex GetRandomWalkableHexFromHexMap()
    {
        int hexIndex = Random.Range(0, walkableHexes.Count);

        return walkableHexes[hexIndex];
    }

    public Hex GetAcceptableStartPosition()
    {
        int acceptableNumberOfImpassible = 2;
        int rangeOfZoneToCheck = 2;
        Hex possibleHex = null;
        bool foundAcceptableTile = false;

        while (!foundAcceptableTile) {
            int hexIndex = Random.Range(0, walkableHexes.Count);
            possibleHex = walkableHexes[hexIndex];

            Hex[] neighbors = GetHexesWithRadiusOf(possibleHex, rangeOfZoneToCheck);
            int impassibleTiles = 0;
            foreach (Hex h in neighbors)
            {
                if (h == null)
                    Debug.LogWarning("Null neighbor, something is borked");
                else if (h.GetTerrain() == Terrain.MOUNTAIN || h.GetTerrain() == Terrain.WATER)
                {
                    impassibleTiles++;
                }
            }
            if (impassibleTiles <= acceptableNumberOfImpassible)
                foundAcceptableTile = true;
        }

        return possibleHex;
    }
    
    public Hex[] GetHexesWithRadiusOf(Hex centerHex, int radius)
    {
        List<Hex> results = new List<Hex>();
        for (int dx = -radius; dx < radius; dx++)
        {
            for (int dy = Mathf.Max(-radius, -dx - radius); dy < Mathf.Min(radius, -dx + radius); dy++)
            {
                results.Add(GetHexAt(centerHex.Q + dx, centerHex.R + dy));
            }
        }
        return results.ToArray();
    }

    protected void SetTerrainForHex(Hex hex)
    {
        if (hex.Elevation >= HeightMountain)
            hex.SetTerrain(Terrain.MOUNTAIN);
        else if (hex.Elevation >= HeightHill)
        {
            walkableHexes.Add(hex);
            if (hex.Moisture >= MoisturePlains)
                hex.SetTerrain(Terrain.GRASSY_HILLS);
            else
                hex.SetTerrain(Terrain.ARID_HILLS);
        }
        else if (hex.Elevation >= HeightFlat)
        {
            walkableHexes.Add(hex);
            if (hex.Moisture >= MoisturePlains)
                hex.SetTerrain(Terrain.GRASS);
            else
                hex.SetTerrain(Terrain.DESERT);
        }
        else
            hex.SetTerrain(Terrain.WATER);
    }
}
