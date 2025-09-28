using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class PointGrid : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private Vector3 cubeSize = new Vector3(5f, 1f, 5f);

    [Header("Points")]
    [SerializeField] private int pointCount = 20;
    [SerializeField] private float pointRadius = 0.1f;
    [SerializeField] private Color pointColor = Color.white;
    [SerializeField] private Color highlightColor = Color.red;

    [Header("Target to check")]
    [SerializeField] private Transform target;

    [Header("Debug Options")]
    [SerializeField] private int selectedPointIndex = -1;
    [SerializeField] private bool regenerateOnValidate = true;

    public List<VoronoiPoint> points = new List<VoronoiPoint>();

    private void OnValidate()
    {
        if (regenerateOnValidate)
            GeneratePoints();
    }

    private void OnDrawGizmos()
    {
        if (points == null || points.Count == 0)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + cubeSize * 0.5f, cubeSize);

        for (int i = 0; i < points.Count; i++)
        {
            bool inside = false;

            if (target != null)
                inside = VoronoiGenerator.IsInsideCell(points[i], target.position);

            Gizmos.color = inside ? highlightColor : pointColor;
            Gizmos.DrawSphere(points[i].position, pointRadius);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(points[i].position + Vector3.up * 0.5f, i.ToString());
#endif
        }


        if (selectedPointIndex >= 0 && selectedPointIndex < points.Count)
        {
            Gizmos.color = Color.cyan;
            var selectedPoint = points[selectedPointIndex];
            for (int i = 6; i < selectedPoint.cellPlanes.Count; i++)
            {
                Vector3 center = selectedPoint.cellPlanes[i].ClosestPointOnPlane(selectedPoint.position);
                DrawPlane(selectedPoint.cellPlanes[i], center, 2f);
            }
        }
    }

    private void GeneratePoints()
    {
        points.Clear();

        for (int i = 0; i < pointCount; i++)
        {
            float x = Random.Range(0f, cubeSize.x);
            float y = Random.Range(0f, cubeSize.y);
            float z = Random.Range(0f, cubeSize.z);

            Vector3 pos = transform.position + new Vector3(x, y, z);
            points.Add(new VoronoiPoint(pos));
        }

        OrderPoints();

        VoronoiGenerator.BuildCells(points, transform.position, transform.position + cubeSize);

        foreach (var p in points)
        {
            p.SortNeighbors(points);
        }
    }

    private void OrderPoints()
    {
        if (points.Count == 0)
            return;

        Vector3 cubeMin = transform.position;
        Vector3 cubeMax = transform.position + cubeSize;

        float bestDist = float.MaxValue;
        VoronoiPoint firstPoint = points[0];

        foreach (var p in points)
        {
            float distToBorder = Mathf.Min(
                Mathf.Min(Mathf.Abs(p.position.x - cubeMin.x), Mathf.Abs(cubeMax.x - p.position.x)),
                Mathf.Min(Mathf.Abs(p.position.y - cubeMin.y), Mathf.Abs(cubeMax.y - p.position.y)),
                Mathf.Min(Mathf.Abs(p.position.z - cubeMin.z), Mathf.Abs(cubeMax.z - p.position.z))
            );

            if (distToBorder < bestDist)
            {
                bestDist = distToBorder;
                firstPoint = p;
            }
        }

        List<VoronoiPoint> rest = new List<VoronoiPoint>(points);
        rest.Remove(firstPoint);

        rest.Sort((a, b) =>
            (a.position - firstPoint.position).sqrMagnitude.CompareTo((b.position - firstPoint.position).sqrMagnitude)
        );

        points.Clear();
        points.Add(firstPoint);
        points.AddRange(rest);
    }

    private void DrawPlane(MyPlane plane, Vector3 center, float size)
    {
        Vector3 normal = plane.normal;
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 0.001f)
            tangent = Vector3.Cross(normal, Vector3.right);
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent);

        Vector3 c0 = center + (tangent + bitangent) * size;
        Vector3 c1 = center + (tangent - bitangent) * size;
        Vector3 c2 = center + (-tangent - bitangent) * size;
        Vector3 c3 = center + (-tangent + bitangent) * size;

        Gizmos.DrawLine(c0, c1);
        Gizmos.DrawLine(c1, c2);
        Gizmos.DrawLine(c2, c3);
        Gizmos.DrawLine(c3, c0);
    }
}
