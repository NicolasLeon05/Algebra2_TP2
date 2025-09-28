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
            float lastDist = 0f;
            foreach (var other in points)
            {
                if (other == voronoiPoint) continue;

                //float dist = (other.position - voronoiPoint.position).magnitude;
                //
                //if (lastDist * Mathf.Sqrt(2f) < dist && lastDist > 0f)
                //    break;

                // Bisector
                Vector3 midPoint = (voronoiPoint.position + other.position) * 0.5f;
                Vector3 normal = (other.position - voronoiPoint.position).normalized;

                MyPlane bisector = new MyPlane(normal, midPoint);

                // The plane normal points to the point
                if (bisector.GetSide(voronoiPoint.position)) // Si p está del lado de la normal
                {
                    bisector.Flip(); // Invertimos para que mire hacia p
                }

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