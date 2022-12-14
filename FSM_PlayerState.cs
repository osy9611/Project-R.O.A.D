using Sword;
using Sword.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM_PlayerState
{
    public class Idle : State<BaseActor>
    {
        ObjectSkillAgent skillAgent;
        PlayerActor actor = null;
        int skillId = -1;
        public override void Enter(BaseActor entity)
        {
            if (entity == null)
                return;

            Managers.Ani.PlayAnimation(entity.Ani, "Idle_3", true);

            if (skillAgent == null)
            {
                skillAgent = entity.GetSkillAgent() as ObjectSkillAgent;
            }

            if (actor == null && !Managers.Object.CheckMyActor(entity))
                return;

            actor = Managers.Object.MyActor as PlayerActor;
        }

        public override void Execute(BaseActor entity)
        {
            if (skillAgent == null)
                return;

            BaseActor target = skillAgent.AttackTarget;
            MyPlayerController controller = entity.GetController() as MyPlayerController;
            if (entity.GetSkillAgent().SkillAniName == "")
            {
                if (entity.Dir != Vector3.zero)
                    entity.GetFSM().ChangeState(Define.ObjectState.Move);

                if (CheckSelfAndTarget(entity))
                {
                    if (!actor.IsAuto)
                    {
                        entity.GetSkillAgent().OnAttack(entity.GetSkillAgent().NowComboCount);
                    }

                    if (actor.IsAuto && entity.Dir == Vector3.zero)
                    {
                        skillId = -1;
                        for (int i = (int)DesignEnum.attack_Id.Skill_Attack1; i < (int)DesignEnum.attack_Id.Skill_Attack4; ++i)
                        {
                            if (!skillAgent.CheckSkillCool(i))
                            {
                                skillId = i;
                                entity.GetSkillAgent().SkillAttackRange = entity.GetSkillAgent().GetSkillAttackRange(skillId);

                                break;
                            }
                        }

                        if (skillId == -1)
                        {
                            skillId = entity.GetSkillAgent().NowComboCount;
                            entity.GetSkillAgent().SkillAttackRange = entity.GetSkillAgent().GetSkillAttackRange(skillId);
                        }

                        if (Vector3.Distance(target.Creature.transform.position, entity.Creature.transform.position) <= entity.GetSkillAgent().SkillAttackRange)
                        {
                            if (controller != null)
                                controller.SetNavAgent(false);


                            entity.GetSkillAgent().OnAttack(skillId);
                        }
                        else
                        {
                            if (controller != null)
                                controller.SetNavAgent(true);
                            entity.GetFSM().ChangeState(Define.ObjectState.Move);
                        }
                    }

                }
                if (!CheckSelfAndTarget(entity))
                {
                    if (controller != null)
                        controller.SetNavAgent(false);
                }
            }
            else
            {
                if (entity.GetSkillAgent().CheckIsSkill() == null)
                    return;

                if (entity.GetSkillAgent().CheckIsSkill() == true)
                {
                    entity.GetFSM().ChangeState(Define.ObjectState.Skill);
                }
                else
                {
                    entity.GetFSM().ChangeState(Define.ObjectState.Attack);
                }

            }
        }

        public override void Exit(BaseActor entity)
        {
        }

        private bool CheckSelfAndTarget(BaseActor actor)
        {
            if (Managers.Object.CheckMyActor(actor) && skillAgent.AttackTarget != null)
            {
                Vector3 dir = (skillAgent.AttackTarget.Creature.transform.position - actor.Creature.transform.position).normalized;
                actor.Creature.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

                return true;
            }
            return false;
        }
    }

    public class Move : State<BaseActor>
    {
        public override void Enter(BaseActor entity)
        {
            if (entity != null)
                Managers.Ani.PlayAnimation(entity.Ani, "Move", true);

            if (Managers.Object.MyActor == entity)
            {
                MyPlayerController controller = (MyPlayerController)entity.GetController();

                if (controller != null)
                {
                    controller.moved = true;
                }
            }
        }

        public override void Execute(BaseActor entity)
        {
            if (Managers.Object.MyActor == entity)
            {
                PlayerActor actor = entity as PlayerActor;
                if (actor.IsAuto && entity.Dir == Vector3.zero)
                {
                    BaseActor target = entity.GetSkillAgent().AttackTarget;

                    // modified <최승환> : target이 null인 경우가 있음.
                    if (target == null) return;

                    if (Vector3.Distance(target.Creature.transform.position, entity.Creature.transform.position) <= Define.DummyAttackRange)
                    {
                        entity.GetFSM().ChangeState(Define.ObjectState.Idle);
                        return;
                    }
                    else
                    {
                        MyPlayerController controller = entity.GetController() as MyPlayerController;
                        if (controller != null)
                            controller.SetNavAgent(true);
                    }
                }
                else
                {
                    MyPlayerController controller = entity.GetController() as MyPlayerController;
                    if (controller != null)
                        controller.SetNavAgent(false);
                    if (entity.Dir == Vector3.zero || entity.GetSkillAgent().SkillAniName != "")
                    {
                        entity.GetFSM().ChangeState(Define.ObjectState.Idle);
                        return;
                    }
                }
            }
            else
            {
                if (entity.Dir == Vector3.zero || entity.GetSkillAgent().SkillAniName != "")
                {
                    entity.GetFSM().ChangeState(Define.ObjectState.Idle);
                    return;
                }
            }
        }

        public override void Exit(BaseActor entity)
        {
        }
    }

    public class Attack : State<BaseActor>
    {
        PlayerActor actor = null;
        public override void Enter(BaseActor entity)
        {
            if (entity != null)
            {
                Managers.Ani.PlayAnimation(entity.Ani, entity.GetSkillAgent().SkillAniName, true);
            }

            if (actor == null && !Managers.Object.CheckMyActor(entity))
                return;

            actor = Managers.Object.MyActor as PlayerActor;
        }

        public override void Execute(BaseActor entity)
        {
            if (entity == null)
                return;

            if (entity.Dir != Vector3.zero)
            {
                entity.GetSkillAgent().ResetData(true);
                entity.GetFSM().ChangeState(Define.ObjectState.Move);
            }

            if (!Managers.Ani.CheckPlayAnimationName(entity.Ani, entity.GetSkillAgent().SkillAniName)
                    && entity.GetSkillAgent().SkillAniName != "")
            {
                entity.GetFSM().ChangeState(Define.ObjectState.Attack);
                return;
            }

            if (entity.GetFSM().AniEnd)
            {
                if (Managers.Object.CheckMyActor(entity))
                {
                    if (entity.GetSkillAgent().AttackTarget == null)
                        entity.GetSkillAgent().ResetData(true);
                    else
                    {
                        entity.GetSkillAgent().ResetData();
                    }
                }
                else
                {
                    entity.GetSkillAgent().ResetData(true);
                }

                entity.GetFSM().ChangeState(Define.ObjectState.Idle);
            }
        }

        public override void Exit(BaseActor entity)
        {

        }
    }

    public class Skill : State<BaseActor>
    {
        PlayerActor actor = null;
        public override void Enter(BaseActor entity)
        {
            if (entity == null)
                return;

            Managers.Ani.PlayAnimation(entity.Ani, entity.GetSkillAgent().SkillAniName, true);

            if (actor == null && !Managers.Object.CheckMyActor(entity))
                return;

            actor = Managers.Object.MyActor as PlayerActor;
        }

        public override void Execute(BaseActor entity)
        {
            if (!Managers.Ani.CheckPlayAnimationName(entity.Ani, entity.GetSkillAgent().SkillAniName))
            {
                entity.GetFSM().ChangeState(Define.ObjectState.Skill);
            }

            if (entity.GetFSM().AniEnd)
            {
                if (Managers.Object.CheckMyActor(entity))
                {
                    if (entity.GetSkillAgent().AttackTarget == null)
                    {
                        entity.GetSkillAgent().ResetData(true);
                    }
                    else
                    {
                        entity.GetSkillAgent().ResetData();
                    }
                }

                if (!Managers.Object.CheckMyActor(entity))
                {
                    entity.GetSkillAgent().ResetData(true);
                    entity.Dir = Vector3.zero;
                }

                entity.GetFSM().AniEnd = false;
                entity.GetFSM().ChangeState(Define.ObjectState.Idle);
            }
        }

        public override void Exit(BaseActor entity)
        {
            entity.HitActor = null;
        }

    }

    public class Death : State<BaseActor>
    {
        public override void Enter(BaseActor entity)
        {
            if (entity == null)
                return;
            entity.GetSkillAgent().ResetData();
            Managers.Ani.PlayAnimation(entity.Ani, "Death", true);

            if (Managers.Object.CheckMyActor(entity))
            {
                MyPlayerController controller = (MyPlayerController)entity.GetController();
                if (controller != null)
                {
                    controller.moved = false;
                }
            }

        }

        public override void Execute(BaseActor entity)
        {
        }

        public override void Exit(BaseActor entity)
        {

        }
    }

    public class Spawn : State<BaseActor>
    {
        public override void Enter(BaseActor entity)
        {
            if (entity == null)
                return;
            Managers.Ani.PlayAnimation(entity.Ani, "Spawn", true);

        }

        public override void Execute(BaseActor entity)
        {
            if (entity == null)
                return;

            MyPlayerController controller = null;
            if (Managers.Object.CheckMyActor(entity))
            {
                controller = (MyPlayerController)entity.GetController();
            }


            if (entity.GetFSM().AniEnd)
            {
                entity.GetSkillAgent().ResetData(true);
                entity.GetFSM().ChangeState(Define.ObjectState.Idle);
            }
        }
        public override void Exit(BaseActor entity)
        {

        }
    }
}