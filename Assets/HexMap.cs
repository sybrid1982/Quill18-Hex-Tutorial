using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*HexMap handles the creation of the map, telling hexes what kind of hex they are when they're created,
 * telling other classes what hex is at a position, holding the pathfinding map (for now, that may move to players)
 * knowing what hexes are walkable, and generally being the go-to object for knowing about hexes.
 * WorldDisplay however creates the GameObjects that actually have meshes and textures and materials that link to
 * this data */
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
    private Dictionary<TerrainType, TerrainData> terrainTypeMap;
    
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
        terrainTypeMap = new Dictionary<TerrainType, TerrainData>();
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
                else if (h.GetTerrainType() == TerrainType.MOUNTAIN || h.GetTerrainType() == TerrainType.WATER)
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
            hex.SetTerrain(terrainTypeMap[TerrainType.MOUNTAIN]);
        else if (hex.Elevation >= HeightHill)
        {
            walkableHexes.Add(hex);
            if (hex.Moisture >= MoisturePlains)
                hex.SetTerrain(terrainTypeMap[TerrainType.GRASSY_HILLS]);
            else
                hex.SetTerrain(terrainTypeMap[TerrainType.ARID_HILLS]);
        }
        else if (hex.Elevation >= HeightFlat)
        {
            walkableHexes.Add(hex);
            if (hex.Moisture >= MoisturePlains)
                hex.SetTerrain(terrainTypeMap[TerrainType.GRASS]);
            else
                hex.SetTerrain(terrainTypeMap[TerrainType.DESERT]);
        }
        else
            hex.SetTerrain(terrainTypeMap[TerrainType.WATER]);
    }

    protected void CreateTerrainForHexes()
    {
        //Done setting the numbers so generate the terrain
        for (int column = 0; column < NumColumns(); column++)
        {
            for (int row = 0; column < NumRows(); column++)
            {
                Hex h = GetHexAt(row, column);
                SetTerrainForHex(h);
            }
        }
    }

    void PrototypeTerrainDataForTerrainType(TerrainType ttype, int movementCost, int cpt, int fpt, int bpt)
    {
        if (terrainTypeMap.ContainsKey(ttype))
        {
            Debug.LogError("Overwriting TerrainType " + ttype + " with new data!");
        } else
        {
            TerrainData newData = new TerrainData(ttype, movementCost, cpt, fpt, bpt);
            terrainTypeMap.Add(ttype, newData);
        }
    }

    void PrototypeTerrainData()
    {
        int hillMovementCost = 2;
        int normalMovementCost = 1;
        int impassibleMovememntCost = 0;

        //Set up Arid Hills to cost 2 movement to go over, give 1 coin and 2 production and 0 food.
        PrototypeTerrainDataForTerrainType(TerrainType.ARID_HILLS, hillMovementCost, 1, 0, 2);

        //Do the other five terrain types
        PrototypeTerrainDataForTerrainType(TerrainType.DESERT, normalMovementCost, 1, 0, 1);
        PrototypeTerrainDataForTerrainType(TerrainType.GRASS, normalMovementCost, 1, 2, 1);
        PrototypeTerrainDataForTerrainType(TerrainType.GRASSY_HILLS, hillMovementCost, 1, 2, 2);
        PrototypeTerrainDataForTerrainType(TerrainType.MOUNTAIN, impassibleMovememntCost, 1, 0, 3);
        PrototypeTerrainDataForTerrainType(TerrainType.WATER, impassibleMovememntCost, 1, 1, 0);
    }
}
