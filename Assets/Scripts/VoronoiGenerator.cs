using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class VoronoiGenerator
{
    public static void BuildCells(List<VoronoiPoint> points, Vector3 cubeMin, Vector3 cubeMax)
    {
        foreach (var p in points)
        {
            p.cellPlanes.Clear();

            // Bounding box
            p.cellPlanes.Add(new MyPlane(Vector3.right, new Vector3(cubeMin.x, 0, 0)));
            p.cellPlanes.Add(new MyPlane(-Vector3.right, new Vector3(cubeMax.x, 0, 0)));
            p.cellPlanes.Add(new MyPlane(Vector3.up, new Vector3(0, cubeMin.y, 0)));
            p.cellPlanes.Add(new MyPlane(-Vector3.up, new Vector3(0, cubeMax.y, 0)));
            p.cellPlanes.Add(new MyPlane(Vector3.forward, new Vector3(0, 0, cubeMin.z)));
            p.cellPlanes.Add(new MyPlane(-Vector3.forward, new Vector3(0, 0, cubeMax.z)));

            foreach (var q in points)
            {
                if (q == p) continue;

                Vector3 dir = q.position - p.position;
                float dist = dir.magnitude;
                if (dist < 1e-5f) continue;

                Vector3 normal = dir / dist;
                Vector3 midpoint = (p.position + q.position) * 0.5f;

                MyPlane plane = new MyPlane(normal, midpoint);

                // Asegurar que p quede del lado negativo
                if (plane.GetDistanceToPoint(p.position) > 0f)
                    plane.Flip();

                // CLAVE: solo agregar si RECORTA la celda
                if (!IsPlaneRedundant(plane, p, points))
                {
                    p.cellPlanes.Add(plane);
                }
            }
        }
    }

    private static bool IsPlaneRedundant(MyPlane candidate, VoronoiPoint owner, List<VoronoiPoint> allPoints)
    {
        // Si ningún otro punto válido queda del lado incorrecto,
        // el plano no aporta frontera
        foreach (var other in allPoints)
        {
            if (other == owner) continue;

            // Si el otro punto ya está descartado por planos previos, ignorar
            bool inside = true;
            foreach (var plane in owner.cellPlanes)
            {
                if (plane.GetSide(other.position) != plane.GetSide(owner.position))
                {
                    inside = false;
                    break;
                }
            }

            if (!inside)
                continue;

            // Si este plano separa a otro punto válido recorta
            if (candidate.GetSide(other.position) != candidate.GetSide(owner.position))
                return false;
        }

        return true; // no recorta nada
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
