using Poly2Tri;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PolygonHelper
{
    public static void Triangulate(Vector3[] polygon, Vector3 normal, out List<ThreeVector3> triangles)
    {
        if (polygon.Length < 3)
        {
            triangles = new List<ThreeVector3>();
            return;
        }

        //var planeNormal = Vector3.Cross(polygon[1] - polygon[0], polygon[2] - polygon[0]);
        //planeNormal.Normalize();
        //if (Vector3.Angle(planeNormal, normal) > Vector3.Angle(-planeNormal, normal))
        //{
        //    planeNormal = -planeNormal;
        //}

        //Quaternion rotation = Quaternion.identity;
        //if (planeNormal != Vector3.forward)
        //{
        //    rotation = Quaternion.FromToRotation(planeNormal, Vector3.forward);
        //}

        Quaternion rotation = Quaternion.FromToRotation(normal, Vector3.forward);

        // Rotate 1 point and note where it ends up in Z
        float z = (rotation * polygon[0]).z;

        var poly = new Polygon(ConvertPoints(polygon, rotation));

        DTSweepContext tcx = new DTSweepContext();
        tcx.PrepareTriangulation(poly);
        DTSweep.Triangulate(tcx);
        tcx = null;

        Quaternion invRot = Quaternion.Inverse(rotation);

        triangles = new List<ThreeVector3>();
        foreach (DelaunayTriangle t in poly.Triangles)
        {
            ThreeVector3 tri = new ThreeVector3();

            for (int i = 0; i < 3; ++i)
            {
                var p = t.Points[i];
                Vector3 pos = new Vector3(p.Xf, p.Yf, z);
                tri[i] = invRot * pos;
            }

            triangles.Add(tri);
        }
    }

    public static void Triangulate(List<Vector3> polygon, Vector3 dir, out List<ThreeVector3> triangles)
    {
        Triangulate(polygon.ToArray(), dir, out triangles);
    }

    static List<PolygonPoint> ConvertPoints(Vector3[] points, Quaternion rotation)
    {
        int count = points.Length;
        List<PolygonPoint> result = new List<PolygonPoint>(count);
        for (int i = 0; i < count; i++)
        {
            Vector3 p = rotation * points[i];
            result.Add(new PolygonPoint(p.x, p.y));
        }
        return result;
    }
}
