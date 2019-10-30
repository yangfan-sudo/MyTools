using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct ThreeVector3 : IEnumerable<Vector3>
{
    public Vector3 x;
    public Vector3 y;
    public Vector3 z;

    public ThreeVector3(Vector3 x, Vector3 y, Vector3 z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public void DrawGizmos(float radius)
    {
        Gizmos.DrawSphere(x, radius);
        Gizmos.DrawSphere(y, radius);
        Gizmos.DrawSphere(z, radius);
    }

    public const int Length = 3;
    public int Count => Length;

    public Vector3 this[int index]
    {
        get => GetByIndex(index);
        set => SetByIndex(index, value);
    }


    public IEnumerator<Vector3> GetEnumerator()
    {
        for (int i = 0; i < Count; ++i)
        {
            yield return GetByIndex(i);
        }
    }

    private Vector3 GetByIndex(int index)
    {
        switch (index)
        {
            case 0: return x;
            case 1: return y;
            case 2: return z;
            default: throw new System.ArgumentOutOfRangeException();
        }
    }

    private void SetByIndex(int index, Vector3 value)
    {
        switch (index)
        {
            case 0: x = value; break;
            case 1: y = value; break;
            case 2: z = value; break;
            default: throw new System.ArgumentOutOfRangeException();
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return $"x:{x},y:{y},z:{z}";
    }
}