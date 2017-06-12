using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class HexMapTests : IPrebuildSetup {
    
    public void Setup()
    {
        
    }

    //Test that hexmap is not null (test to see if tests are working)
    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexMapExists()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Assert.AreNotEqual(null, hexMap);
    }

    //Test that a hex exists at 5,5 (should generate 20x40 map)
	[Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexExistsAtQ5R5()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(5, 5);
        Assert.AreNotEqual(null, hex);
    }
	
    //The next six tests take a hex in the middle of the map and check that it has neighbors
    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtQ5R5HasRightNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(5, 5);
        Hex neighborRight = hex.GetNeighbor(Direction.RIGHT);
        Assert.AreNotEqual(null, neighborRight);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtQ5R5HasUpperRightNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(5, 5);
        Hex neighbor = hex.GetNeighbor(Direction.UPPER_RIGHT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtQ5R5HasUpperLeftNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(5, 5);
        Hex neighbor = hex.GetNeighbor(Direction.UPPER_LEFT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtQ5R5HasLeftNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(5, 5);
        Hex neighbor = hex.GetNeighbor(Direction.LEFT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtQ5R5HasLowerLeftNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(5, 5);
        Hex neighbor = hex.GetNeighbor(Direction.LOWER_LEFT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtQ5R5HasLowerRightNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(5, 5);
        Hex neighbor = hex.GetNeighbor(Direction.LOWER_RIGHT);
        Assert.AreNotEqual(null, neighbor);
    }

    //The next six tests take a hex on the left edge of the map and check that it has neighbors
    //The left neighbors to this hex are the right most hexes
    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtq0r5HasRightNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(0, 5);
        Hex neighborRight = hex.GetNeighbor(Direction.RIGHT);
        Assert.AreNotEqual(null, neighborRight);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtq0r5HasUpperRightNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(0, 5);
        Hex neighbor = hex.GetNeighbor(Direction.UPPER_RIGHT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtq0r5HasUpperLeftNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(0, 5);
        Hex neighbor = hex.GetNeighbor(Direction.UPPER_LEFT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtq0r5HasLeftNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(0, 5);
        Hex neighbor = hex.GetNeighbor(Direction.LEFT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtq0r5HasLowerLeftNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(0, 5);
        Hex neighbor = hex.GetNeighbor(Direction.LOWER_LEFT);
        Assert.AreNotEqual(null, neighbor);
    }

    [Test]
    [PrebuildSetup(typeof(HexMapTests))]
    public void AssertHexAtq0r5HasLowerRightNeighbor()
    {
        HexMap hexMap = ScriptableObject.CreateInstance<HexMap>();
        hexMap.GenerateMap();
        Hex hex = hexMap.GetHexAt(0, 5);
        Hex neighbor = hex.GetNeighbor(Direction.LOWER_RIGHT);
        Assert.AreNotEqual(null, neighbor);
    }
}
