using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;
using System.Collections.Generic;

//プレイヤーのGUI的挙動。
[RequireComponent(typeof(Animator))]
public class PlayerReactiveView : MonoBehaviour
{
    [SerializeField, Range(0, 1)] int targetPlayer;
    JankenPlayer target;
    [SerializeField] PlayerActionProfile damageProfile;
    [SerializeField] PlayerActionProfile diffenceBuffProfile;
    [SerializeField] PlayerActionProfile attackBuffProfile;
    [SerializeField] TMPro.TMP_Text thinkingText;
    PlayerAnimator playerAnimator;
    CharacterBuffer buffer;
    Animator animator;
    List<Action> enhanceQueue = new List<Action>();

    void Start()
    {
        Initialize();
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.viewInitialize)
            {
                ReInitialize();
            }
        };
    }

    void Initialize()
    {
        animator = GetComponent<Animator>();
    }

    void ReInitialize()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);

        target = JankenManager.instance.nowSession.GetPlayer(targetPlayer);
        playerAnimator = target.GetComponent<PlayerAnimator>();
        thinkingText.DOFade(0, 0).Play();
        JankenManager.instance.OnJankenEvent += Reaction;
        buffer = new CharacterBuffer();
        buffer.hp = JankenManager.instance.settingProfile.defaultHP;
        buffer.attackEnhance = 0;
        buffer.diffenceEnhance = 0;


        target.OnEnhance += (x, y) =>
          {
              enhanceQueue.Add(() => OnEnhance(x, y));
          };

        InitialLoad(target);

        playerAnimator.OnAnimationContact += OnPlayerAction;

        task.compleate = true;

    }

    void Reaction(JankenState state)
    {
        if (state == JankenState.ChooseHandStart)
        {
            thinkingText.DOFade(1, 1).Play();
            thinkingText.text = "考え中";
        }
        else if (state == JankenState.JudgeStart)
        {
            thinkingText.DOFade(0, 1).Play();
        }
    }


    void OnPlayerAction(PlayerActionType type)
    {
        if (type == PlayerActionType.Damage)
        {
            var damage = buffer.hp - target.hp;
            damageProfile.SetString(damage.ToString());
            buffer.hp = target.hp;
            animator.runtimeAnimatorController = damageProfile.overrideController;

            animator.SetTrigger("Flash");
        }


        //キャラの何らかの行動とともに実行される
        //ターンと行動とenhanceは1:1:1対応なので問題ない
        if (enhanceQueue.Count != 0)
        {
            foreach (var enhance in enhanceQueue)
            {
                enhance();
            }
            enhanceQueue.Clear();
        }
    }

    void InitialLoad(JankenPlayer player)
    {
        OnEnhance(EnhanceType.Attack,player.attackEnhance);
        OnEnhance(EnhanceType.Diffence,player.diffenceEnhance);
    }

    protected virtual void OnEnhance(EnhanceType type, int amount)
    {
        AnimatorOverrideController overrider = null;
        if (type == EnhanceType.Attack)
        {
            attackBuffProfile.SetString(amount.ToString());
            overrider = attackBuffProfile.overrideController;

            buffer.attackEnhance = amount;
        }
        else if (type == EnhanceType.Diffence)
        {
            diffenceBuffProfile.SetString(amount.ToString());
            overrider = diffenceBuffProfile.overrideController;

            buffer.attackEnhance = amount;
        }
        else
        {
            Debug.LogWarning("Unknown enhance");
        }

        animator.runtimeAnimatorController = overrider;
        animator.SetTrigger("Flash");
    }


    IEnumerator WaitForPlayerSelect()
    {
        yield return new WaitUntil(() => target.AreadySet);
        thinkingText.text = "確定！";
    }


    [Serializable]
    class PlayerActionProfile
    {
        [SerializeField] AnimatorOverrideController _controller;
        [SerializeField] TMPro.TMP_Text targetText;
        public AnimatorOverrideController overrideController { get { return _controller; } }
        public void SetString(string text)
        {
            targetText.text = text;
        }
    }

    struct CharacterBuffer
    {
        public int hp;
        public int attackEnhance;
        public int diffenceEnhance;
    }

}