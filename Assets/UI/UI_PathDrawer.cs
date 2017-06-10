using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PathDrawer : MonoBehaviour {
    [SerializeField]
    Sprite pathSprite;
    [SerializeField]
    Sprite targetSprite;

    Pawn mySelectedPawn;

    float upwardAdjustOfMarkers = 0.4f;
    float downwardAdjustOfMarkers = -1.8f;

    List<GameObject> pathMarkers;

	// Use this for initialization
	void Start () {
        pathMarkers = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClearPath()
    {
        while (pathMarkers.Count > 0)
        {
            GameObject deadman = pathMarkers[0];
            pathMarkers.Remove(pathMarkers[0]);
            Destroy(deadman);
        }
    }

    public void DisplayPathForPawn(Pawn selectedPawn, bool wantTempPath = false)
    {
        mySelectedPawn = selectedPawn;
        //Debug.Log("Asked to draw a path");
        //First step:  If we're already drawing a path, get rid of it
        ClearPath();

        //basic idea:
        //Get the pawn's hexpath, if it has one
        //(if it doesn't, we're already done!)
        //find out all the hexes in it, and if it's
        //not the last hex in the stack, put a circle
        //sprite centered on that hex
        //otherwise put a target sprite centered on that
        //hex
        Stack<Hex> hexStack = selectedPawn.CopyMovementHexStack(wantTempPath);

        //if the stack is empty, don't do anything
        if (hexStack.Count == 0)
        {
            //Debug.Log("Stack was empty");
            return;
        }


         while(hexStack.Count > 1)
        {
            //Get the next hex that isn't the end of the list
            Hex hexToMark = hexStack.Pop();
            //Spawn a GO on it and add it to the markers list
            CreateMarkerAtHex(hexToMark, pathSprite);

        }
        //get the last hex in the stack
        Hex lastHexToMark = hexStack.Pop();
        //Spawn a GO on it and add it to the markers list
        CreateMarkerAtHex(lastHexToMark, targetSprite);
    }

    private void CreateMarkerAtHex(Hex hexToMark, Sprite spriteToMark)
    {
        GameObject newMarker = new GameObject();
        Vector3 markerPosition = hexToMark.PositionFromCamera(Camera.main.transform.position);
        markerPosition.y += downwardAdjustOfMarkers;
        //need to adjust upward for hills
        if (hexToMark.GetTerrain() == Terrain.ARID_HILLS || hexToMark.GetTerrain() == Terrain.GRASSY_HILLS)
        {
            markerPosition.y += upwardAdjustOfMarkers;
        }
        newMarker.transform.position = markerPosition;
        //newMarker.transform.SetParent()
        pathMarkers.Add(newMarker);
        //Add a spriteRenderer component to the gameObject
        SpriteRenderer sr = newMarker.AddComponent<SpriteRenderer>();
        //Set the sprite on the renderer to the path sprite
        sr.sprite = spriteToMark;

        //Rotate the marker so it is right-way up
        newMarker.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
