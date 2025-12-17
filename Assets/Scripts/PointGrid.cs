using System.Collections.Generic;
using UnityEngine;
using CustomMath;
using UnityEngine.UIElements;

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

    [Header("Plane Gizmos size")]
    [SerializeField] private float size = 5f;

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

        //Draw selected points planes
        Gizmos.color = Color.magenta;
        foreach (var plane in points[selectedPointIndex].cellPlanes)
        {
            //DrawPlane(plane.Point, plane.normal);
            Vector3 center = plane.normal * plane.distance;
            //Vector3 center = plane.Point;
            Vector3 axisA = Vector3.Cross(plane.normal, Vector3.right).normalized;
            Vector3 axisB = Vector3.Cross(plane.normal, axisA).normalized;
            
            Gizmos.DrawLine(center + axisA * size + axisB * size, center + axisA * size - axisB * size);
            Gizmos.DrawLine(center + axisA * size - axisB * size, center - axisA * size - axisB * size);
            Gizmos.DrawLine(center - axisA * size - axisB * size, center - axisA * size + axisB * size);
            Gizmos.DrawLine(center - axisA * size + axisB * size, center + axisA * size + axisB * size);
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

        foreach (VoronoiPoint point in points)
            VoronoiGenerator.DebugCell(point);
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

    void DrawPlane(Vector3 position, Vector3 normal)
    {

        Vector3 v3;

        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;

        var corner0 = position + v3;
        var corner2 = position - v3;

        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Debug.DrawLine(corner0, corner2, Color.magenta);
        Debug.DrawLine(corner1, corner3, Color.magenta);
        Debug.DrawLine(corner0, corner1, Color.magenta);
        Debug.DrawLine(corner1, corner2, Color.magenta);
        Debug.DrawLine(corner2, corner3, Color.magenta);
        Debug.DrawLine(corner3, corner0, Color.magenta);
        Debug.DrawRay(position, normal, Color.red);
    }

}
