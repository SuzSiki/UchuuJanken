using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThreadPanel:MonoBehaviour
{
    Image panel;
    IJankenAnimation openAnimation;
    IJankenAnimation closeAnimation;
    ThreadPanel[] siblings;

    void Start()
    {
        var animator = GetComponent<Animator>();
        var jAnimator = new AnimatorJankenateExchanger(animator);
        openAnimation = jAnimator.GetTriggerAction("Enter");
        closeAnimation = jAnimator.GetTriggerAction("Exit");
        siblings = transform.parent.GetComponentsInChildren<ThreadPanel>();

        TitleManager.instance.onTitleEvent += (x) =>
        {
            if(x == TitleState.onEyeCatch)
            {
                closeAnimation.Play(BattleAnimKantoku.audioSource);
            }
        };
    }

    public void Hide()
    {
        closeAnimation.Play(BattleAnimKantoku.audioSource);
    }

    public void Show()
    {
        openAnimation.Play(BattleAnimKantoku.audioSource);
    }

    public void ComeUp()
    {
        Debug.Log(name);
        transform.SetSiblingIndex(siblings.Length);
    }
}
