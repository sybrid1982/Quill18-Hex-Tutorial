using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnKeeper : MonoBehaviour {
    [SerializeField]
    GameObject PawnPrefab;

    Dictionary<Pawn, GameObject> pawnToPawnGOMap;
    Dictionary<string, Pawn> stringToPrototypeMap;

    HexMap hexMap;
    GameManager gameManager;

    public void StartPawnkeeper()
    {
        hexMap = FindObjectOfType<HexMap>();
        gameManager = FindObjectOfType<GameManager>();
        InitializeDictionaries();
        GeneratePawnPrototypes();
    }

    void InitializeDictionaries()
    {
        pawnToPawnGOMap = new Dictionary<Pawn, GameObject>();
        stringToPrototypeMap = new Dictionary<string, Pawn>();
    }

    void GeneratePawnPrototypes()
    {
        Pawn basic = new Pawn("Basic", 4);
        stringToPrototypeMap.Add("Basic", basic);
    }

    public void GenerateStartPawn(Player player)
    {
        Debug.Log("GenerateStartPawn called");
        Hex spawnHex = hexMap.GetAcceptableStartPosition();

        GameManager gameManager = FindObjectOfType<GameManager>();

        Pawn p = new Pawn(stringToPrototypeMap["Basic"], spawnHex, player);
        player.AddPawn(p);

        GameObject pawnGO = (GameObject)Instantiate(PawnPrefab,
                spawnHex.PositionFromCamera(Camera.main.transform.position, hexMap.NumRows(), hexMap.NumColumns()),
                Quaternion.identity,
                gameManager.GetWorldDisplay().GetHexGOFromHex(spawnHex).transform);

        pawnGO.GetComponent<PawnComponent>().pawn = p;
        pawnToPawnGOMap.Add(p, pawnGO);
        
    }

    public GameObject GetPawnGOFromPawn(Pawn pawn)
    {
        if (pawnToPawnGOMap.ContainsKey(pawn))
            return pawnToPawnGOMap[pawn];
        else
        {
            Debug.LogError("Asked for pawn that does not exist in map");
            return null;
        }
    }

}
