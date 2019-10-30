using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonAreaConfig : MonoBehaviour
{
    [SerializeField] private PolygonArea m_Area;
    [SerializeField] private string m_AreaName;
    [SerializeField] private bool m_Foldout;
    [SerializeField] private float m_pSize;
    [SerializeField] private Color m_AreaColor;
    [SerializeField] private float m_AreaVertialOffsetBase;
    [SerializeField] private float m_DirLineLength;
    [SerializeField] private float m_DirLineWidth;

    public PolygonArea Area => m_Area;
    public string AreaName => m_AreaName;
}
