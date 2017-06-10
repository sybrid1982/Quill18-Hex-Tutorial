using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//Hex class defines the grid position, world space position, size
//Neighbors, etc of a hex tile, but it is a pure data class that
//Does not interact with Unity

//It also contains some hex_operators (hex_add, hex_subtract, and hex_multiply)
//and defines overrides for equality/inequality

public enum Direction {
    RIGHT,
    UPPER_RIGHT,
    UPPER_LEFT,
    LEFT,
    LOWER_LEFT,
    LOWER_RIGHT
}

public enum Terrain
{
    GRASS,
    MOUNTAIN,
    ARID_HILLS,
    GRASSY_HILLS,
    DESERT,
    WATER
}

public class Hex {
//Constructor.  We don't need S because S can be derived from Q and R
    public Hex(HexMap hexMap, int q, int r)
    {
        neighbors = new Hex[6];
        this.hexMap = hexMap;

        this.Q = q;
        this.R = r;
        this.S = -(q + r);
    }


    //Q+R+S MUST = 0;
    //So S = -(Q+R)
    public readonly int Q,      //Column 
                        R,      //Row
                        S;      //Some value

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    float radius = 1f;
    Terrain terrain;

    Hex[] neighbors;
    public readonly HexMap hexMap;

    int movementCost = 1;

    public float Elevation;
    public float Moisture;

    private Feature feature;

    HashSet<Pawn> pawns;

    public int MovementCost
    {
        get
        {
            int mc = 0;
            switch (terrain)
            {
                case Terrain.DESERT:
                case Terrain.GRASS:
                    mc = 1;
                    break;
                case Terrain.ARID_HILLS:
                case Terrain.GRASSY_HILLS:
                    mc = 2;
                    break;
                case Terrain.MOUNTAIN:
                case Terrain.WATER:
                    mc = 0;
                    break;
                default:
                    Debug.Log("Unknown terrain type");
                    break;
            }
            if(feature != null)
            {
                switch (feature.FeatureType)
                {
                    case FeatureTypes.FOREST:
                        mc++;
                        break;
                }
            }
            return mc;
        }

        protected set
        {
            movementCost = value;
        }
    }

    public float HexHeight()
    {
        return radius * 2;
    }

    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    //Returns world space position of this hex
    public Vector3 Position()
    {
        float horizontal = HexWidth();
        float vertical = HexHeight() * 0.75f;

        return new Vector3(HexHorizontalSpacing() * (this.Q + this.R * 0.5f), 0, HexVerticalSpacing() * this.R);
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numRows, float numColumns)
    {
        float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        float howManyWidthsFromCamera = (position.x - cameraPosition.x)/ mapWidth;

        if(Mathf.Abs(howManyWidthsFromCamera) <= 0.5f)
        {
            return position;
        }

        //if we are at 0.6, we want to be at -0.4
        //if we are at 2.2, we want to be at 0.2
        //2.6 => -0.4
        //-0.6 => 0.4

        if (howManyWidthsFromCamera > 0)
            howManyWidthsFromCamera += 0.5f;
        else
            howManyWidthsFromCamera += -0.5f;

        int howManyWidthsToFix = (int)howManyWidthsFromCamera;

        position.x -= howManyWidthsToFix * mapWidth;

        return position;
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition)
    {
        float mapWidth = hexMap.NumColumns() * HexHorizontalSpacing();

        Vector3 position = Position();

        float howManyWidthsFromCamera = (position.x - cameraPosition.x) / mapWidth;

        if (Mathf.Abs(howManyWidthsFromCamera) <= 0.5f)
        {
            return position;
        }

        if (howManyWidthsFromCamera > 0)
            howManyWidthsFromCamera += 0.5f;
        else
            howManyWidthsFromCamera += -0.5f;

        int howManyWidthsToFix = (int)howManyWidthsFromCamera;

        position.x -= howManyWidthsToFix * mapWidth;

        return position;
    }
    
    public static int HexDistance(Hex a, Hex b)
    {
        return (Mathf.Abs(a.Q - b.Q) + Mathf.Abs(a.R - b.R) + Mathf.Abs(a.S - b.S))/2;
    }

    //index 0 = neighbor to right, index moves counterclockwise
    public void SetNeighbor(Hex neighbor, Direction direction)
    {
        neighbors[(int)direction] = neighbor;
    }

    public Hex GetNeighbor(Direction direction)
    {
        if (neighbors[(int)direction] != null)
            return neighbors[(int)direction];
        else
            return null;
    }

    public Hex[] GetNeighbors()
    {
        return neighbors;
    }

    public void SetTerrain(Terrain terrain)
    {
        this.terrain = terrain;
    }

    public Terrain GetTerrain()
    {
        return terrain;
    }

    public static float Distance(Hex a, Hex b)
    {
        HexMap hexMap = a.hexMap;

        int dQ = Mathf.Abs(a.Q - b.Q);
        if (dQ > hexMap.NumColumns() / 2)
            dQ = hexMap.NumColumns() - dQ;

        return Mathf.Max(dQ,
                        Mathf.Abs(a.R - b.R),
                        Mathf.Abs(a.S - b.S));
    }

    public void AddPawn(Pawn pawn)
    {
        if (pawns == null)
            pawns = new HashSet<Pawn>();

        pawns.Add(pawn);
    }
    
    public void RemovePawn(Pawn pawn)
    {
        if (pawns != null)
            pawns.Remove(pawn);
    }
}
