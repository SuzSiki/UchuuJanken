using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CutinConductor : AnimationConductor
{
    Dictionary<AnimationType, IJankenAnimation> animations { get;set;}

    [SerializeField] bool autoPrepareSprite = true;
    [SerializeField, Range(0, 1)] int _targetPlayer;
    public int targetPlayer { get { return _targetPlayer; } }
    [SerializeField] string triggerName = "Flash";
    AnimatorJankenateExchanger animator;
    [SerializeField] Image  tachieRenderer;

    void Start()
    {
        animations = new Dictionary<AnimationType, IJankenAnimation>();
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.viewInitialize)
            {
                StartCoroutine(Initialize());
            }
        };
    }

    public override IJankenAnimation GetAnimation(AnimationType type)
    {
        return animations[type];
    }

    IEnumerator Initialize()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);

        var tmp = GetComponent<Animator>();
        yield return null;
        animator = new AnimatorJankenateExchanger(tmp);
        yield return null;
        var anime = animator.GetTriggerAction(triggerName);

        if(autoPrepareSprite)
        {
            SetCharacter();
        }

        animations[AnimationType.Flash] = anime;


        task.compleate = true;
    }

    void SetCharacter()
    {
        var player = JankenManager.instance.nowSession.GetPlayer(targetPlayer);
        if (tachieRenderer != null)
        {
            tachieRenderer.sprite = player.cutInSprite;
        }
    }

    [SerializeField] AudioClip sound;
    public void OnFlash()
    {
        BattleAnimKantoku.audioSource.PlayOneShot(sound);
    }
}