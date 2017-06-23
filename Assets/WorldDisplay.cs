using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDisplay : MonoBehaviour
{
    Dictionary<Hex, GameObject> hexToHexGOMap;
    Dictionary<TerrainType, HexVisuals> terrainToVisualsMap;

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
        CreateHexVisuals();
        hexToHexGOMap = new Dictionary<Hex, GameObject>();
        for(int q = 0; q < hexMap.NumColumns(); q++)
        {
            GenerateHexGOsForColumn(hexMap, q);
        }
    }

    private void GenerateHexGOsForColumn(HexMap hexMap, int q)
    {
        for (int r = 0; r < hexMap.NumRows(); r++)
        {
            GenerateHexGO(hexMap.GetHexAt(q, r));
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

        SetVisualsForHex(h.GetTerrainType(), mr, mf);
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

    void CreateHexVisuals()
    {
        /* To avoid needing the switch statement, we need
         * to instead have a dictionary that links
         * terraintypes to objects that store the visual
         * information associated with those terrain types */

        //Initialize map
        terrainToVisualsMap = new Dictionary<TerrainType, HexVisuals>();

        //Visuals for mountain
        CreateHexVisual(TerrainType.MOUNTAIN, MeshMountain, MatMountains);
        //Visuals for Arid Hills
        CreateHexVisual(TerrainType.ARID_HILLS, MeshHill, MatDesert);
        //Visuals for Grassy Hills
        CreateHexVisual(TerrainType.GRASSY_HILLS, MeshHill, MatGrasslands);
        //Visuals for Desert
        CreateHexVisual(TerrainType.DESERT, MeshFlat, MatDesert);
        //Visuals for Grasslands
        CreateHexVisual(TerrainType.GRASS, MeshFlat, MatGrasslands);
        //Visuals for Water
        CreateHexVisual(TerrainType.WATER, MeshWater, MatOcean);
    }

    void CreateHexVisual(TerrainType ttype, Mesh mesh, Material material)
    {
        HexVisuals hexVisual = new HexVisuals(mesh, material);
        terrainToVisualsMap.Add(ttype, hexVisual);
    }

    HexVisuals GetVisualsForTerrain(TerrainType ttype)
    {
        if (terrainToVisualsMap.ContainsKey(ttype))
        {
            return terrainToVisualsMap[ttype];
        } else
        {
            /* We've asked for a terrain we didn't prototype
             * Return a gray flat hex which should hopefully
             * be visually obvious to be wrong */
            HexVisuals error = new HexVisuals(MeshFlat, MatMountains);
            return error;
        }

    }

    void SetVisualsForHex(TerrainType ttype, MeshRenderer mr, MeshFilter mf)
    {
        HexVisuals terrainVisuals = GetVisualsForTerrain(ttype);
        mr.material = terrainVisuals.GetMaterial();
        mf.mesh = terrainVisuals.GetMesh();
    }
}