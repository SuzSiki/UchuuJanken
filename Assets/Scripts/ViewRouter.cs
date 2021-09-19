using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//タイミングを管理する必要のある動きと、
//親のいない動きへのアクセスポイント
public class ViewRouter : Singleton<ViewRouter>
{
    [SerializeField] BanarConductor banarConductor;

    [SerializeField] AnimationConductor CountdownView;

    //作成済み
    [SerializeField] AnimationConductor faderConductor;

    //作成済み
    [SerializeField] CutinConductor[] cutinconductor;

    //Faderと同じもので十分
    [SerializeField] AnimationConductor handCutinConductor;

    List<PlayerAnimator> playerAnimatorList = new List<PlayerAnimator>();
    void Start()
    {
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.viewInitialize)
            {
                StartCoroutine(Initialize());
            }
        };
    }

    //ターンの終わりには消す。そういうこと。
    List<IJankenAnimation> closeQueue = new List<IJankenAnimation>();

    public void FlashRoundCounter()
    {

    }

    public void HideAll(AudioSource source)
    {
        foreach (var anim in closeQueue)
        {
            anim.Play(source);
        }

        closeQueue.Clear();
    }


    public IJankenAnimation GetIntro()
    {
        return CountdownView.GetAnimation(AnimationType.Enter);
    }

    IEnumerator Initialize()
    {
        playerAnimatorList.Clear();
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);
        var session = JankenManager.instance.nowSession;
        for (int i = 0; i < session.playerCount; i++)
        {
            var anime = session.GetPlayer(i).GetComponent<PlayerAnimator>();
            playerAnimatorList.Add(anime);
            yield return null;
        }

        task.compleate = true;
    }

    public IJankenAnimation GetPlayerCutIn(int id)
    {
        for (int i = 0; i < cutinconductor.Length; i++)
        {
            if (cutinconductor[i].targetPlayer == id)
            {
                var flashFade = FlashFade();
                var cutin = cutinconductor[i].GetAnimation(AnimationType.Flash);
                var anim = new CombinedJankenAnimation();
                anim.Append(flashFade);
                anim.Append(cutin);

                return anim;
            }
        }

        Debug.LogWarning("Cutin not set");
        return null;
    }
    public IJankenAnimation GetHandCutin(AnimationType type)
    {
        if (type == AnimationType.Enter)
        {
            closeQueue.Add(GetHandCutin(AnimationType.Exit));
        }

        return handCutinConductor.GetAnimation(type);
    }

    //アタックモーションをえるえる。
    //あるならチャージアクションもみとみと
    public IJankenAnimation GetPlayerAction(int id, PlayerActionType type)
    {
        var animator = playerAnimatorList[id].GetActionAnime(type);
        return animator;
    }


    public void UpdateHPbar(int id)
    {
        HPBar.GetBar(id).UpdateBar();
    }

    public IJankenAnimation FadeIn()
    {
        return faderConductor.GetAnimation(AnimationType.Enter);
    }

    public IJankenAnimation FadeOut()
    {
        return faderConductor.GetAnimation(AnimationType.Exit);
    }

    public IJankenAnimation FlashFade()
    {
        return faderConductor.GetAnimation(AnimationType.Flash);
    }

    public IJankenAnimation GetBanarEffect(JankenResult result, bool show)
    {
        banarConductor.SetBanar(result);

        if (show)
        {
            closeQueue.Add(GetBanarEffect(result, false));
            return banarConductor.GetAnimation(AnimationType.Enter);
        }
        else
        {
            return banarConductor.GetAnimation(AnimationType.Exit);
        }

    }

    class CombinedJankenAnimation : IJankenAnimation
    {
        List<IJankenAnimation> animes = new List<IJankenAnimation>();
        public float duration
        {
            get
            {
                return animes.Max(x => x.duration);
            }
        }

        public System.Action onCompleate{get;set;}

        public void SetUp(float timescale)
        {
            animes.ForEach(x => x.SetUp(timescale));
        }

        public bool compleate{get{return animes.All((x) => x.compleate == true);}}
        public void Append(IJankenAnimation anime)
        {
            animes.Add(anime);
        }

        public void Play(AudioSource source)
        {
            if(onCompleate!=null)
            {
                animes[0].onCompleate += onCompleate;
            }
            animes.ForEach(x => x.Play(source));
        }
    }
}
