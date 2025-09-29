using System.Collections.Generic;
using UnityEngine;
using CustomMath;

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

            // Adds the bisector planes generated with the other points
            //List<VoronoiPoint> ordered = new List<VoronoiPoint>(points);
            //ordered.Remove(voronoiPoint);
            //ordered.Sort((a, b) =>
            //{
            //    float pointA = (a.position - voronoiPoint.position).sqrMagnitude;
            //    float pointB = (b.position - voronoiPoint.position).sqrMagnitude;
            //    return pointA.CompareTo(pointB);
            //});
            //
            //float lastDist = 0f;
            foreach (var other in points)
            {
                //float dist = (other.position - voronoiPoint.position).magnitude;
                //if (lastDist > 0f && dist > lastDist * Mathf.Sqrt(2f))
                //    break;

                // Bisector
                Vector3 midPoint = (voronoiPoint.position + other.position) * 0.5f;
                Vector3 normal = (other.position - voronoiPoint.position).normalized;

                MyPlane bisector = new MyPlane(normal, midPoint);

                if (bisector.GetSide(voronoiPoint.position))
                    bisector.Flip();

                voronoiPoint.cellPlanes.Add(bisector);

                //lastDist = dist;
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
}