using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkPoisonHelper
{
    enum PoisonState
    {
        Waiting,
        ShrinkageRing,
        End,
    }
    //外圈，内圈圆心，半径
    private Vector2 mPoint_outer;
    private Vector2 mPoint_inner;
    private float mRadius_outer = 0;
    private float mRadius_inner = 0;
    //外圈圆变化回调
    private Action<float, Vector2> m_OutCircleChangeAction;
    //内圈圆变化回调
    private Action<float, Vector2> m_InCircleChangeAction;

    private Action m_NoSafeAreaCallBack;
    //毒圈配置
    private BattleRoyaleAreaConfig m_PoisonConfig;
    private int m_CurrentLevel;
    //计时开始
    public bool isStart = false;
    private bool loadConfigError = false;

    //当前等待缩圈时间
    private float curDurationTime = 0;
    //配置等待缩圈时间
    private float configDurationTime = 0;
    //配置缩圈速度
    private float configReduceRadiusSpeed = 0;
    private PoisonState m_poisonstate = PoisonState.Waiting;
    public ShrinkPoisonHelper(BattleRoyaleAreaConfig poisonconfig,Action<float, Vector2>poisonCircleChange, Action<float, Vector2> safeCircleChange,Action nosafeareaCallBack)
    {
        m_OutCircleChangeAction = poisonCircleChange;
        m_InCircleChangeAction = safeCircleChange;
        m_NoSafeAreaCallBack = nosafeareaCallBack;
        mPoint_outer = Vector2.zero;
        m_CurrentLevel = 0;       
        m_PoisonConfig = poisonconfig;
        ResetConfigData();
        InitPoisonAndSafeCircle();
        isStart = true;
    }
    private void InitPoisonAndSafeCircle()
    {
        mRadius_outer = getCurrentLevelRadius();
        refreshNewSafeCircle();
    }
    public void Update()
    {
        if(!isStart || loadConfigError|| m_poisonstate== PoisonState.End)
        {
            return;
        }
        switch(m_poisonstate)
        {
            case PoisonState.Waiting:
                WaitingState();
                break;
            case PoisonState.ShrinkageRing:
                shrinkPoisonCirCleRadius();
                break;
        }   
    }
    private void WaitingState()
    {
        if(curDurationTime>= configDurationTime)
        {
            m_poisonstate = PoisonState.ShrinkageRing;
            curDurationTime = 0;
        }
        else
        {
            curDurationTime += Time.deltaTime;
        }
    }
    private void upLevel()
    {
        m_CurrentLevel++;
        ResetConfigData();
    }
    private void ResetConfigData()
    {
        if(loadConfigError)
        {
            return;
        }
        configDurationTime = m_PoisonConfig.ListPoisonData[m_CurrentLevel].Durationtime;
        configReduceRadiusSpeed = m_PoisonConfig.ListPoisonData[m_CurrentLevel].ReduceRadiusOneSecond;
    }
    //获得当前等级的半径
    private int getCurrentLevelRadius()
    {
        if (loadConfigError)
            return 0;
        return m_PoisonConfig.ListPoisonData[m_CurrentLevel].Radius;
    }
    //缩毒圈
    private void shrinkPoisonCirCleRadius()
    {
        mRadius_outer -= configReduceRadiusSpeed/30;
        if (!CircleMathfHelper.isIntersect(mPoint_outer, mRadius_outer, mPoint_inner, mRadius_inner))
        {
            m_OutCircleChangeAction?.Invoke(mRadius_outer, mPoint_outer);
        }
        else
        {
            if (mRadius_outer > mRadius_inner)  //外圈和内圈圆心重合,半径相同
            {
                // k = y/x
                // y = kx
                // x^2+y^y = 1
                // x^2 = 1/(k^2+1)
                float k = (mPoint_outer.y - mPoint_inner.y) / (mPoint_outer.x - mPoint_inner.x);

                float x_off = 1 * (float)Mathf.Sqrt(1 / (k * k + 1));
                // k<0  x+x_off
                mPoint_outer.x += 1 * (mPoint_outer.x < mPoint_inner.x ? 1 : -1) * x_off;
                mPoint_outer.y = k * (mPoint_outer.x - mPoint_inner.x) + mPoint_inner.y;
                m_OutCircleChangeAction?.Invoke(mRadius_outer, mPoint_outer);
            }
            else
            {
                m_OutCircleChangeAction?.Invoke(mRadius_outer, mPoint_outer);
                refreshNewSafeCircle();
                m_poisonstate = PoisonState.Waiting;
            }
        }
        if(mRadius_outer<=0)
        {
            m_poisonstate = PoisonState.End;
        }
    }
    /// <summary>
    /// 毒圈收缩到安全区大小重新生成安全区
    /// </summary>
    private void refreshNewSafeCircle()
    {
        if (m_CurrentLevel < m_PoisonConfig.ListPoisonData.Count - 1)
        {
            upLevel();
            mRadius_inner = getCurrentLevelRadius();
            mPoint_inner = CircleMathfHelper.PointOfRandom(mPoint_outer, mRadius_outer, mRadius_inner);

            m_InCircleChangeAction?.Invoke(mRadius_inner, mPoint_inner);
        }
        else
        {
            mRadius_inner = 0;
            mPoint_inner = mPoint_outer;
            m_NoSafeAreaCallBack?.Invoke();
        }
    }
    public float GetLeftTime()
    {
        return configDurationTime - curDurationTime;
    }
}
