﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour {

    [SerializeField]
    GameObject HexPrefab;
    [SerializeField]
    GameObject PawnPrefab;

    [SerializeField]
    int numRows = 20;
    [SerializeField]
    int numColumns = 40;

    //TODO: Create a custom inspector so that the material slots are labeled
    //in unity with the actual terrain types instead of "element0" for
    //usability
    [SerializeField]
    Material[] hexMaterials;

    List<List<Hex>> hexMapAxial;

    Dictionary<Hex, GameObject> hexToHexGOMap;
    Dictionary<Pawn, GameObject> pawnToPawnGOMap;

    Dictionary<string, Pawn> stringToPrototypeMap;

    public HexGraph hexGraph;
    
	// Use this for initialization
	void Start () {
        hexMapAxial = new List<List<Hex>>();
        hexToHexGOMap = new Dictionary<Hex, GameObject>();
        pawnToPawnGOMap = new Dictionary<Pawn, GameObject>();
        stringToPrototypeMap = new Dictionary<string, Pawn>();
    }

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
        GenerateMap();
    }

    public void GenerateMap()
    {
        for (int column = 0; column < numColumns; column++)
        {
            //add a new column
            hexMapAxial.Add(new List<Hex>());
            GenerateColumn(column);
        }
        GeneratePawnPrototypes();
        GenerateStartPawn();
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
        Hex h;
        GameObject hexGO;
        GenerateHexGO(column, row, out h, out hexGO);
        SetTerrainOnHex(hexGO);

        //add to the new column a new row member
        hexMapAxial[column].Add(h);
        //and to the dictionary
        hexToHexGOMap.Add(h, hexGO);
        //Generate Neighbors for the tile
        GenerateNeighbors(h);
    }

    private void SetTerrainOnHex(GameObject hexGO)
    {
        MeshRenderer meshRenderer = hexGO.GetComponentInChildren<MeshRenderer>();
        int terrainIndex = Random.Range(0, hexMaterials.Length);
        meshRenderer.material = hexMaterials[terrainIndex];
        hexGO.GetComponent<HexComponent>().hex.SetTerrain((Terrain)terrainIndex);
    }

    private void GenerateHexGO(int column, int row, out Hex h, out GameObject hexGO)
    {
        h = new Hex(this, column, row);
        hexGO = (GameObject)Instantiate(HexPrefab,
            h.PositionFromCamera(Camera.main.transform.position, numRows, numColumns),
            Quaternion.identity,
            this.transform
            );
        hexGO.GetComponent<HexComponent>().hex = h;
        hexGO.GetComponent<HexComponent>().hexMap = this;
        hexGO.name = ("Hex: " + column + ", " + row);
    }

    void GenerateNeighbor(Hex h, Vector2 neighborToCheck, int directionIndex)
    {
        Hex hexToCheck = GetHexFromHexMap(neighborToCheck);
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
        Vector2[] potentialNeighborCoords = {   new Vector2 (h.Q, h.R - 1),          //left
                                                new Vector2 (h.Q - 1, h.R),         //lower left
                                                new Vector2 (h.Q - 1, h.R + 1),};    //lower right
        for (int i = 0; i < 3; i++)
        {
            GenerateNeighbor(h, potentialNeighborCoords[i], i+3);
        }
        //HOWEVER, if this is the last tile in a row, there are two
        //other neighbors to consider to wrap the map
        if(h.R == numColumns - 1)
        {
            GenerateWrapNeighbors(h);
        }
    }

    private void GenerateWrapNeighbors(Hex h)
    {
        Hex rightH = GetHexFromHexMap(0, h.R);
        if (rightH != null)
        {
            h.SetNeighbor(rightH, Direction.RIGHT);
            rightH.SetNeighbor(h, Direction.LEFT);
        }
        Hex lowerRightH = GetHexFromHexMap(0, h.R - 1);
        if (lowerRightH != null)
        {
            h.SetNeighbor(lowerRightH, Direction.LOWER_RIGHT);
            lowerRightH.SetNeighbor(h, Direction.UPPER_LEFT);
        }
    }

    void GeneratePawnPrototypes()
    {
        Pawn basic = new Pawn("Basic", 4);
        stringToPrototypeMap.Add("Basic", basic);
    }

    void GenerateStartPawn()
    {
        Hex spawnHex = GetRandomHexFromHexMap();

        GameManager gameManager = FindObjectOfType<GameManager>();

        Pawn p = new Pawn(stringToPrototypeMap["Basic"], spawnHex, gameManager.GetActivePlayer());
        gameManager.GetActivePlayer().AddPawn(p);

        GameObject pawnGO = (GameObject)Instantiate(PawnPrefab,
                spawnHex.PositionFromCamera(Camera.main.transform.position, numRows, numColumns),
                Quaternion.identity,
                hexToHexGOMap[spawnHex].transform);

        pawnGO.GetComponent<PawnComponent>().pawn = p;
        pawnToPawnGOMap.Add(p, pawnGO);
        
    }

    //Returns the hex at coordinates Q, R
    //If Q,R is out of the map bounds, returns null
    public Hex GetHexFromHexMap(int Q, int R)
    {
        if (Q >= 0 && R >= 0 && Q < numColumns && R < numRows)
            return hexMapAxial[Q][R];
        else
            return null;
    }

    public Hex GetHexFromHexMap(Vector2 coords)
    {
        if (coords.x >= 0 && coords.y >= 0 && coords.x < numColumns && coords.y < numRows)
        {
            return hexMapAxial[(int)coords.x][(int)coords.y];
        }
        else
            return null;
    }

    Hex GetRandomHexFromHexMap()
    {
        int q = Random.Range(0, numColumns);
        int r = Random.Range(0, numRows);

        return GetHexFromHexMap(q, r);
    }

    public GameObject GetHexGOFromHex(Hex hex)
    {
        GameObject hexGO = hexToHexGOMap[hex];
        return hexGO;
    }
    
    public GameObject GetPawnGOFromPawn(Pawn pawn)
    {
        if (pawnToPawnGOMap.ContainsKey(pawn))
            return pawnToPawnGOMap[pawn];
        else
            return null;
    }
}
