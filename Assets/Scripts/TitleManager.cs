using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class TitleManager : Singleton<TitleManager>
{
    List<IInteraptor> interaptorQueue = new List<IInteraptor>();
    public event Action<TitleState> onTitleEvent;
    SessionProfile _profile;
    public SessionProfile profile { get { return _profile; } private set { _profile = value; } }
    CanvasGroup titleScreen;

    void Start()
    {
        titleScreen = GetComponent<CanvasGroup>();
        _profile = new SessionProfile();
        _profile.mode = GameMode.pvc;
        onTitleEvent += (x) => {
            Debug.Log(x);
         };
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if(x == SystemState.inJanken)
            {
                HideScreen();      
            }
        };


        StartCoroutine(TitleSequence());
    }

    void HideScreen()
    {
        var tw = titleScreen.DOFade(0,0.2f);
        tw.onComplete += () =>
        {
            gameObject.SetActive(false);
        };
        tw.Play();
    }

    IEnumerator TitleSequence()
    {
        yield return 4;
        onTitleEvent(TitleState.onEyeCatch);
        yield return StartCoroutine(HandleInteraptor());
        onTitleEvent(TitleState.onTitleScreen);
        yield return StartCoroutine(HandleInteraptor());
        onTitleEvent(TitleState.onCharacterSelect);
        yield return StartCoroutine(HandleInteraptor());

        JankenManager.instance.LoadSessionProfile(profile);
        onTitleEvent(TitleState.InJanken);
        yield return StartCoroutine(HandleInteraptor());
    }

    public void RegisterInterapt(IInteraptor interaptor)
    {
        interaptorQueue.Add(interaptor);
    }

    IEnumerator HandleInteraptor()
    {
        while (interaptorQueue.Count != 0)
        {
            yield return new WaitUntil(() => interaptorQueue[0].compleate);
            interaptorQueue.RemoveAt(0);
        }

    }


}