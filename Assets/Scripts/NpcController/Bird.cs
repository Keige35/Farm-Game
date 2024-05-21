using System.Xml;
using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;

public class Bird : PuduNpcStateMachine
{
 
    public override void Initialize()
    {
        base.Initialize();
        damagePercent = new List<float> {0f};
        startPosition = floorPosition.position;
        InitializeStateMachine();
    }
    protected override void InitializeStateMachine()
    {
        var npcAnimatorController = new NpcAnimationController(animator);

        deadState = new NpcDeadState(npcAnimatorController, this, transform);
        var idleState = new NpcIdleState(npcAnimatorController);
        walkState = new NpcWalkState(npcAnimatorController, navMeshAgent);
        runState = new NpcRunState(npcAnimatorController, navMeshAgent);
        var eatState = new NpcEatState(npcAnimatorController, this);
        var startFlyState = new NpcStartFlyState(npcAnimatorController, this.gameObject);
        var flyIdleState = new NpcFlyIdleState();
        var afterFlyState = new NpcAfterFlyState(npcAnimatorController, this.gameObject);


        runState.AddTransition(new StateTransition(startFlyState, new TemporaryCondition(4)));
        startFlyState.AddTransition(new StateTransition(flyIdleState, new FuncCondition(() => startFlyState.IsOnPosition)));
        flyIdleState.AddTransition(new StateTransition(afterFlyState, new TemporaryCondition(4)));
        afterFlyState.AddTransition(new StateTransition(idleState, new FuncCondition(() => afterFlyState.IsOnPosition)));
        idleState.AddTransition(new StateTransition(walkState, new FuncCondition(() =>
        {
            UpdateEatPosition();
            return true;
        })));

        eatState.AddTransition(new StateTransition(walkState, new TemporaryCondition(5)));

        walkState.AddTransition(new StateTransition(eatState, new FuncCondition(() => walkState.IsOnPosition)));
        stateMachine = new StateMachine(idleState);
    }
}