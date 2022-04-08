using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class RankingField : MonoBehaviour
{
    CanvasGroup uchiraCG;
    [SerializeField] PlayerTag nodePref;
    InstantPool<PlayerTag> nodePool;
    [SerializeField] int tagHeightDelta = 169;
    [SerializeField] TMP_Text nowRating;
    [SerializeField] TMP_Text rankingPosition;
    [SerializeField] ScrollRect rankingView;
    [SerializeField] Button backToTitieButton;
    [SerializeField] bool hideOnInit = true;
    //アップデート間隔。-1の時はアップデートせず最初だけ読み込む
    [SerializeField] float updateMin = -1;
    Dictionary<PlayerTag, TMP_Text> rankTextHash = new Dictionary<PlayerTag, TMP_Text>();

    void Start()
    {
        Initialize();
        if (backToTitieButton != null)
        {
            backToTitieButton.onClick.AddListener(() =>
            {
                var tw = ResultScreen.FadeIn();
                tw.onComplete += () =>
                {
                    var scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(scene.name);
                };
            });
            backToTitieButton.interactable = false;
        }


        uchiraCG = GetComponent<CanvasGroup>();
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.systemInitialize)
            {
                ReInitialize();
            }
        };
    }


    void Initialize()
    {
        nodePool = new InstantPool<PlayerTag>(rankingView.content);

    }

    void ReInitialize()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);
        nodePool.CreatePool(nodePref, 30, false);
        foreach (var node in nodePool.objects)
        {
            rankTextHash[node] = node.GetComponentInChildren<TMP_Text>();
        }

        if (hideOnInit)
        {
            var tw = uchiraCG.DOFade(0, 0);
            tw.onComplete += () =>
            {
                task.compleate = true;
            };

            tw.Play();
            gameObject.SetActive(false);
        }
        else
        {
            task.compleate = true;
        }
    }

    public IEnumerator LoadRanking()
    {
        if (PlayerManager.instance.rating.Value != 1000)
        {
            yield return StartCoroutine(RemoteRankingLoader.instance.SendScoreEnumerator());
        }

        if (RemoteRankingLoader.instance.sinceLastUpdate == null || (updateMin != -1 && updateMin * 60 < RemoteRankingLoader.instance.sinceLastUpdate))
        {
            yield return StartCoroutine(RemoteRankingLoader.instance.ReloadRanking());
        }
    }

    void ShowNodes()
    {
        nodePool.objects.ForEach(x => x.gameObject.SetActive(false));
        var sq = DOTween.Sequence();

        var size = rankingView.content.sizeDelta;
        size.y = tagHeightDelta * RemoteRankingLoader.instance.rankingData.Count;
        rankingView.content.sizeDelta = size;

        var playerdata = PlayerManager.instance.data;
        bool playerPut = false;

        var position = 0;
        foreach (var data in RemoteRankingLoader.instance.rankingData)
        {
            position++;

            var obj = nodePool.GetObj();
            obj.canvasGroup.DOFade(0, 0).Play();


            //ランキングは更新されていない場合があるが、プレイやーのデータは常に更新されるので
            //プレイヤーだけ別枠で作る
            if (!playerPut && data.data.rating < playerdata.rating)
            {
                rankTextHash[obj].text = position.ToString();
                obj.LoadPlayerData(playerdata);
                sq.Append(obj.canvasGroup.DOFade(1, 0.2f));
                playerPut = true;

                //ポジションは進める
                position++;

                //今ロードしていた人のための新しいノードを取得
                obj = nodePool.GetObj();
            }
            else if (data.ID == RemoteRankingLoader.instance.ObjectID)
            {
                //逆にすでにロードしてあった場合は飛ばす
                position--;

                //ちゃんとノードも返すの忘れなくてえらい
                obj.gameObject.SetActive(false);
                continue;
            }

            rankTextHash[obj].text = position.ToString();
            obj.LoadPlayerData(data.data);
            sq.Append(obj.canvasGroup.DOFade(1, 0.2f));
        }

        if (backToTitieButton != null)
        {
            backToTitieButton.interactable = true;
        }
        sq.Play();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        uchiraCG.DOFade(1, 0.4f).Play();
        StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        nowRating.text = PlayerManager.instance.rating.TextForDisplay;
        yield return StartCoroutine(LoadRanking());

        int? pp = RemoteRankingLoader.instance.playerPosition;
        var pStr = "";
        if (pp == null || pp == 0)
        {
            pStr = "---";
        }
        else
        {
            pStr = pp.ToString();
        }

        rankingPosition.text = pStr + "位";

        ShowNodes();
    }

    public void Hide()
    {
        var tw = uchiraCG.DOFade(0, 0.4f);
        tw.onComplete += () =>
        {
            gameObject.SetActive(false);
        };
        tw.Play();
    }
}