using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

//結果やhandの表示を行う
//とりあえずTMPで設定しておく
//あーBanarProfile的なのつ来るか？
[RequireComponent(typeof(Animator))]
public class BanarConductor : AnimationConductor
{
    BanarAnimation animator;


    [SerializeField] BanarProfile[] aiko;
    [SerializeField] BanarProfile negativeEntangle;
    [SerializeField] BanarProfile poitiveEntangle;

    void Start()
    {
        var animator = GetComponent<Animator>();
        this.animator = new BanarAnimation(animator);
    }

    public override IJankenAnimation GetAnimation(AnimationType type)
    {
        var anime = animator.GetTriggerAction(type.ToString());

        return anime;
    }

    public void SetBanar(JankenResult name)
    {
        if (name == JankenResult.NEntangle)
        {
            animator.LoadProfile(negativeEntangle);
        }
        else if (name == JankenResult.PEntangle)
        {
            animator.LoadProfile(poitiveEntangle);
        }
        else if (name == JankenResult.aiko)
        {
            var rand = UnityEngine.Random.Range(0,aiko.Length);
            animator.LoadProfile(aiko[rand]);
        }
        else
        {
            Debug.LogWarning("banar of " + name + "not found!");
        }
    }



    [Serializable]
    class BanarProfile
    {
        [SerializeField] AnimatorOverrideController _controller;
        public AnimatorOverrideController overrideController { get { return _controller; } }
    }

    class BanarAnimation : AnimatorJankenateExchanger
    {

        public BanarAnimation(Animator animator) : base(animator)
        { }
        public void LoadProfile(BanarProfile profile)
        {
            animator.runtimeAnimatorController = profile.overrideController;
        }
    }
}