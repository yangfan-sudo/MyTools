using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyaleGameArea : MonoBehaviour
{
    [SerializeField] private List<PolygonAreaConfig> m_PrepareArea;
    [SerializeField] private List<PolygonAreaConfig> m_BornArea;
    [SerializeField] private PolygonAreaConfig m_GameArea;
    [SerializeField] private List<int> m_PlayerAreaIndexes;
    [SerializeField] private int m_BattleAreaIndex;


    public List<PolygonAreaConfig> PrepareArea => m_PrepareArea;
    public List<PolygonAreaConfig> BornArea => m_BornArea;
    public PolygonAreaConfig GameArea => m_GameArea;
    public List<int> PlayerAreaIndexes => m_PlayerAreaIndexes;
    public int BattleAreaIndex => m_BattleAreaIndex;
}
