using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDisplay : MonoBehaviour
{
    Dictionary<Hex, GameObject> hexToHexGOMap;

    public GameObject hexPrefab;

    public Material MatGrasslands;
    public Material MatPlains;
    public Material MatDesert;
    public Material MatOcean;
    public Material MatMountains;
    public Material MatUnrevealed;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public void Initialize(HexMap hexMap)
    {
        hexToHexGOMap = new Dictionary<Hex, GameObject>();
        for(int q = 0; q < hexMap.NumColumns(); q++)
        {
            for (int r = 0; r < hexMap.NumRows(); r++)
            {
                GenerateHexGO(hexMap.GetHexAt(q, r));
            }
        }
    }

    public void DisplayMapForPlayer(Player player)
    {
        TurnAllTilesBlack();
        foreach (Hex visibleHex in player.GetVisibleHexes())
        {
            UpdateHexVisuals(visibleHex);
        }
        foreach (Hex revealedHex in player.GetRevealedButNotVisibleHexes())
        {
            UpdateHexVisuals(revealedHex);
        }
    }

    private void GenerateHexGO(Hex h)
    {
        GameObject hexGO = (GameObject)Instantiate(hexPrefab,
            h.PositionFromCamera(Camera.main.transform.position),
            Quaternion.identity,
            this.transform
            );
        hexGO.GetComponent<HexComponent>().hex = h;
        hexGO.GetComponent<HexComponent>().hexMap = h.hexMap;
        hexToHexGOMap.Add(h, hexGO);
        hexGO.name = ("Hex: " + h.Q + ", " + h.R);
    }

    private void TurnAllTilesBlack()
    {
        foreach (var pair in hexToHexGOMap)
        {
            GameObject hexGO = pair.Value;
            MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
            MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

            mr.material = MatUnrevealed;
            mf.mesh = MeshFlat;
        }
    }

    public void UpdateHexVisuals(Hex h, bool seeHexEntirely = true)
    {
        if (h == null)
        {
            Debug.LogError("Null hex passed to visual updater");
            return;
        }

        GameObject hexGO = hexToHexGOMap[h];

        MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
        MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

        switch (h.GetTerrainType())
        {
            case TerrainType.MOUNTAIN:
                mr.material = MatMountains;
                mf.mesh = MeshMountain;
                break;
            case TerrainType.ARID_HILLS:
                mr.material = MatDesert;
                mf.mesh = MeshHill;
                break;
            case TerrainType.GRASSY_HILLS:
                mr.material = MatGrasslands;
                mf.mesh = MeshHill;
                break;
            case TerrainType.GRASS:
                mr.material = MatGrasslands;
                mf.mesh = MeshFlat;
                break;
            case TerrainType.DESERT:
                mr.material = MatDesert;
                mf.mesh = MeshFlat;
                break;
            case TerrainType.WATER:
                mr.material = MatOcean;
                mf.mesh = MeshWater;
                break;
            default:
                Debug.LogError("Unknown terrain " + h.GetTerrainType().ToString() + " sent to WorldDisplay");
                break;
        }
    }

    public GameObject GetHexGOFromHex(Hex hex)
    {
        if (hexToHexGOMap.ContainsKey(hex))
            return hexToHexGOMap[hex];
        else
        {
            Debug.Log("Asked DisplayWorld for hex not in its dictionary");
            return null;
        }
    }
}