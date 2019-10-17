using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyaleAreaConfig : MonoBehaviour
{
    [SerializeField] private Vector3 m_Mapcenter;
    [SerializeField] public List<PosionData> m_ListPoisonData=new List<PosionData>();
    public Vector3 Mapcenter => m_Mapcenter;
    public List<PosionData> ListPoisonData => m_ListPoisonData;
}
[System.Serializable]
public class PosionData
{
    public int Radius;
    public float Durationtime;
    public float ReduceRadiusOneSecond;
}
