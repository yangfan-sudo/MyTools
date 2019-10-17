using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMathfHelper
{
    /// <summary>
    /// 在圆心为point，半径为radius的圆内，产生一个半径为radius_inner的圆的圆心
    /// </summary>
    /// <param name="point">外圆圆心</param>
    /// <param name="radius_outer">外圆半径</param>
    /// <param name="radius_inner">内圆半径</param>
    /// <returns>内圆圆心</returns>
    public static Vector2 PointOfRandom(Vector2 point, float radius_outer, float radius_inner)
    {
        int x = Random.Range((int)(point.x - (radius_outer - radius_inner)), (int)(point.x + (radius_outer - radius_inner)));
        int y = Random.Range((int)(point.y - (radius_outer - radius_inner)), (int)(point.y + (radius_outer - radius_inner)));

        while (!isInRegion(x - point.x, y - point.y, radius_outer - radius_inner))
        {
            x = Random.Range((int)(point.x - (radius_outer - radius_inner)), (int)(point.x + (radius_outer - radius_inner)));
            y = Random.Range((int)(point.y - (radius_outer - radius_inner)), (int)(point.y + (radius_outer - radius_inner)));
        }

        Vector2 p = new Vector2(x, y);
        return p;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x_off">与大圆圆心的x方向偏移量</param>
    /// <param name="y_off">与大圆圆心的y方向偏移量</param>
    /// <param name="distance">大圆与小圆半径的差</param>
    /// <returns>判断点是否在范围内</returns>
    public static bool isInRegion(float x_off, float y_off, float distance)
    {
        if (x_off * x_off + y_off * y_off <= distance * distance)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断两个圆是否重合，或者是相内切
    /// </summary>
    /// <param name="p_outer">外圆圆心</param>
    /// <param name="r_outer">外圆半径</param>
    /// <param name="p_inner">内圆圆心</param>
    /// <param name="r_inner">内圆半径</param>
    /// <returns>是否相内切</returns>
    public static bool isIntersect(Vector2 p_outer, float r_outer, Vector2 p_inner, float r_inner)
    {
        //判定条件：两圆心的距离 + r_inner = r_outer

        float distance = (float)Mathf.Sqrt((p_outer.x - p_inner.x) * (p_outer.x - p_inner.x) + (p_outer.y - p_inner.y) * (p_outer.y - p_inner.y));

        if (distance + r_inner >= r_outer)
        {
            return true;
        }
        return false;
    }
}
