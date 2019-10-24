using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyaleAreaConfig : MonoBehaviour
{
    [SerializeField] private Vector3 m_Mapcenter;
    [SerializeField] private float m_StableTime;
    [SerializeField] private float m_damageDuration;
    [SerializeField] public List<PosionData> m_ListPoisonData=new List<PosionData>();
    public Vector3 Mapcenter => m_Mapcenter;
    public List<PosionData> ListPoisonData => m_ListPoisonData;
    public float StableTime => m_StableTime;
    public float DamageDuration => m_damageDuration;
}
[System.Serializable]
public class PosionData
{
    public int index;
    public int Radius;
    public float DamageValue;
    public float PreShrink;
    public float ShrinkTime;
}
