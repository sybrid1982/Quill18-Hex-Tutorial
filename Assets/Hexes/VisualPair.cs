using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexVisuals {
    Mesh mesh;
    Material material;

    public HexVisuals(Mesh mesh, Material material)
    {
        this.mesh = mesh;
        this.material = material;
    }

    public Mesh GetMesh()
    {
        return mesh;
    }
    public Material GetMaterial()
    {
        return material;
    }
}
