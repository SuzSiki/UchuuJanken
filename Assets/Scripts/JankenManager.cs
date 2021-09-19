using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class JankenManager : Singleton<JankenManager>
{
    public event Action<JankenState> OnJankenEvent;
    public event Action<SystemState> OnSystemEvent;
    public static SessionProfile sessionProfile;
    [SerializeField] JankenProfile _settingProfile;

    public JankenSession nowSession { get; private set; }
    public JankenProfile settingProfile { get { return _settingProfile; } }
    SystemState nowSystem;
    List<IInteraptor> tasks = new List<IInteraptor>();
    ConfarmButton button;
    

    void Start()
    {
        button = GetComponentInChildren<ConfarmButton>();
        OnSystemEvent += (x) =>
        {
            nowSystem = x;
            Debug.Log(x);
        };

        OnJankenEvent += (x) =>
        {
            Debug.Log(x);
        };

    }

    public void LoadSessionProfile(SessionProfile profile)
    {
        if (nowSystem == SystemState.none || nowSystem == SystemState.ResultScreen)
        {
            if(nowSession!= null)
            {
                nowSession.KillSession();
            };

            sessionProfile = profile;
            StartCoroutine(SystemRoutine(profile));
        }
    }

    void InitSession()
    {
        nowSession = new JankenSession();
    }

    public void RegisterInterapt(IInteraptor task)
    {
        tasks.Add(task);
    }

    public void RegisterPlayer(JankenPlayer player)
    {
        if (nowSystem == SystemState.playerRegister)
        {
            var count = nowSession.AddPlayer(player);
        }
    }

    IEnumerator SystemRoutine(SessionProfile profile)
    {
        yield return 3;


        InitSession();

        OnSystemEvent(SystemState.systemInitialize);
        if (tasks.Count != 0)
        {
            yield return StartCoroutine(HandleInteraptors());
        }

        OnSystemEvent(SystemState.playerRegister);

        if (profile.mode == GameMode.pvc)
        {
            PlayerSetter.instance.SetHumanPlayer(profile.playerPosition, profile.player0);
            PlayerSetter.instance.SetAIPlayer(profile.playerPosition - 1 * (-1), profile.player1);
        }
        else if (profile.mode == GameMode.cvc)
        {
            PlayerSetter.instance.SetAIPlayer(0, profile.player0);
            PlayerSetter.instance.SetAIPlayer(0, profile.player1);
        }

        yield return new WaitUntil(() => nowSession.playerCount == 2);

        if (tasks.Count != 0)
        {
            yield return StartCoroutine(HandleInteraptors());
        }

        OnSystemEvent(SystemState.viewInitialize);

        if (tasks.Count != 0)
        {
            yield return StartCoroutine(HandleInteraptors());
        }

        OnSystemEvent(SystemState.inJanken);

        if (tasks.Count != 0)
        {
            yield return StartCoroutine(HandleInteraptors());
        }

        yield return StartCoroutine(JankenCoroutine());
        OnSystemEvent(SystemState.jankenEnd);

        if (tasks.Count != 0)
        {
            yield return StartCoroutine(HandleInteraptors());
        }

        OnSystemEvent(SystemState.ResultScreen);
    }

    IEnumerator JankenCoroutine()
    {
        var tmp = Enum.GetValues(typeof(JankenState)) as JankenState[];
        var states = tmp.ToList();
        states.RemoveAt(0);

        while (!nowSession.finished)
        {
            foreach (var state in states)
            {
                OnJankenEvent(state);

                if (state == JankenState.Result)
                {
                    button.PressButtonToContinue();
                }

                while (tasks.Count != 0)
                {
                    yield return StartCoroutine(HandleInteraptors());
                }
            }
        }

    }

    IEnumerator HandleInteraptors()
    {
        while (tasks.Count != 0)
        {
            yield return new WaitUntil(() => tasks[0].compleate);
            tasks.RemoveAt(0);
        }
    }

}