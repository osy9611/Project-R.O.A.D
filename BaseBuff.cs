using DesignTable;
using Sword;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuff
{
    private int id;
    public int Id
    {
        get => id;
        set => id = value;
    }

    private skill_effect_buffInfo skillInfo;
    public skill_effect_buffInfo SkillInfo
    {
        get { return skillInfo; }
        set { skillInfo = value; }
    }

    private BuffManager _buffMaanager;

    //시전자가 누구인지 확인하기 위해서 사용
    private BaseActor skillCaster;
    public BaseActor SkillCaster
    {
        get => skillCaster;
        set => skillCaster = value;
    }
  
    private float nowDurationTime = 0;
    private float nowInterval = 0;

    private bool isEnd = false;

    private bool stopInvoke = false; //TODO : 즉발기를 사용하기 위해서 사용
    public bool IsEnd
    {
        get { return isEnd; }
        set { isEnd = value; }
    }

    private SkillActions skillActions;
    public SkillActions SkillActions
    {
        get { return skillActions; }
        set { skillActions = value; }
    }

    public void Init()
    {
        _buffMaanager = new BuffManager();       
        _buffMaanager.baseBuff = this;
        _buffMaanager.Init();
        _buffMaanager.CheckBuffType(skillInfo);        
    }

    public void Execute()
    {
        if (!isEnd)
            CalcDurationTime();       
    }

    public void DeActivation()
    {
        Debug.Log("초기화 진행중");       
        isEnd = true;
        nowDurationTime = 0;
        _buffMaanager.buffDel.Invoke(skillInfo, skillActions);
        _buffMaanager.buffDel = null; 
        if(Self.IsSinglePlayMap())
            skillActions.RemoveBuffList(this);        
    }

    private void CalcDurationTime()
    {
        if (nowDurationTime <= skillInfo.buff_Duration)
        {
            nowDurationTime += Time.deltaTime;
            //즉발기인 경우를 체크하기 위해서 사용
            if (stopInvoke)
                return;

            CalcIntervalTime();
        }
        else
        {            
            if (Self.IsSinglePlayMap())
                DeActivation();
        }
    }

    private void CalcIntervalTime()
    {
        if (skillInfo.buff_Process_type == (short)DesignEnum.buff_Process_type.Once)
        {
            stopInvoke = true;
            _buffMaanager.buffDel.Invoke(skillInfo, skillActions);
            return;
        }
            
        if (nowInterval <= skillInfo.buff_Interval)
        {
            nowInterval += Time.deltaTime;
        }
        else
        {
            _buffMaanager.buffDel.Invoke(skillInfo, skillActions);
            nowInterval = 0;
        }
    }
}
