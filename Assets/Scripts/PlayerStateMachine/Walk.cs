using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Walk", menuName = "States/Walk")]
public class Walk : State
{
    private void OnEnable() { stateName = "Walk"; }

    public float walkVelocity;
    public Animation walkAnimation;
    public override void Register(StateMachine stateMachine)
    {
        base.Register(stateMachine);
        
    }
    public override void Enter(params object[] enterParams)
    {
        Debug.Log("进入Walk状态");
        //播放walk动画
        //walkAnimation.Play(sm.lastState.stateName == "jump"? "jump_Landing" : "walk");
    }
    public override void StateUpedate()
    {
        Debug.Log("Walk状态更新");
        if(Control.instance.m_isGround == false)
        {
            sm.SwitchState("Fall");
            return;
        }
        else
        {
            if(GoJump()) return;
            if(NotMove()) return;
        }
    }
    public override void StateFixedUpedate()
    {
        //if(Move()) return;
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

    private bool NotMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if(horizontalInput < float.Epsilon)
        {
            sm.SwitchState("Idle");
            // Vector3 movement = new Vector3(horizontalInput, 0, 0);
            // entityTransform.position += movement * velocity * Time.deltaTime;
            return true;
        }
        else return false;
    }
    // private bool Move()
    // {
    //     // float horizontalInput = Input.GetAxis("Horizontal");
    //     // if(horizontalInput != 0)
    //     // {
    //     //     sm.SwitchState("Walk");
    //     //     // Vector3 movement = new Vector3(horizontalInput, 0, 0);
    //     //     // entityTransform.position += movement * velocity * Time.deltaTime;
    //     //     return true;
    //     // }
    //     // else return false;
    // }
}
