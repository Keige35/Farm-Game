using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Unity.VisualScripting;

public class NpcBirdStateMachine : Damageable
{
    [SerializeField] private Transform floorPosition;
    [SerializeField] private Vector2 size;

    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private BirdWalkState walkState;
    private BirdRunState runState;
    private BirdIdleState idleState;
    private BirdDeadState deadState;
    private BirdEatState eatState;
    private BirdFlyIdleState flyIdleState;
    private BirdAfterFlyState afterFlyState;
    

    private Vector3 startPosition;

    private Transform player;

    private StateMachine stateMachine;
    
   
    
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
       // changedHealth?.Invoke(currentHealth, maxHealth, gameObject);

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

        deadState = new BirdDeadState(npcAnimatorController, this, transform);
        idleState = new BirdIdleState(npcAnimatorController);
        walkState = new BirdWalkState(npcAnimatorController, navMeshAgent);
        runState = new BirdRunState(npcAnimatorController, navMeshAgent);
        eatState = new BirdEatState(npcAnimatorController, this);
        

        runState.AddTransition(new StateTransition(idleState, new FuncCondition(() => runState.IsOnPosition)));

        idleState.AddTransition(new StateTransition(walkState, new FuncCondition(() =>
        {
            UpdateEatPosition();
            return true;
        })));

        eatState.AddTransition(new StateTransition(walkState, new TemporaryCondition(5)));
        
       // flyIdleState.AddTransition(new StateTransition(runState, new TemporaryCondition(4)));
 //       afterFlyState.AddTransition(new StateTransition(flyIdleState, new TemporaryCondition(4)));
  //      idleState.AddTransition(new StateTransition(afterFlyState,new TemporaryCondition(1)));

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

public class BirdWalkState : State
{
    private readonly NpcAnimationController npcAnimationController;
    private readonly NavMeshAgent navMeshAgent;

    private Vector3 endPosition;
    private Transform endTransform;

    private const float stopDistance = 1;

    public bool IsOnPosition => CheckPosition();

    private bool CheckPosition()
    {
        if (endTransform)
        {
            var isOnPosition = Vector3.Distance(navMeshAgent.transform.position, endTransform.position) <= stopDistance;
            return isOnPosition;
        }
        else
        {
            var isOnPosition = Vector3.Distance(navMeshAgent.transform.position, endPosition) <= stopDistance;
            return isOnPosition;
        }
    }


    public BirdWalkState(NpcAnimationController npcAnimationController, NavMeshAgent navMeshAgent)
    {
        this.npcAnimationController = npcAnimationController;
        this.navMeshAgent = navMeshAgent;
    }

    public void SetPosition(Vector3 endPosition)
    {
        this.endPosition = endPosition;
    }

    public void SetPosition(Transform endTransform)
    {
        this.endTransform = endTransform;
    }

    public override void OnUpdate(float deltaTime)
    {
        navMeshAgent.SetDestination(endTransform == null ? endPosition : endTransform.position);
    }

    public override void OnStateEnter()
    {
        navMeshAgent.enabled = true;
        npcAnimationController.SetBool(NpcAnimationType.Walk, true);
    }

    public override void OnStateExit()
    {
        navMeshAgent.enabled = false;
        endPosition = Vector3.zero;
        endTransform = null;

        npcAnimationController.SetBool(NpcAnimationType.Walk, false);
    }
}

public class BirdRunState : State
{
    private readonly NpcAnimationController npcAnimationController;
    private readonly NavMeshAgent navMeshAgent;

    private Vector3 endPosition;
    private Transform endTransform;

    private const float stopDistance = 1;

    public bool IsOnPosition => CheckPosition();

    private bool CheckPosition()
    {
        if (endTransform)
        {
            var isOnPosition = Vector3.Distance(navMeshAgent.transform.position, endTransform.position) <= stopDistance;
            return isOnPosition;
        }
        else
        {
            var isOnPosition = Vector3.Distance(navMeshAgent.transform.position, endPosition) <= stopDistance;
            return isOnPosition;
        }
    }


