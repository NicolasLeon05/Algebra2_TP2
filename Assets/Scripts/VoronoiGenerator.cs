using CustomMath;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class VoronoiGenerator
{
    public static void BuildCells(List<VoronoiPoint> points, Vector3 cubeMin, Vector3 cubeMax)
    {
        foreach (var voronoiPoint in points)
        {
            voronoiPoint.cellPlanes.Clear();

            // Adds the limits of the cointainer cube as planes
            voronoiPoint.cellPlanes.Add(new MyPlane(Vector3.right, new Vector3(cubeMin.x, 0, 0)));
            voronoiPoint.cellPlanes.Add(new MyPlane(-Vector3.right, new Vector3(cubeMax.x, 0, 0)));
            voronoiPoint.cellPlanes.Add(new MyPlane(Vector3.up, new Vector3(0, cubeMin.y, 0)));
            voronoiPoint.cellPlanes.Add(new MyPlane(-Vector3.up, new Vector3(0, cubeMax.y, 0)));
            voronoiPoint.cellPlanes.Add(new MyPlane(Vector3.forward, new Vector3(0, 0, cubeMin.z)));
            voronoiPoint.cellPlanes.Add(new MyPlane(-Vector3.forward, new Vector3(0, 0, cubeMax.z)));


            float lastDistance = 0.0f;
            // Adds the bisector planes generated with the other points
            foreach (var other in points)
            {


                // Bisector
                Vector3 midPoint = (voronoiPoint.position + other.position) * 0.5f;
                Vector3 normal = (other.position - voronoiPoint.position).normalized;

                MyPlane bisector = new MyPlane(normal, midPoint);
                float dist = Vector3.Distance(voronoiPoint.position, bisector.ClosestPointOnPlane(voronoiPoint.position));

                if (lastDistance != 0 && dist > lastDistance * Mathf.Sqrt(2f))
                    break;

                if (bisector.GetSide(voronoiPoint.position))
                    bisector.Flip();

                voronoiPoint.cellPlanes.Add(bisector);
                lastDistance = dist;
            }
        }

    }

    public static bool IsInsideCell(VoronoiPoint point, Vector3 target)
    {
        foreach (var plane in point.cellPlanes)
        {
            if (plane.GetSide(target) != plane.GetSide(point.position))
                return false;
        }
        return true;
    }

    public static void DebugCell(VoronoiPoint point)
    {
        Debug.Log($"--- Voronoi Cell Debug ---");
        Debug.Log($"Point position: {point.position}");
        Debug.Log($"Plane count: {point.cellPlanes.Count}");

        for (int i = 0; i < point.cellPlanes.Count; i++)
        {
            MyPlane p = point.cellPlanes[i];
            Debug.Log($"Plane {i}: normal={p.normal}, d={p.distance}, point={p.Point}");
        }
    }
}