using System.Collections.Generic;
using UnityEngine;
using CustomMath;
//El ultimo que me delimito una frontera, si hasta maximo esa distancia por raiz de 2 no se me delimito una nueva frontera
// dejo de intentar generar nuevas porque ya es seguro que no se me van a generar nuevas delimitaciones
public class VoronoiGenerator
{
    public static void BuildCells(List<VoronoiPoint> points, Vector3 cubeMin, Vector3 cubeMax)
    {
        foreach (var point in points)
        {
            point.cellPlanes.Clear();

            //Container cube planes
            point.cellPlanes.Add(new MyPlane(Vector3.right, new Vector3(cubeMin.x, 0, 0)));
            point.cellPlanes.Add(new MyPlane(-Vector3.right, new Vector3(cubeMax.x, 0, 0)));
            point.cellPlanes.Add(new MyPlane(Vector3.up, new Vector3(0, cubeMin.y, 0)));
            point.cellPlanes.Add(new MyPlane(-Vector3.up, new Vector3(0, cubeMax.y, 0)));
            point.cellPlanes.Add(new MyPlane(Vector3.forward, new Vector3(0, 0, cubeMin.z)));
            point.cellPlanes.Add(new MyPlane(-Vector3.forward, new Vector3(0, 0, cubeMax.z)));

            //Order points by distance
            List<VoronoiPoint> ordered = new List<VoronoiPoint>(points);
            ordered.Remove(point);

            ordered.Sort((a, b) =>
                (a.position - point.position).sqrMagnitude
                .CompareTo((b.position - point.position).sqrMagnitude)
            );

            float lastEffectiveDistance = 0f;

            foreach (var other in ordered)
            {
                float dist = Vector3.Distance(point.position, other.position);

                //if (lastEffectiveDistance > 0f && dist > lastEffectiveDistance * Mathf.Sqrt(2f))
                //    break;

                //Bisector plane
                Vector3 midPoint = (point.position + other.position) * 0.5f;
                Vector3 normal = (other.position - point.position).normalized;
                MyPlane bisector = new MyPlane(normal, midPoint);

                if (bisector.GetSide(point.position))
                    bisector.Flip();

                if (PlaneCutsCell(point, bisector))
                {
                    point.cellPlanes.Add(bisector);
                    lastEffectiveDistance = dist;
                }

            }
        }
    }

    public static bool PlaneCutsCell(VoronoiPoint point, MyPlane bisector)
    {
        bool divides = false;
        foreach (var other in point.cellPlanes)
        {
            if (!other.SameSide(point.position, bisector.Point))
            {
                divides = true;
                break;
            }
        }

        return divides;
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
