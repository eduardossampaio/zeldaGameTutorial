using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAttackFinished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SendMessage("AttackDone", SendMessageOptions.DontRequireReceiver);
    }
}
