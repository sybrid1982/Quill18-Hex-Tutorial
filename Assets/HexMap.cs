using System.Collections;
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

        GenerateMap();
    }

    public int NumRows()
    {
        return numRows;
    }

    public int NumColumns()
    {
        return numColumns;
    }

    public void GenerateMap()
    {
        for (int column = 0; column < numColumns; column++)
        {
            //add a new column
            hexMapAxial.Add( new List<Hex>() );
            for (int row = 0; row < numRows; row++)
            {
                Hex h = new Hex(this, column, row);

                GameObject hexGO = (GameObject)Instantiate(HexPrefab,
                    h.PositionFromCamera(Camera.main.transform.position, numRows, numColumns), 
                    Quaternion.identity, 
                    this.transform
                    );

                hexGO.GetComponent<HexComponent>().hex = h;
                hexGO.GetComponent<HexComponent>().hexMap = this;
                hexGO.name = ("Hex: " + column + ", " + row);

                MeshRenderer meshRenderer = hexGO.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material = hexMaterials[Random.Range(0, hexMaterials.Length)];

                //add to the new column a new row member
                hexMapAxial[column].Add(h);
                //and to the dictionary
                hexToHexGOMap.Add(h, hexGO);
            }
        }
        GeneratePawns();
        GenerateStartPawn();
    }

    void GeneratePawns()
    {
        Pawn basic = new Pawn("Basic", 4);
        stringToPrototypeMap.Add("Basic", basic);
    }

    void GenerateStartPawn()
    {
        Hex spawnHex = GetRandomHexFromHexMap();

            Pawn p = new Pawn(stringToPrototypeMap["Basic"], spawnHex);

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



    void Update()
    {

    }

}
