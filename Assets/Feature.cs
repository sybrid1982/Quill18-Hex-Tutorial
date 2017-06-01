using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FeatureTypes
{
    FOREST
}

public class Feature {

    int movementCost;
    string name;
    FeatureTypes myFeatureType;

    Hex myHex;

    public FeatureTypes FeatureType
    {
        get
        {
            return myFeatureType;
        }

        set
        {
            myFeatureType = value;
        }
    }

    //used to prototype features
    public Feature(string name, int movementCost, FeatureTypes featureType)
    {
        this.movementCost = movementCost;
        this.name = name;
        this.myFeatureType = featureType;
    }

    //used to create the features themselves
    public Feature(Feature featureProto, Hex hex)
    {
        this.movementCost = featureProto.movementCost;
        this.name = featureProto.name;
        this.myFeatureType = featureProto.myFeatureType;
        this.myHex = hex;
    }

    
}
