using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System;

[RequireComponent(typeof(Image))]
public class FaderAnimationConductor : AnimationConductor
{
    Dictionary<AnimationType, IJankenAnimation> animations { get; set; }
    new Image renderer;
    AnimatorJankenateExchanger animator;

    void Start()
    {
        renderer = GetComponent<Image>();
        Initialize();
    }

    void Initialize()
    {
        animations = new Dictionary<AnimationType, IJankenAnimation>();
        animator = new AnimatorJankenateExchanger(GetComponent<Animator>());
        //animations[AnimationType.Enter] = new FaderAnimation(renderer, AnimationType.Enter);
        //animations[AnimationType.Flash] = new FaderAnimation(renderer, AnimationType.Flash);
        //animations[AnimationType.Exit] = new FaderAnimation(renderer, AnimationType.Exit);
    }

    public override IJankenAnimation GetAnimation(AnimationType type)
    {
        return animator.GetTriggerAction(type.ToString());
    }

    class FaderAnimation : NormalJankenAnimation
    {
        AnimationType type;
        AudioClip clip;
        Tween tween;
        Image targetRenderer;

        public FaderAnimation(Image renderer, AnimationType type, AudioClip clip = null)
        {
            this.type = type;
            this.clip = clip;
            targetRenderer = renderer;
        }

        void Initialize()
        {
            switch (type)
            {
                case AnimationType.Enter:
                    tween = targetRenderer.DOFade(1, duration);
                    break;

                case AnimationType.Exit:
                    tween = targetRenderer.DOFade(0, duration);
                    break;

                case AnimationType.Flash:
                    var sq = DOTween.Sequence();
                    sq.Append(targetRenderer.DOFade(1, duration * 0.75f));
                    sq.Append(targetRenderer.DOFade(0, duration * 0.25f));
                    tween = sq;
                    break;
            }

            tween.onComplete += () =>
            {
                compleate = true;
            };

        }


        public override void SetUp(float timescale)
        {
            tween.timeScale = timescale;
        }

        public override void Play(AudioSource audio)
        {
            compleate = false;


            tween.Play();
        }

    }

}