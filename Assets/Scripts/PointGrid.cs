using System.Collections.Generic;
using UnityEngine;

public class PointGrid : MonoBehaviour
{
    [SerializeField] private int gridSize = 5;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private Vector3 offset = Vector3.zero;

    [SerializeField] private float pointRadius = 0.1f;
    [SerializeField] private Color pointColor = Color.white;

    public List<Vector3> points = new List<Vector3>();

    private void OnValidate()
    {
        GeneratePoints();
    }

    private void OnDrawGizmos()
    {
        if (points == null || points.Count == 0)
            GeneratePoints();

        Gizmos.color = pointColor;
        foreach (Vector3 p in points)
        {
            Gizmos.DrawSphere(p, pointRadius);
        }
    }

    private void GeneratePoints()
    {
        points.Clear();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Vector3 pos = transform.position + offset + new Vector3(
                        x * spacing,
                        y * spacing,
                        z * spacing
                    );
                    points.Add(pos);
                }
            }
        }
    }
}
