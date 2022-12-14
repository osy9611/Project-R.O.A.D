using DesignEnum;
using Sword;
using Sword.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static Define;
public class MyPlayerController : Controller
{
    float cameraHeight;
    float cameraDistance;
    float cameraSpeed;

    public bool moved;
    Vector3 prevPosition;
    Vector3 prevRotation;

    bool prevNaveState;

    public override void Init()
    {
        PlayerActor playerActor = actor as PlayerActor;
        nowSpeed = speed = playerActor.CharData.char_Move_Speed;
    }

    public override void Execute()
    {
        if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
        {
            actor.GetSkillAgent().OnAttack((int)DesignEnum.attack_Id.Skill_Attack1);
        }
        if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
        {
            actor.GetSkillAgent().OnAttack((int)DesignEnum.attack_Id.Skill_Attack2);
        }
        if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
        {
            actor.GetSkillAgent().OnAttack((int)DesignEnum.attack_Id.Skill_Attack3);
        }
        if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
        {
            actor.GetSkillAgent().OnAttack((int)DesignEnum.attack_Id.Skill_Attack4);
        }
        //Debug.LogWarning(actor.GetFSM().CurrentObjectState);
        //Debug.LogWarning(actor.GetSkillAgent().SkillAniName);

    }

    public override void LateExecute()
    {

    }

    public void SetCamera(float height, float distance, float speed)
    {
        cameraHeight = height;
        cameraDistance = distance;
        cameraSpeed = speed;
    }

    public override void CheckDir()
    {
        if (actor.GetFSM().CurrentObjectState == ObjectState.Death || actor.GetFSM().CurrentObjectState == ObjectState.Spawn)
            return;

        PlayerFSM fsm = actor.GetFSM() as PlayerFSM;

        if (actor.GetSkillAgent().MoveSkill)
        {
            nowSpeed = 0;
        }
        else
        {
            nowSpeed = speed;
        }

    }

    public bool CheckMoveable()
    {
        return actor.GetFSM().CurrentObjectState != ObjectState.Death && actor.GetFSM().CurrentObjectState != ObjectState.Spawn;
    }


    protected override void Move()
    {
        float delta = Time.deltaTime * nowSpeed;
        actor.CharCon.Move(new Vector3(actor.Dir.x, gravity, actor.Dir.z) * delta);
    }


    public override void CheckUpdateFlag()
    {
        if (Self.IsSinglePlayMap())
            return;

        if (nowFlagTime <= Define.checkFlagTime)
        {
            nowFlagTime += Time.deltaTime;

        }
        else
        {
            nowFlagTime = 0;
            PlayerActor playerActor = actor as PlayerActor;

           

            if (playerActor.IsAuto && actor.Dir==Vector3.zero)
            {
                if (actor.Creature.transform.position != prevPosition)
                {
                    RequestOperations.Move(Self.Peer, actor.Creature.transform.position,
                                           actor.Creature.transform.forward, true);
                    prevPosition = actor.Creature.transform.position;
                }
                
                if(!actor.Creature.NavAgent.enabled && !prevNaveState)
                {
                    RequestOperations.Move(Self.Peer, actor.Creature.transform.position,
                                      Vector3.zero, true);
                    prevNaveState = actor.Creature.NavAgent.enabled;
                }
                else if(actor.Creature.NavAgent.enabled)
                {
                    prevNaveState = false;
                }

                return;
            }

            if (actor.Creature.transform.position != prevPosition)
            {
                RequestOperations.Move(Self.Peer, actor.Creature.transform.position,
                                       actor.Dir, true);
                prevPosition = actor.Creature.transform.position;
            }

            if (actor.Dir == Vector3.zero && prevRotation == Vector3.zero)
            {
                RequestOperations.Move(Self.Peer, actor.Creature.transform.position,
                                       Vector3.zero, true);
                prevRotation = actor.Creature.transform.forward;
            }
            else if (actor.Dir != Vector3.zero)
            {
                prevRotation = Vector3.zero;
            }

        }
    }

    public void SetNavAgent(bool isActive)
    {
        if (actor.Dir != Vector3.zero)
            return;

        if (isActive)
        {
            actor.Creature.NavAgent.destination = actor.GetSkillAgent().AttackTarget.Creature.transform.position;
            actor.Creature.NavAgent.speed = nowSpeed;
        }

        actor.Creature.NavAgent.enabled = isActive;
        actor.CharCon.enabled = !isActive;
      
        
    }
}
