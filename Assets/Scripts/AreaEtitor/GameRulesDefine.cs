using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct BattleRoyaleGameRuleDataStruct
{
    public List<PolygonArea> PrepareArea;
    public List<PolygonArea> BornArea;
    public PolygonArea GameArea;
}




[System.Serializable]
public struct PolygonArea
{
    public static readonly Vector3 Normal = Vector3.up; // 多边形的方向
    public List<Vector3> Vertexes;        // 多边形顶点
    public List<ThreeVector3> Triangles;  // 多边形三角化之后的所有三角形区域
    public List<float> TriangleAreas;     // 记录每个三角形的面积


    [SerializeField] private Vector3 m_Origin;   // 原点
    [SerializeField] private Vector3 m_Direction; // 朝向
    [SerializeField] private float m_Area;       // 总面积
    [SerializeField] private Vector3 m_Offset; // Editor使用

    public float Area
    {
        get => m_Area;
        set => m_Area = value;
    }

    public Vector3 Direction => m_Direction;
}




