using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap_Continents : HexMap {
    public override void GenerateMap()
    {
        base.GenerateMap();

        int numContinents = 3;
        int continentSpacing = NumColumns() / numContinents;

        CreateBasicContinentSplats(numContinents, continentSpacing);

        //add lumpiness Perlin Noise?
        float noiseResolution = 0.01f;
        float noiseScale = 2f;
        AddNoiseToTerrainHeights(noiseResolution, noiseScale);

        //simulate rainfall/moisture (probably just Perlin for now) and set plains/grasslands + forest
        SimulateRainfall();

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

    private void SimulateRainfall()
    {
        float noiseResolution = 0.05f;
        float noiseScale = 1.5f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        for (int column = 0; column < NumColumns(); column++)
        {
            for (int row = 0; row < NumRows(); row++)
            {
                Hex h = GetHexAt(column, row);
                float n = Mathf.PerlinNoise((float)column / Mathf.Max(NumColumns(), NumRows()) / noiseResolution + noiseOffset.x,
                    (float)row / Mathf.Max(NumColumns(), NumRows()) / noiseResolution + noiseOffset.y
                    - 0.5f);
                h.Moisture = n * noiseScale;

            }
        }
    }

    private void AddNoiseToTerrainHeights(float noiseResolution, float noiseScale)
    {
        Vector2 noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        for (int column = 0; column < NumColumns(); column++)
        {
            for (int row = 0; row < NumRows(); row++)
            {
                Hex h = GetHexAt(column, row);
                float n = Mathf.PerlinNoise(((float)column / Mathf.Max(NumColumns(), NumRows()) / noiseResolution) + noiseOffset.x,
                    ((float)row / Mathf.Max(NumColumns(), NumRows()) / noiseResolution) + noiseOffset.y)
                    - 0.5f;
                h.Elevation += n * noiseScale;

            }
        }
    }

    void ElevateArea(int q, int r, int radius, float centerHeight = 0.8f)
    {
        Hex centerHex = GetHexAt(q, r);

        Hex[] areaHexes = GetHexesWithRadiusOf(centerHex, radius);

        foreach (Hex h in areaHexes)
        {
            h.Elevation = centerHeight * Mathf.Lerp(1f,
                0.25f,
                Mathf.Pow(Hex.Distance(centerHex, h) / radius, 2f));
        }

    }

    private void CreateBasicContinentSplats(int numContinents, int continentSpacing)
    {
        for (int c = 0; c < numContinents; c++)
        {
            int numSplats = Random.Range(4, 8);
            for (int i = 0; i < numSplats; i++)
            {
                int range = Random.Range(5, 8);
                int y = Random.Range(range, NumRows() - range);
                int x = Random.Range(0, 10) - y / 2 + (c * continentSpacing);

                ElevateArea(x, y, range);
            }
        }
    }
}