    public BirdRunState(NpcAnimationController npcAnimationController, NavMeshAgent navMeshAgent)
    {
        this.npcAnimationController = npcAnimationController;
        this.navMeshAgent = navMeshAgent;
    }

    public void SetPosition(Vector3 endPosition)
    {
        this.endPosition = endPosition;
    }

    public void SetPosition(Transform endTransform)
    {
        this.endTransform = endTransform;
    }

    public override void OnUpdate(float deltaTime)
    {
        navMeshAgent.SetDestination(endTransform == null ? endPosition : endTransform.position);
    }

    public override void OnStateEnter()
    {
        navMeshAgent.speed += 1.5f;
        navMeshAgent.enabled = true;
        npcAnimationController.SetBool(NpcAnimationType.Run, true);
    }

    public override void OnStateExit()
    {
        navMeshAgent.speed -= 1.5f;
        navMeshAgent.enabled = false;
        endPosition = Vector3.zero;
        endTransform = null;

        npcAnimationController.SetBool(NpcAnimationType.Run, false);
    }
}

public class BirdIdleState : State
{
    private readonly NpcAnimationController npcAnimationController;

    public BirdIdleState(NpcAnimationController npcAnimationController)
    {
        this.npcAnimationController = npcAnimationController;
    }

    public override void OnStateEnter()
    {
        npcAnimationController.SetBool(NpcAnimationType.Idle, true);
    }

    public override void OnStateExit()
    {
        npcAnimationController.SetBool(NpcAnimationType.Idle, false);
    }
}

public class BirdEatState : State
{
    private readonly NpcAnimationController npcAnimationController;
    private readonly NpcBirdStateMachine npcBirdStateMachine;

    public BirdEatState(NpcAnimationController npcAnimationController, NpcBirdStateMachine npcBirdStateMachine)
    {
        this.npcAnimationController = npcAnimationController;
        this.npcBirdStateMachine = npcBirdStateMachine;
    }

    public override void OnStateEnter()
    {
        npcAnimationController.SetBool(NpcAnimationType.Eat, true);
    }

    public override void OnStateExit()
    {
        npcAnimationController.SetBool(NpcAnimationType.Eat, false);
        npcBirdStateMachine.UpdateEatPosition();
    }
}

public class BirdDeadState : State
{
    private readonly NpcAnimationController npcAnimationController;
    private readonly IPooled pooled;
    private readonly Transform npc;

    private const float deadTime = 1f;
    private float currentTime;

    public BirdDeadState(NpcAnimationController npcAnimationController, IPooled pooled, Transform npc)
    {
        this.npcAnimationController = npcAnimationController;
        this.pooled = pooled;
        this.npc = npc;
    }

    public override void OnUpdate(float deltaTime)
    {
        currentTime -= deltaTime;
        if (currentTime <= 0)
        {
            npc.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                npc.transform.localScale = Vector3.one;
                pooled.ReturnToPool();
            });
        }
    }

    public override void OnStateEnter()
    {
        currentTime = deadTime;
        npcAnimationController.SetTrigger(NpcAnimationType.Dead);
    }
}

public class BirdFlyIdleState : State
{
    private readonly NpcAnimationController npcAnimationController;
    private GameObject bird;
    private Vector3 endPosition;
    private Transform endTransform;
    
   


    public BirdFlyIdleState(NpcAnimationController npcAnimationController)
    {
        this.npcAnimationController = npcAnimationController;
    }

    public void SetPosition(Vector3 endPosition)
    {
        this.endPosition = endPosition;
    }

    public void SetPosition(Transform endTransform)
    {
        this.endTransform = endTransform;
    }

    public override void OnUpdate(float deltaTime)
    {
        bird.transform.Translate(0,10,0);
    }

    public override void OnStateEnter()
    {
        npcAnimationController.SetBool(NpcAnimationType.Fly, true);
    }

    public override void OnStateExit()
    {
        npcAnimationController.SetBool(NpcAnimationType.Fly, false);
    }
    
}

public class BirdAfterFlyState : State
{
    private readonly NpcAnimationController npcAnimationController;
    
    
}