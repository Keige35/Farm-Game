using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAfterFlyState : State
{
    private readonly NpcAnimationController npcAnimationController;
    private Vector3 _endPosition;
    private GameObject _birdObject;
    private bool _isOnPosition;
    public bool IsOnPosition => CheckPosition();

    private bool CheckPosition()
    {
        return _isOnPosition;
    }


    public NpcAfterFlyState(NpcAnimationController npcAnimationController, GameObject birdObject)
    {
        this.npcAnimationController = npcAnimationController;
        this._birdObject = birdObject;
        this._endPosition = birdObject.transform.position + new Vector3(0, -10, 0);
    }
    public override void OnUpdate(float deltaTime)
    {
        if (_birdObject.transform.position.y >= 0.4f)
        {
            _birdObject.transform.position -= new Vector3(0, 0.01f, 0);
        }
        else _isOnPosition = true;
    }

    public override void OnStateEnter()
    {
        
    }

    public override void OnStateExit()
    {
        npcAnimationController.SetBool(NpcAnimationType.Fly, false);
    }
}

