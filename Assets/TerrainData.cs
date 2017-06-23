using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* If we're going to start cities that get resources from nearby hexes then we need
 * some way to know what kind of resources a terrain should generate
 * Since we are already using an enum to then get values for movement costs for hexes
 * from their terrain, let's just put all that data in another class that can be referenced */

/* Ideally we'll initalize all the terrain types (maybe in HexMap) and then refer hexes to one
 * the terrain types without making more than one TerrainData for each terraintype */

/* This may also be a better place to put things like the height limits and moisture limits
 * to determine what kind of terrain a hex will have */

public enum TerrainType
{
    GRASS,
    MOUNTAIN,
    ARID_HILLS,
    GRASSY_HILLS,
    DESERT,
    WATER
}

public class TerrainData {


    TerrainType terrainType;
    int movementCost;
    int coinsPerTile;
    int foodPerTile;
    int buildPerTile;

    //Create TerrainData - There'll be one of these objects for each type in the enum
    public TerrainData (TerrainType ttype, 
                        int mc, 
                        int cpt,
                        int fpt,
                        int bpt)
    {
        terrainType = ttype;
        movementCost = mc;
        coinsPerTile = cpt;
        foodPerTile = fpt;
        buildPerTile = bpt;
    }

    //Get the movement cost for the terrain type
    public int GetMovementCost()
    {
        return movementCost;
    }

    public int GetCoinsForHex()
    {
        return coinsPerTile;
    }

    public int GetFoodForHex()
    {
        return foodPerTile;
    }

    public int GetBuildPerHex()
    {
        return buildPerTile;
    }

    public TerrainType GetTerrainType()
    {
        return terrainType;
    }
}
