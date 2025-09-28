using UnityEngine;
using System.Collections.Generic;
using CustomMath;

[System.Serializable]
public class VoronoiPoint
{
    public Vector3 position;
    public List<MyPlane> cellPlanes = new List<MyPlane>();

    public List<VoronoiPoint> sortedPoints = new List<VoronoiPoint>();

    public VoronoiPoint(Vector3 pos)
    {
        position = pos;
    }

    public void SortNeighbors(List<VoronoiPoint> allPoints)
    {
        sortedPoints.Clear();
        foreach (var other in allPoints)
        {
            if (other == this)
                continue;
            sortedPoints.Add(other);
        }

        sortedPoints.Sort((a, b) =>
            (a.position - position).sqrMagnitude.CompareTo((b.position - position).sqrMagnitude)
        );
    }
}
