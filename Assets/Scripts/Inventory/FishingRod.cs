using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : PlayerHandItem
{
    private InputController inputController;
    private CharacterAnimationController characterAnimationController;

    private void Update()
    {
        inputController ??= FindObjectOfType<InputController>();
        characterAnimationController ??= FindObjectOfType<CharacterStateMachine>().CharacterAnimationController;
        if (IsSelectable == false)
        {
            characterAnimationController.SetBool(CharacterAnimationType.FishingCast, inputController.IsAttack);
        }
    }

    public override void ItemDeSelected()
    {
        base.ItemDeSelected();
        characterAnimationController.SetBool(CharacterAnimationType.FishingCast, false);
    }
}
