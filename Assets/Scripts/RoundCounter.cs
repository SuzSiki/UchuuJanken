using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(Animator))]
public class RoundCounter:MonoBehaviour,IInteraptor
{
    public bool compleate{get;private set;}
    [SerializeField] TMP_Text targetText;
    IJankenAnimation anime;
    [SerializeField] AudioClip showSound;


    void Start()
    {
        anime = new AnimatorJankenateExchanger(GetComponent<Animator>()).GetTriggerAction("Flash");
        JankenManager.instance.OnJankenEvent += (x) => 
        {
            if(x == JankenState.TurnStart)
            {
                Trigger();
            }
        };
    }


    void Trigger()
    {
        compleate = false;
        targetText.text = JankenManager.instance.nowSession.roundCount.ToString();
        JankenManager.instance.RegisterInterapt(this);
        StartCoroutine(PlayAnime());
    }

    IEnumerator PlayAnime()
    {
        anime.Play(BattleAnimKantoku.audioSource);
        yield return new WaitUntil(()=>anime.compleate);
        compleate = true;
    }

}