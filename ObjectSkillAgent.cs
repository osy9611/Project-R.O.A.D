using Sword;
using Sword.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectSkillAgent : SkillAgent
{
    public override void Init()
    {
        base.Init();
        skillAniName = "";
    }

    public override void Execute()
    {
        skillAction.Execute();

        if (Managers.Object.CheckMyActor(actor))
        {
            CheckAttackRange();
        }
    }

    public override void ResetData(bool stopCombo = false)
    {
        base.ResetData(stopCombo);
    }

    public override void OnAttack(int _skillId)
    {
        if (actor.GetFSM().CheckCondition(ObjectCondition.Slient) ||
            actor.GetFSM().CurrentObjectState == ObjectState.Death ||
            actor.GetFSM().CurrentObjectState == ObjectState.Spawn
            )
            return;

        if (Managers.Object.CheckMyActor(actor)
            && actor.GetFSM().CurrentObjectState == ObjectState.Skill)
            return;

        if (!skillAction.Actions.ContainsKey(_skillId) || skillAction.Actions[_skillId].IsCool)
            return;

        if (!Self.IsSinglePlayMap() && ComMainManager.Object.CheckMyActor(actor))
        {
            Vector3 lookForward = actor.Creature.transform.forward;
            RequestOperations.Attack(Self.Peer, (short)_skillId, lookForward);
        }

        skillId = _skillId;
        skillAniName = skillAction.Actions[skillId].SkillInfo.skill_AniId;
        moveSkill = skillAction.Actions[skillId].SkillInfo.skill_Bool_Move;

        skillAction.Actions[skillId].IsCool = true;


        if (_skillId == (int)DesignEnum.attack_Id.Spirit_Attack1 || _skillId == (int)DesignEnum.attack_Id.Spirit_Attack2)
            ActiveSpirit(false);
    }

    public override void StartCombo()
    {

    }

    public override void EndCombo()
    {
        if (nowComboCount < (int)DesignEnum.attack_Id.Common_Attack3)
            nowComboCount++;
        else
            nowComboCount = (int)DesignEnum.attack_Id.Common_Attack1;
    }
}
