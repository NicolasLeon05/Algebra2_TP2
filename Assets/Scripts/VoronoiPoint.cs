using UnityEngine;
using System.Collections.Generic;
using CustomMath; // para MyPlane

[System.Serializable]
public class VoronoiPoint
{
    public Vector3 position;
    public List<MyPlane> cellPlanes = new List<MyPlane>();

    public VoronoiPoint(Vector3 pos)
    {
        position = pos;
    }
}