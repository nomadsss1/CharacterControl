using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "States/Idle")]
public class Idle : State
{
    private void OnEnable() { stateName = "Idle"; }

    public float idleVelocity;
    public Animation idleAnimation;
    public override void Register(StateMachine stateMachine)
    {
        base.Register(stateMachine);
        idleVelocity = 0f;
    }
    public override void Enter(params object[] enterParams)
    {
        Debug.Log("进入Idle状态");
        //播放idle动画
        //idleAnimation.Play(sm.lastState.stateName == "jump"? "jump_Landing" : "idle");
    }
    public override void StateUpedate()
    {
        Debug.Log("Idle状态更新");
        if(GoJump()) return;
        Move();
    }
    public override void StateFixedUpedate()
    {
        if(Move()) return;
    }
    private bool GoJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            sm.SwitchState("jump");
            return true;
        }
        return false;
    }
    private bool Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if(horizontalInput != 0)
        {
            sm.SwitchState("Walk");
            // Vector3 movement = new Vector3(horizontalInput, 0, 0);
            // entityTransform.position += movement * velocity * Time.deltaTime;
            return true;
        }
        else return false;
    }
}
