using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//特定のスロットにJankenAnimを設定したら
//プログラムされた通りの順番とタイミングで再生してくれる監督
[RequireComponent(typeof(AudioSource))]
public class BattleAnimKantoku : MonoBehaviour
{
    new AudioSource audio;
    public static AudioSource audioSource { get; private set; }
    [SerializeField] float janDuration = 0.7f;
    struct PlayerActionData
    {
        public int playerID;
        public PlayerActionType type;
    }
    //前回プレイヤーがとった行動を保存。
    List<PlayerActionData> playerActionQueue = new List<PlayerActionData>();

    List<IJankenAnimation> animationQueue = new List<IJankenAnimation>();

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        audioSource = audio;
    }

    void Start()
    {
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.viewInitialize)
            {
                InitView();
            }
        };

        JankenManager.instance.OnJankenEvent += (x) =>
        {
            if (x == JankenState.PrepareAnimation)
            {
                PrepareForJanken();
            }
            else if (x == JankenState.AnimationStart)
            {
                StartCoroutine(Animate());
            }
        };
    }

    void InitView()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);

        var num = JankenManager.instance.nowSession.playerCount;
        for (int i = 0; i < num; i++)
        {
            var player = JankenManager.instance.nowSession.GetPlayer(i);
            var tmp = i;
            player.OnAction += (x) =>
            {
                var data = new PlayerActionData();
                data.playerID = tmp;
                data.type = x;
                playerActionQueue.Add(data);
            };
        }

        task.compleate = true;
    }

    void PrepareForJanken()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);

        var result = JankenManager.instance.nowSession.lastResult;


        SetIntro(animationQueue);
        if (result.result == JankenResult.settled)
        {
            SetCutin(animationQueue);
            SetPon(animationQueue);
        }
        else
        {
            SetPon(animationQueue);
            SetCutin(animationQueue);
        }

        foreach (var data in playerActionQueue)
        {
            SetPlayerAction(data.playerID, data.type, animationQueue);
        }
        playerActionQueue.Clear();

        task.compleate = true;
    }

    IEnumerator Animate()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);

        while (animationQueue.Count > 0)
        {
            var anime = animationQueue[0];
            anime.Play(audio);
            yield return new WaitUntil(() => anime.compleate);
            animationQueue.RemoveAt(0);
        }

        task.compleate = true;
    }




    void SetIntro(List<IJankenAnimation> list)
    {
        var anim = ViewRouter.instance.GetIntro();
        anim.SetUp(1 / janDuration);
        list.Add(anim);
    }

    void SetPon(List<IJankenAnimation> list)
    {
        var pon = ViewRouter.instance.GetHandCutin(AnimationType.Enter);
        pon.SetUp(2 / janDuration);
        list.Add(pon);
    }

    void SetCutin(List<IJankenAnimation> list)
    {
        var result = JankenManager.instance.nowSession.lastResult;
        var turnResult = result.result;
        IJankenAnimation cutin = null;
        if (turnResult == JankenResult.settled)
        {
            var winner = result.winner;
            if (JankenManager.instance.nowSession.finished)
            {
                cutin = ViewRouter.instance.GetPlayerCutIn(winner);
                cutin.SetUp(1 / janDuration);
            }
        }
        else
        {
            cutin = ViewRouter.instance.GetBanarEffect(turnResult, true);
            cutin.SetUp(2 / janDuration);
        }

        if (cutin != null)
        {
            list.Add(cutin);
        }
    }

    void SetPlayerAction(int target, PlayerActionType type, List<IJankenAnimation> list)
    {
        IJankenAnimation animation;
        animation = ViewRouter.instance.GetPlayerAction(target, type);
        animation.onCompleate += () =>
        {
            ViewRouter.instance.UpdateHPbar(target);
        };

        animation.SetUp(2 / janDuration);
        list.Add(animation);
    }

}