using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PolygonAreaExtend
{
    public static Vector3 RandomPoint(this PolygonArea area)
    {
        switch (area.Vertexes.Count)
        {
            case 0: return Vector3.zero;
            case 1: return area.Vertexes[0];
            case 2: return RandomPoint(area.Vertexes[0], area.Vertexes[1]);
            case 3: return area.Triangles[0].RandomTrianglePoint();
        }

        float n = Random.Range(0, area.Area);
        float c = 0;
        for(int i=0;i<area.TriangleAreas.Count;++i)
        {
            var triArea = area.TriangleAreas[i];
            c += triArea;
            if(c >= n)
                return area.Triangles[i].RandomTrianglePoint();
        }
        return area.Triangles[area.Triangles.Count - 1].RandomTrianglePoint();
    }

    public static Vector3 RandomTrianglePoint(this ThreeVector3 triangle)
    {
        float r1 = Mathf.Sqrt(Random.Range(0.0f, 1.0f));
        float r2 = Random.Range(0.0f, 1.0f);
        return (1 - r1) * triangle.x + r1 * (1 - r2) * triangle.y + r1 * r2 * triangle.z;
    }

    public static float CalcTriangleArea(this ThreeVector3 triangle)
    {
        float a = Vector3.Distance(triangle.x, triangle.y);
        float b = Vector3.Distance(triangle.y, triangle.z);
        float c = Vector3.Distance(triangle.z, triangle.x);
        float s = (a + b + c) / 2.0f;
        return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
    }

    public static Vector3 RandomPoint(Vector3 p1, Vector3 p2)
    {
        return p1 + (p2 - p1).normalized * Random.Range(0.0f, 1.0f) * Vector3.Distance(p1, p2);
    }

    public static bool IsContainsPoint(this PolygonArea area, Vector3 point)
    {
        if (area.Vertexes.Count <= 0)
            return false;
        Plane plane = new Plane(-PolygonArea.Normal, area.Vertexes[0]);
        point = plane.GetPointOnPlane(point);
        foreach (var tri in area.Triangles)
        {
            if (tri.IsContainsPoint(point))
                return true;
        }
        return false;
    }

    public static bool IsContainsPoint(this ThreeVector3 triangle, Vector3 point)
    {
        return PointsOnSameSideLine(point, triangle.x, triangle.y, triangle.z) &&
               PointsOnSameSideLine(point, triangle.y, triangle.x, triangle.z) &&
               PointsOnSameSideLine(point, triangle.z, triangle.x, triangle.y);
    }

    public static Vector3 GetPointOnPlane(this Plane plane, Vector3 point)
    {
        Vector3 o = -plane.normal * plane.distance;
        float d = Vector3.Dot(point - o, plane.normal);
        return point - d * plane.normal;
    }

    public static Vector3 GetCentroid(this PolygonArea polygon)
    {
        return GetCentroid(polygon.Vertexes);
    }

    public static Vector3 GetCentroid(List<Vector3> vertexes)
    {
        switch (vertexes.Count)
        {
            case 0: return Vector3.zero;
            case 1: return vertexes[0];
            case 2: return (vertexes[0] + vertexes[1]) / 2.0f;
            default:
            {
                var s = new Vector3();
                var areaTotal = 0.0f;

                var p1 = vertexes[0];
                var p2 = vertexes[1];

                for (var i = 2; i < vertexes.Count; i++)
                {
                    var p3 = vertexes[i];
                    var edge1 = p3 - p1;
                    var edge2 = p3 - p2;

                    var crossProduct = Vector3.Cross(edge1, edge2);
                    var area = crossProduct.magnitude / 2;

                    s.x += area * (p1.x + p2.x + p3.x) / 3;
                    s.y += area * (p1.y + p2.y + p3.y) / 3;
                    s.z += area * (p1.z + p2.z + p3.z) / 3;

                    areaTotal += area;
                    p2 = p3;
                }
                return Mathf.Approximately(areaTotal, 0) ? s : s / areaTotal;
            }
        }
    }

    private static bool PointsOnSameSideLine(Vector3 p1, Vector3 p2, Vector3 lp1, Vector3 lp2)
    {
        Vector3 cp1 = Vector3.Cross((lp2 - lp1), (p1 - lp1));
        Vector3 cp2 = Vector3.Cross((lp2 - lp1), (p2 - lp1));
        return Vector3.Dot(cp1, cp2) >= 0;
    }
}
