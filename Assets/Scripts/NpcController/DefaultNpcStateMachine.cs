using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefaultNpcStateMachine : Damageable
{
    [SerializeField] private Transform floorPosition;
    [SerializeField] private Vector2 size;

    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private NpcWalkState walkState;
    private NpcRunState runState;
    private NpcDeadState deadState;

    private Vector3 startPosition;

    private Transform player;

    private StateMachine stateMachine;

    public static Action<int, int, GameObject> changedHealth;
    private void Update()
    {
        stateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.OnFixedUpdate();
    }

    protected override void HealthUpdated()
    {
        changedHealth?.Invoke(currentHealth, maxHealth, gameObject);

        if (stateMachine.CurrentState == deadState)
        {
            return;
        }

        ItemSpawner.Instance.SpawnEffect(EffectType.Hit, transform.position);
        UpdateRunState();
        if (currentHealth > 0)
        {
            return;
        }

        stateMachine.SetState(deadState);
    }

    public override void Initialize()
    {
        base.Initialize();
        startPosition = floorPosition.position;
        InitializeStateMachine();
    }

    private void UpdateRunState()
    {
        player ??= GameObject.FindGameObjectWithTag("Player").transform;
        var endPosition = transform.position + player.forward * 5;
        runState.SetPosition(endPosition);
        stateMachine.SetState(runState);
    }

    private void InitializeStateMachine()
    {
        var npcAnimatorController = new NpcAnimationController(animator);

        deadState = new NpcDeadState(npcAnimatorController, this, transform);
        var idleState = new NpcIdleState(npcAnimatorController);
        walkState = new NpcWalkState(npcAnimatorController, navMeshAgent);
        runState = new NpcRunState(npcAnimatorController, navMeshAgent);
        var eatState = new NpcEatState(npcAnimatorController, this);

        runState.AddTransition(new StateTransition(idleState, new FuncCondition(() => runState.IsOnPosition)));

        idleState.AddTransition(new StateTransition(walkState, new FuncCondition(() =>
        {
            UpdateEatPosition();
            return true;
        })));

        eatState.AddTransition(new StateTransition(walkState, new TemporaryCondition(5)));

        walkState.AddTransition(new StateTransition(eatState, new FuncCondition(() => walkState.IsOnPosition)));
        stateMachine = new StateMachine(idleState);
    }

    public void UpdateEatPosition()
    {
        var randomX = UnityEngine.Random.Range(-size.x / 2f, size.x / 2f);
        var randomZ = UnityEngine.Random.Range(-size.y / 2f, size.y / 2f);
        var newPosition = startPosition + new Vector3(randomX, 0, randomZ);
        walkState.SetPosition(newPosition);
    }


    private void OnDrawGizmos()
    {
        if (floorPosition == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(floorPosition.position, new Vector3(size.x, 0.2f, size.y));
    }
}