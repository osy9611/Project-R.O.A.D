using DesignEnum;
using DesignTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Koyo.Unity.UGui.Hud;

public enum StatusType
{
    Default,
    Buff,
    Item,
    Spirit,
}

public class Status
{
    protected BaseActor actor;
    public BaseActor Actor
    {
        get { return actor; }
        set { actor = value; }
    }

    protected char_StatusInfo baseStatusInfo; 
    public char_StatusInfo BaseStatusInfo
    {
        get => baseStatusInfo;
    }

    protected long[] totalStatus;
    public long[] TotalStatus
    {
        get => totalStatus;
    }

    protected long totalForce;
    public long TotalForce
    {
        get => totalForce;
    }

    protected StatusType<BaseActor>[] statusTypes;
    public StatusType<BaseActor>[] StatusTypes
    {
        get => statusTypes;
    }

    protected long hp = 1000;
    public long Hp
    {
        get => hp;
        set
        {
            hp = value;

            if (actor.Creature != null)
            {
                var bar = actor.Creature.ComUIElemHudUnitInfo.ComHealthBar;
                bar.SetNormalize((float)hp / (float)maxHp);
            }
        }
    }

    protected long maxHp;
    public long MaxHp
    {
        get => maxHp;
        set => maxHp= value;
    }

    protected long exp = 0;
    public long Exp
    {
        get => exp;
    }

    public delegate void CheckStatus();
    protected CheckStatus checkStatusDel;
    public CheckStatus CheckStatusDel
    {
        get => checkStatusDel;
        set => checkStatusDel = value;
    }

    public virtual void Init()
    {
        statusTypes = new StatusType<BaseActor>[Enum.GetValues(typeof(StatusType)).Length];
        totalStatus = new long[Enum.GetValues(typeof(Status_Id)).Length];
        
    }

    public virtual void Execute() { }
    public virtual void DeathCheck() 
    {
        hp = 0;
        actor.GetFSM().ChangeState(Define.ObjectState.Death);

    }

    public virtual void CalcTotalStatus() { }

    public virtual void IncreaseStatus(StatusType type, Status_Id statusInfo, Buff_Type buffType, skill_effect_buffInfo buffInfo)
    {
        statusTypes[(int)type].IncreaseStatus(actor, statusInfo, buffType, buffInfo);
    }

    public virtual void IncreaseStatus(StatusType type, Status_Id statusInfo, long value)
    {
        statusTypes[(int)type].IncreaseStatus(actor, statusInfo, value);
    }

    public virtual void IncreaseStatus(StatusType type, Status_Id statusInfo, Buff_Type buffType, long value)
    {
        statusTypes[(int)type].IncreaseStatus(actor, statusInfo, buffType, value);
    }

    public virtual void DecreaseStatus(StatusType type, Status_Id statusInfo, Buff_Type buffType, skill_effect_buffInfo buffInfo)
    {
        statusTypes[(int)type].DecreaseStatus(actor, statusInfo, buffType, buffInfo);
    }

    public virtual void DecreaseStatus(StatusType type, Status_Id statusInfo, long value)
    {
        statusTypes[(int)type].DecreaseStatus(actor, statusInfo, value);
    }

    public virtual void DecreaseStatus(StatusType type, Status_Id statusInfo, Buff_Type buffType, long value)
    {
        statusTypes[(int)type].DecreaseStatus(actor, statusInfo, buffType, value);
    }

    public virtual void ResetStatus(StatusType type, Status_Id statusInfo)
    {
        statusTypes[(int)type].ResetStatus(actor, statusInfo);
    }

    public virtual void ResetStatus(StatusType type, Status_Id statusInfo, Buff_Type buffType)
    {
        statusTypes[(int)type].ResetStatus(actor, statusInfo, buffType);
    }

    public virtual void InitExp(int exp) { }
    public virtual void InitLevel(int level) { }



}
