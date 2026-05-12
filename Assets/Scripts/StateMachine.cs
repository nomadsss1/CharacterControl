using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public IEntity Entity { get; private set; }
    public bool IsInitialization { get; private set; }
    public List<State> registeredStateList = new List<State>();
    public List<State> preRegisteredStateList = new List<State>();
    public State startState;
    public State currentState;
    public State lastState;

    public void Initialize(IEntity entity)
    {
        Entity = entity;
        IsInitialization = true;
        preRegisteredStateList.ForEach(state => RegisterState(state));
        if(startState != null)
        {
            SwitchState(startState.stateName);
        }
    }
    public bool RegisterState(State state)
    {
        if(state == null)
        {
            Debug.LogError("State is null");
            return false;
        }
        if(!IsInitialization || registeredStateList.Exists(x => x.stateName == state.stateName))
        {
            Debug.LogError("注册失败！状态未初始化 |" + state.stateName + "已存在");
            return false;
        }

        if(true)//判断是否需要注册
        {
            registeredStateList.Add(state);
            state.Register(this);
            return true;
        }
        else return false;
    }
    public bool UnRegisterState(State state)
    { 
        state.Unregister();
        registeredStateList.Remove(state);
        return true; 
    }

    public bool SwitchState(string targetStateName, params object[] enterParams)
    {
        if(!IsInitialization)
        {
            Debug.LogError("StateMachine未初始化");
            return false;
        }
        State state = registeredStateList.Find(x => x.stateName == targetStateName);
        if(state == null)
        {
            Debug.LogError("State not found: " + targetStateName);
            return false;
        }
        if(true)//是否顺利切换
        {
            currentState?.Exit();
            lastState = currentState;
            currentState = state;
            currentState.Enter(enterParams);
            return true;
        }
        else return false;
        
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if(!IsInitialization) return;
        currentState?.StateUpedate();
    }
    private void FixedUpdate()
    {
        if(!IsInitialization) return;
        currentState?.StateFixedUpedate();
    }
    private void LateUpdate()
    {
        if(!IsInitialization) return;
        currentState?.StateLateUpedate();
    }

    
}