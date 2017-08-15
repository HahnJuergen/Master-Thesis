using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMesh
{
    Mesh GenerateMesh(MeshType mt, float width, float height);
    GameObject MeshToGameObject(Mesh m, MeshType mt, string shaderName, Color color, string layerName, string name, Transform parent);
    GameObject GenerateGameObjectFromPrefabBetweenTwoPoints(Vector3 p, Vector3 q, string name, string prefabPath, string layer, GameObject target, Transform parent, MeshType mt);
}

public enum MeshType
{
    Plane,
    Cylinder
}
