using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using DG.Tweening;

public class ResultScreen : Singleton<ResultScreen>
{
    IJankenAnimation showAnim;
    IJankenAnimation hideAnim;
    [SerializeField] Image gameScreenFader;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text ratingText;
    [SerializeField] TMP_Text winningCombo;
    [SerializeField] TMP_Text rateBonusText;
    StringBuilder rateBonusBuilder = new StringBuilder();
    [SerializeField] string ratingTextFormat = "RATING:{0} + {1}";
    Button[] buttons;
    [SerializeField]RankingField rankingField;
    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        var anim = GetComponent<Animator>();
        var jank = new AnimatorJankenateExchanger(anim);

        showAnim = jank.GetTriggerAction("Enter");
        hideAnim = jank.GetTriggerAction("Exit");

        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.viewInitialize)
            {
                Initialize();
            }
            else if (x == SystemState.jankenEnd)
            {
                SolveResult();
            }
            else if (x == SystemState.ResultScreen)
            {
                ShowResult();
            }
        };

        for (int i = 0; i < buttons.Length; i++)
        {
            var num = i;
            buttons[i].onClick.AddListener(()=>OnButtonClick(num));
            buttons[i].interactable = false;
        }
    }

    void Initialize()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);
        ViewRouter.instance.FadeOut().Play(BattleAnimKantoku.audioSource);
        FadeOut();

        hideAnim.onCompleate += () =>
        {
            task.compleate = true;
        };
        hideAnim.Play(BattleAnimKantoku.audioSource);
    }

    void SolveResult()
    {

        var now = PlayerManager.instance.rating;
        var newRate = ModifyRate((int)now.Value);
        PlayerManager.instance.SetRating(newRate);
    }

    //負けた時に減るレートの量(単位=%)
    [SerializeField] int loselosePercent = 10;
    [SerializeField] int defaultGainAmount = 40;

    int lastRate;
    int ModifyRate(int now)
    {
        //ここで設定するのが美しくないが無難。
        rateBonusText.text = "";
        winningCombo.text = "";



        rateBonusBuilder.Clear();
        lastRate = now;
        //この数には今回の分はまだ含まれていない。
        var combo = PlayerManager.instance.winningComboCount;
        int pos = JankenManager.sessionProfile.playerPosition;

        if (JankenManager.instance.nowSession.winner != pos)
        {
            var newRating = now - now * loselosePercent / 100;
            return (int)newRating;
        }

        now += defaultGainAmount;

        if (combo > 0)
        {
            rateBonusBuilder.Append("連勝ボーナス! ");
            var amount = 0;
            if (combo == 1)
            {
                amount = 10;
            }
            else if (combo == 2)
            {
                amount = 20;
            }
            else if (combo >= 3)
            {
                amount = 30;
            }
            rateBonusBuilder.Append("+" + amount + "!   \n");
            now += amount;

            winningCombo.text = (combo + 1) + "連勝中！";
        }
        var noDamage = JankenManager.instance.nowSession.GetPlayer(pos).hp == JankenManager.instance.settingProfile.defaultHP;

        if (noDamage)
        {
            rateBonusBuilder.Append("ノーダメボーナス!" + "+" + "50!");
            now += 50;
        }

        rateBonusText.text = rateBonusBuilder.ToString();

        return now;
    }

    void ShowResult()
    {

        int winner = JankenManager.instance.nowSession.winner.Value;
        int pos = JankenManager.sessionProfile.playerPosition;

        Debug.Log(winner);
        if (winner == pos)
        {
            resultText.text = "You Win!";
        }
        else
        {
            resultText.text = "負け。";
        }
        var now = (int)PlayerManager.instance.rating.Value;
        var tw = AnimateResult(lastRate, now);
        ViewRouter.instance.FadeIn().Play(BattleAnimKantoku.audioSource);
        showAnim.Play(BattleAnimKantoku.audioSource);
        tw.onComplete +=  ()=>
        {
            foreach(var button in buttons)
            {
                button.interactable = true;
            }
        };
        tw.Play();
    }

    Tween AnimateResult(int from, int to)
    {
        var sq = DOTween.Sequence();


        var delta = to - from;

        ratingText.text = string.Format(ratingTextFormat, from, delta);

        var tw = ratingText.DOFade(1, 0.3f);
        sq.Append(tw);

        var twe = DOTween.To(() => (int)from, (x) => from = x, to, 1);
        twe.onUpdate += () =>
        {
            delta = to - from;
            ratingText.text = string.Format(ratingTextFormat, from, delta);
        };

        twe.SetDelay(2f);

        sq.Append(twe);

        return sq;
    }

    public static Tween FadeIn()
    {
        var tw = instance.gameScreenFader.DOFade(1, 0.4f);
        tw.Play();
        return tw;
    }

    public static Tween FadeOut()
    {
        var tw = instance.gameScreenFader.DOFade(0, 0.4f);
        tw.Play();
        return tw;
    }



    void OnButtonClick(int id)
    {
        int winner = JankenManager.instance.nowSession.winner.Value;
        int pos = JankenManager.sessionProfile.playerPosition;
        Debug.Log("aa");

        if (id == 0)
        {
            FadeIn();
            var sessionProfile = JankenManager.sessionProfile;
            JankenManager.instance.LoadSessionProfile(sessionProfile);
        }
        else if(id == 1)
        {
            rankingField.Show();
        }
        else
        {
            Debug.LogWarning("too many buttons!");
        }

        foreach(var button in buttons)
        {
            button.interactable = false;
        }
    }

}