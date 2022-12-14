using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sword;
using static Define;
using DesignTable;

public class SkillAgent
{
    protected SkillActions skillAction;
    public SkillActions SkillActions
    {
        get { return skillAction; }
    }

    protected string skillAniName;
    public string SkillAniName
    {
        get { return skillAniName; }
        set { skillAniName = value; }
    }

    protected bool moveSkill = false;
    public bool MoveSkill
    {
        get { return moveSkill; }
        set { moveSkill = value; }
    }

    protected int skillId = -1;
    public int SkillId
    {
        get { return skillId; }
        set { skillId = value; }
    }

    protected float skillAttackRange;
    public float SkillAttackRange
    {
        get => skillAttackRange;
        set => skillAttackRange = value;
    }

    protected BaseActor actor;

    public BaseActor Actor
    {
        get { return actor; }
        set { actor = value; }
    }

    protected BaseActor attackTarget;
    public BaseActor AttackTarget
    {
        get => attackTarget;
        set => attackTarget = value;
    }

    protected int nowComboCount = (int)DesignEnum.attack_Id.Common_Attack1;
    public int NowComboCount
    {
        get => nowComboCount;
    }

    protected int spiritCount = 0;
    public int SpiritCount
    {
        get => spiritCount;
        set => spiritCount = value;
    }

    public virtual void Init()
    {
        skillAction = new SkillActions();
        skillAction.SkillAgent = this;
        skillAction.Init();

        if (ComMainManager.Object.CheckMyActor(actor))
        {
            SetSpiritPoint(Self.MyPlayerInfo.spiritSkillGuage);
        }
    }

    public virtual void Execute() { }

    public virtual void ResetData(bool stopCombo = false)
    {
        FSM fsm = actor.GetFSM();

        if (!stopCombo)
        {
            EndCombo();
        }
        else
        {
            skillId = -1;
            nowComboCount = (int)DesignEnum.attack_Id.Common_Attack1;
        }

        moveSkill = false;
        skillAniName = "";
        fsm.AniEnd = false;
    }

    public virtual void CheckAttackRange()
    {

        Dictionary<string, BaseActor> objects = ComMainManager.Object.objects;
        float minRange = 0;
        float range = Actor.GetSkillAgent().SkillActions.Actions[(int)DesignEnum.attack_Id.Common_Attack1].SkillInfo.skill_Range;

        attackTarget = null;

        PlayerActor playerActor = actor as PlayerActor;

        if (playerActor == null)
            return;

        foreach (BaseActor baseActor in objects.Values)
        {
            if (baseActor.UnitType == DesignEnum.Unit_Type.Monster)
            {
                if (baseActor.GetStatus().Hp <= 0 ||
                               baseActor.GetFSM().CurrentObjectState == Define.ObjectState.Death ||
                               baseActor.GetFSM().CurrentObjectState == Define.ObjectState.Spawn)
                    continue;

                float dis = Vector3.Distance(baseActor.Creature.transform.position, actor.Creature.transform.position);

                if (!playerActor.IsAuto)
                {
                    if (dis <= range)
                    {
                       

                        if (attackTarget == null)
                        {
                            attackTarget = baseActor;
                            minRange = dis;
                        }
                        else
                        {
                            if (Mathf.Min(dis, minRange) == dis)
                            {
                                attackTarget = baseActor;
                                minRange = dis;
                            }
                        }
                    }
                }

                if (playerActor.IsAuto)
                {
                    if (attackTarget == null)
                    {
                        attackTarget = baseActor;
                        minRange = dis;
                    }
                    else
                    {
                        if (Mathf.Min(dis, minRange) == dis)
                        {
                            attackTarget = baseActor;
                            minRange = dis;
                        }
                    }
                }
            }
        }
    }

    public virtual void OnAttack(int skillId) { }

    public virtual bool? CheckIsSkill()
    {
        if (skillId == -1)
            return null;

        switch ((DesignEnum.attack_Id)skillId)
        {
            case DesignEnum.attack_Id.Common_Attack1:
            case DesignEnum.attack_Id.Common_Attack2:
            case DesignEnum.attack_Id.Common_Attack3:
                return false;
            case DesignEnum.attack_Id.Skill_Attack1:
            case DesignEnum.attack_Id.Skill_Attack2:
            case DesignEnum.attack_Id.Skill_Attack3:
            case DesignEnum.attack_Id.Skill_Attack4:
            case DesignEnum.attack_Id.Spirit_Attack1:
            case DesignEnum.attack_Id.Spirit_Attack2:
                return true;
        }

        return null;
    }

    public virtual float GetSkillAttackRange(int skillId)
    {
        if (!CheckActions(skillId))
        {
            return 1.5f;
        }
        return skillAction.Actions[skillId].SkillInfo.skill_Range;
    }


    public virtual void StartCombo() { }

    public virtual void EndCombo() { }

    public virtual bool CheckActions(int skillId)
    {
        return skillAction.Actions.ContainsKey(skillId);
    }

    public virtual BaseAction GetSkillAction(int skillId)
    {
        return skillAction.Actions[skillId];
    }

    public virtual bool CheckSkillCool(int skillId)
    {
        if (!CheckActions(skillId))
            return false;

        return skillAction.Actions[skillId].IsCool;
    }

    public virtual void AddBuffList(int skillId, BaseActor skillCaster = null)
    {
        skillAction.AddBuffList(skillId, skillCaster);
    }

    public virtual void AddBuffList(skill_effect_buffInfo buffInfo)
    {
        skillAction.AddBuffList(buffInfo);
    }

    public virtual void AddBuffList_Net(int buffId, int buffDesignId)
    {
        skillAction.AddBuffList_Net(buffId, buffDesignId);
    }

    public virtual void RemoveBuffList_Net(int buffId)
    {
        skillAction.RemoveBufflist_Net(buffId);
    }

    public virtual void SetSpiritPoint(int spiritPoint)
    {
        if (spiritCount < DataHelper.Manager.ComDesignVariable.skill.spirit_max_guage)
        {
            if (Self.IsSinglePlayMap())
                spiritCount += spiritPoint;
            else
                spiritCount = spiritPoint;
        }
        else
        {
            ActiveSpirit(true);
        }
    }

    protected virtual void ActiveSpirit(bool active)
    {
        if (!active)
        {
            spiritCount = 0;
        }
    }

}
