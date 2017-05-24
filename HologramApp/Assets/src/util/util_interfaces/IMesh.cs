using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMesh
{
    Mesh GenerateMesh(MeshType mt, float width, float height);
    GameObject MeshToGameObject(Mesh m, MeshType mt, string shaderName, Color color, string layerName, string name, Transform parent);
}

public enum MeshType
{
    Plane
}
