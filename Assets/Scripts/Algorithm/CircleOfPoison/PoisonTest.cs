using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTest : MonoBehaviour
{
    public GizmosCircle Poison;
    public GizmosCircle Safe;
    private ShrinkPoisonHelper m_shrinkPoisonHelper;
    // Start is called before the first frame update
    GUIStyle showstyle = new GUIStyle();
    void Start()
    {
        m_shrinkPoisonHelper = new ShrinkPoisonHelper(OutCirCleChange, InCirCleChange, NoSafeAreaCallBack);
        showstyle.normal.textColor = Color.yellow;
        showstyle.fontSize = 40;
    }
    // Update is called once per frame
    void Update()
    {
        m_shrinkPoisonHelper?.Update();
    }
    public void OutCirCleChange(float mRadius_outer,Vector2 mPoint_outer)
    {
        Poison.m_Radius = mRadius_outer;
        Poison.transform.localPosition = new Vector3(mPoint_outer.x, 0, mPoint_outer.y);
    }
    public void InCirCleChange(float mRadius_inner, Vector2 mPoint_inner)
    {
        Safe.gameObject.SetActive(true);
        Safe.m_Radius = mRadius_inner;
        Safe.transform.localPosition = new Vector3(mPoint_inner.x, 0, mPoint_inner.y);
    }
    public void NoSafeAreaCallBack()
    {
        Safe.gameObject.SetActive(false);
    }
    private void OnGUI()
    {
        
        GUILayout.Label(m_shrinkPoisonHelper?.GetLeftTime().ToString(),showstyle);
    }
}
