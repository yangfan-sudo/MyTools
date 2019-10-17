using UnityEngine;
using System;

public class GizmosCircle : MonoBehaviour
{
    public Transform m_Transform;
    public float m_Radius = 1; // 圆环的半径
    //圆形被切分的平均弧度
    //圆形总弧度 6.283185  360*Mathf.Deg2Rad
    public float m_Theta = 0.1f; // 值越低圆环越平滑
    public Color m_Color = Color.green; // 线框颜色

    void Start()
    {
        if (m_Transform == null)
        {
            m_Transform = transform;
            //throw new Exception("Transform is NULL.");
        }
    }

    void OnDrawGizmos()
    {
        if (m_Transform == null) return;
        if (m_Theta < 0.0001f) m_Theta = 0.0001f;

        // 设置矩阵
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        Gizmos.matrix = m_Transform.localToWorldMatrix;

        // 设置颜色
        Color defaultColor = Gizmos.color;
        Gizmos.color = m_Color;

        // 绘制圆环
        Vector3 beginPoint = Vector3.zero;
        Vector3 firstpoint = Vector3.zero;
        for(float theta=0;theta<2*Mathf.PI;theta+=m_Theta)
        {
            float x = Mathf.Cos(theta) * m_Radius;
            float z = Mathf.Sin(theta) * m_Radius;
            Vector3 endpoint = new Vector3(x, 0, z);
            if (theta == 0)
            {
                firstpoint = endpoint;
            }
            else
            {
                Gizmos.DrawLine(beginPoint, endpoint);
            }
            beginPoint = endpoint;
        }

        // 绘制最后一条线段
        Gizmos.DrawLine(beginPoint, firstpoint);

        // 恢复默认颜色
        Gizmos.color = defaultColor;

        // 恢复默认矩阵
        Gizmos.matrix = defaultMatrix;
    }
}