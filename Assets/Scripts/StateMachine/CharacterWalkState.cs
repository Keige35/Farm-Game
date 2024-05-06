﻿using UnityEngine;

public class CharacterWalkState : State
{
    protected readonly CharacterAnimationController _characterAnimationController;
    protected readonly float _speed;
    private readonly float _speedRotate;
    protected readonly Rigidbody _characterBody;
    protected readonly InputController _inputController;

    public CharacterWalkState(CharacterAnimationController characterAnimationController, float speed, float speedRotate,
        Rigidbody characterBody, InputController inputController)
    {
        _characterAnimationController = characterAnimationController;
        _speed = speed;
        _speedRotate = speedRotate;
        _characterBody = characterBody;
        _inputController = inputController;
    }

    public override void OnStateEnter()
    {
        _characterAnimationController.SetBool(CharacterAnimationType.Walk, true);
    }

    public override void OnStateExit()
    {
        _characterAnimationController.SetBool(CharacterAnimationType.Walk, false);
        _characterBody.velocity = Vector3.zero;
    }

    public override void OnFixedUpdate(float fixedDeltaTime)
    {
        Move();
        Rotate();
    }

    protected virtual void Move()
    {
        _characterBody.velocity = _inputController.MoveDirection * _speed;
    }

    private void Rotate()
    {
        var charForward = _characterBody.transform.forward;
        var newDirection = Vector3.RotateTowards(charForward, _inputController.MoveDirection, _speed, 0.0f);
        _characterBody.rotation =
            Quaternion.Lerp(_characterBody.transform.rotation, Quaternion.LookRotation(newDirection),
                _speedRotate * Time.deltaTime);
    }
}

public class CharacterRunState : CharacterWalkState
{
    private float runFloat;

    public CharacterRunState(CharacterAnimationController characterAnimationController, float speed, float speedRotate,
        Rigidbody characterBody, InputController inputController) : base(characterAnimationController, speed,
        speedRotate, characterBody, inputController)
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (runFloat < 1)
        {
            runFloat += deltaTime;
        }
        else
        {
            runFloat = 1;
        }

        _characterAnimationController.SetFloat(CharacterAnimationType.RunFloat, runFloat);
    }

    protected override void Move()
    {
        _characterBody.velocity = _inputController.MoveDirection * _speed * 2;
    }

    public override void OnStateEnter()
    {
        _characterAnimationController.SetBool(CharacterAnimationType.Walk, true);
    }

    public override void OnStateExit()
    {
        _characterAnimationController.SetBool(CharacterAnimationType.Walk, false);
        _characterBody.velocity = Vector3.zero;

        runFloat = 0;
        _characterAnimationController.SetFloat(CharacterAnimationType.RunFloat, runFloat);
    }
}