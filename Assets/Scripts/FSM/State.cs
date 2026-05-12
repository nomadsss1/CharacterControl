using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class State : ScriptableObject
{
    public string stateName;
    protected StateMachine sm;
    protected IEntity entity => sm.Entity;
    protected Transform entityTransform => sm.Entity.transform;

    public virtual void StateUpedate(){}
    public virtual void StateFixedUpedate(){}
    public virtual void StateLateUpedate(){}
    public virtual void Register(StateMachine stateMachine)
    {
        sm = stateMachine;
    }
    public virtual void Unregister()
    {
        sm = null;
    }
    public virtual void Enter(params object[] enterParams) { }
    public virtual void Exit() { }
}

public interface IEntity
{
    Transform transform { get; }
}


