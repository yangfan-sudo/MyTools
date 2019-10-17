using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SolarLand/Posion", fileName = "PosionCfg")]
public class PoisonConfig : ScriptableObject
{
    public List<PosionData> listPosionData = new List<PosionData>();
}
[System.Serializable]
public class PosionData
{
    public int Radius;
    public float Durationtime;
    public float ReduceRadiusOneSecond;
}