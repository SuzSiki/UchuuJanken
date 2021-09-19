using UnityEngine;
using System.Collections.Generic;

public class IntroAnimationConductor:AnimationConductor
{
    AnimatorJankenateExchanger animator;
    IJankenAnimation anime;
    void Start()
    {
        var animator = GetComponent<Animator>();
        this.animator = new AnimatorJankenateExchanger(animator);
        this.anime = this.animator.GetTriggerAction("Enter");
    }

    public override IJankenAnimation GetAnimation(AnimationType type)
    {
        return anime;
    }   

}