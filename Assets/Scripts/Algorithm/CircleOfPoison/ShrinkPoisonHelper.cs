using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShrinkPoisonHelper
{
    enum PoisonState
    {
        stableTime,
        preShrink,
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
    //毒圈出现前等待时间
    private float currentStableTime = 0;
    //毒圈出现前等待时间
    private float stableTimeConfig = 99999;

    private PoisonState m_poisonstate = PoisonState.stableTime;
    public ShrinkPoisonHelper(BattleRoyaleAreaConfig poisonconfig, Action<float, Vector2> poisonCircleChange, Action<float, Vector2> safeCircleChange, Action nosafeareaCallBack)
    {
        m_OutCircleChangeAction = poisonCircleChange;
        m_InCircleChangeAction = safeCircleChange;
        m_NoSafeAreaCallBack = nosafeareaCallBack;
        mPoint_outer = new Vector2(poisonconfig.Mapcenter.x, poisonconfig.Mapcenter.z);
        m_CurrentLevel = 0;
        m_PoisonConfig = poisonconfig;
        stableTimeConfig = m_PoisonConfig.StableTime;
        isStart = true;

    }
    private void InitPoisonAndSafeCircle()
    {
        mRadius_outer = getCurrentLevelRadius();
        refreshNewSafeCircle();
    }
    public void Update(float dt)
    {
        if (!isStart || loadConfigError || m_poisonstate == PoisonState.End)
        {
            return;
        }
        switch (m_poisonstate)
        {
            case PoisonState.stableTime:
                stableTime(dt);
                break;
            case PoisonState.preShrink:
                WaitingState(dt);
                break;
            case PoisonState.ShrinkageRing:
                shrinkPoisonCirCleRadius();
                break;
        }
    }
    private void stableTime(float dt)
    {
        if (currentStableTime >= stableTimeConfig)
        {
            m_poisonstate = PoisonState.preShrink;
            currentStableTime = 0;
            InitPoisonAndSafeCircle();
        }
        else
        {
            currentStableTime += dt;
        }

    }
    private void WaitingState(float dt)
    {
        if (curDurationTime >= configDurationTime)
        {
            m_poisonstate = PoisonState.ShrinkageRing;
            curDurationTime = 0;
        }
        else
        {
            curDurationTime += dt;
        }
    }
    private void upLevel()
    {
        m_CurrentLevel++;

    }
    private void ResetConfigData(bool haveNextData)
    {
        if (loadConfigError || m_CurrentLevel == 0)
        {
            return;
        }
        configDurationTime = m_PoisonConfig.ListPoisonData[m_CurrentLevel - 1].PreShrink;
        if (haveNextData)
        {
            configReduceRadiusSpeed = (m_PoisonConfig.ListPoisonData[m_CurrentLevel - 1].Radius -
                m_PoisonConfig.ListPoisonData[m_CurrentLevel].Radius) / m_PoisonConfig.ListPoisonData[m_CurrentLevel - 1].ShrinkTime;
        }
        else
        {
            configReduceRadiusSpeed = m_PoisonConfig.ListPoisonData[m_CurrentLevel - 1].Radius / m_PoisonConfig.ListPoisonData[m_CurrentLevel - 1].ShrinkTime;
        }

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
        float changeradius = configReduceRadiusSpeed / 60;
        mRadius_outer -= changeradius;
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
                // x^2+y^2 = changeradius^2
                // x^2 = 1/(k^2+1)
                float k = (mPoint_outer.y - mPoint_inner.y) / (mPoint_outer.x - mPoint_inner.x);

                float x_off = 1 * (float)Mathf.Sqrt((changeradius* changeradius) / (k * k + 1));
                // k<0  x+x_off
                mPoint_outer.x += 1 * (mPoint_outer.x < mPoint_inner.x ? 1 : -1) * x_off;
                mPoint_outer.y = k * (mPoint_outer.x - mPoint_inner.x) + mPoint_inner.y;
                m_OutCircleChangeAction?.Invoke(mRadius_outer, mPoint_outer);
            }
            else
            {
                m_OutCircleChangeAction?.Invoke(mRadius_outer, mPoint_outer);
                refreshNewSafeCircle();
                m_poisonstate = PoisonState.preShrink;
            }
        }
        if (mRadius_outer < 1)
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
            ResetConfigData(true);
            mRadius_inner = getCurrentLevelRadius();
            mPoint_inner = CircleMathfHelper.PointOfRandom(mPoint_outer, mRadius_outer, mRadius_inner);

            m_InCircleChangeAction?.Invoke(mRadius_inner, mPoint_inner);
            m_OutCircleChangeAction?.Invoke(mRadius_outer, mPoint_outer);
        }
        else
        {
            mRadius_inner = 0;
            mPoint_inner = mPoint_outer;
            ResetConfigData(false);
            m_NoSafeAreaCallBack?.Invoke();
        }

    }
    public float GetLeftTime()
    {
        return configDurationTime - curDurationTime;
    }
}

