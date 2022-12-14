using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class FSM 
{
    protected BaseActor actor;
    public BaseActor Actor
    {
        get { return actor; }
        set { actor = value; }
    }

    protected bool aniEnd;
    public bool AniEnd
    {
        get { return aniEnd; }
        set { aniEnd = value; }
    }

    protected State<BaseActor>[] states;
    protected StateMachine<BaseActor> stateMachine;
    public StateMachine<BaseActor> StateMachine
    {
        get { return stateMachine; }
    }

    private BitArray conditions = new BitArray(Enum.GetValues(typeof(ObjectCondition)).Length);
    public BitArray Conditions
    {
        get => conditions;
    }

    private BitArray checkAniStates = new BitArray(Enum.GetValues(typeof(CheckAnimationState)).Length);
    public BitArray CheckAniStates
    {
        get => checkAniStates;
    }

    private ObjectState prevObjectState;
    public ObjectState PrevObjectState
    {
        get => prevObjectState;
    }

    private ObjectState currentObjectState;
    public ObjectState CurrentObjectState
    {
        get => currentObjectState;
    }

    public virtual void Init() 
    {
        states = new State<BaseActor>[Enum.GetValues(typeof(ObjectState)).Length];
        stateMachine = new StateMachine<BaseActor>();
    }

    public virtual void Execute()
    {
        stateMachine.Execute();

    }

    public virtual void ChangeState(ObjectState state)
    {
        if(state != currentObjectState)
        {
            prevObjectState = currentObjectState;
        }           
        currentObjectState = state;       

        
        stateMachine.ChangeState(states[(int)state]);
    }

    public virtual void SetCondition(ObjectCondition condition, bool option)
    { 
        conditions.Set((int)condition, option);
    }

    public virtual bool CheckCondition(ObjectCondition condition)
    {
        if (conditions.Length == 0)
            return false;

        return conditions.Get((int)condition);
    }

    public virtual void SetAniState(CheckAnimationState state, bool option)
    {
        checkAniStates.Set((int)state, option);
    }
}
