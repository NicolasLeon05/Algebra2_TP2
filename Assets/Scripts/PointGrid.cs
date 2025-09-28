using System.Collections.Generic;
using UnityEngine;

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

    public List<VoronoiPoint> points = new List<VoronoiPoint>();

    private void OnValidate()
    {
        GeneratePoints();
    }

    private void OnDrawGizmos()
    {
        if (points == null || points.Count == 0)
            GeneratePoints();

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
                //Mathf.Min(Mathf.Abs(p.position.y - cubeMin.y), Mathf.Abs(cubeMax.y - p.position.y)),
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
}
