using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EyeCatchController : MonoBehaviour
{
    Image worldFader;
    void Start()
    {
        worldFader = GetComponentInChildren<Image>();
        TitleManager.instance.onTitleEvent += (x) =>
        {
            if (x == TitleState.onEyeCatch)
            {
                ShowEyeCatch();
            }
        };
    }

    void ShowEyeCatch()
    {
        var task = new InteraptTask();
        TitleManager.instance.RegisterInterapt(task);
        var animator = GetComponentInChildren<Animator>();
        var janimator = new AnimatorJankenateExchanger(animator);
        var anime = janimator.GetTriggerAction("Enter");
        anime.onCompleate += () =>
        {
            task.compleate = true;
            var tw = worldFader.DOFade(0, 0.5f).SetDelay(0.5f);
            tw.onComplete += () =>
            {
                gameObject.SetActive(false);
            };
            tw.Play();
        };

        anime.Play(BattleAnimKantoku.audioSource);
    }
}
