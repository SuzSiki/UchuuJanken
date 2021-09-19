using UnityEngine;
using UnityEngine.UI;
using naichilab;
using DG.Tweening;
using System.Linq;
using TMPro;
public class PlayerManager : Singleton<PlayerManager>
{
    string nameKey = "playerName";
    public new string name { get; private set; }
    public IScore rating { get { return _rating; } }
    public PlayerData data { get; private set; }
    TMP_InputField nameField;
    IScore _rating;
    public int winningComboCount { get; private set; }
    public event System.Action onUpdate;

    void Start()
    {
        onUpdate += () => { };

        nameField = GetComponentInChildren<TMP_InputField>();
        nameField.gameObject.SetActive(false);

        var playName = PlayerPrefs.GetString(nameKey, null);

        TitleManager.instance.onTitleEvent += (x) =>
        {
            if (x == TitleState.onTitleScreen)
            {
                if (playName == null || playName == "")
                {
                    RequireName();
                }
                else
                {
                    data.name = playName;
                    name = playName;
                    onUpdate();
                }

            }
        };



        var objid = RemoteRankingLoader.instance.ObjectID;
        if (objid != null)
        {
            var rawdata = PlayerPrefs.GetString(objid);
            data = JsonUtility.FromJson<PlayerData>(rawdata);
            if (data == null)
            {
                data = new PlayerData();
                _rating = new NumberScore(1000);
            }
            else
            {
                _rating = new NumberScore(data.rating);
            }
        }
        else
        {
            data = new PlayerData();
        }

        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.ResultScreen)
            {
                UpdateData();
            }
        };

    }

    void UpdateData()
    {
        var session = JankenManager.instance.nowSession;

        if (session.winner == JankenManager.sessionProfile.playerPosition)
        {
            winningComboCount++;
            if (data.maxWin > winningComboCount)
            {
                data.maxWin = winningComboCount;
            }
        }
        else
        {
            winningComboCount = 0;
        }

        data.rating = (int)rating.Value;
        var character = session.GetPlayer(JankenManager.sessionProfile.playerPosition);
        if (!data.pickCount.Any(x => x.id == character.information.ID))
        {
            var pickData = new PickData();
            pickData.id = character.information.ID;
            data.pickCount.Add(pickData);
        }
        data.pickCount.Find(x => x.id == character.information.ID).count++;

        var json = JsonUtility.ToJson(data);
        var id = RemoteRankingLoader.instance.ObjectID;
        PlayerPrefs.SetString(id, json);
        PlayerPrefs.Save();
        onUpdate();
    }

    void RequireName()
    {
        var task = new InteraptTask();
        TitleManager.instance.RegisterInterapt(task);
        nameField.gameObject.SetActive(true);
        var cg = nameField.GetComponent<CanvasGroup>();
        cg.DOFade(1, 0.4f).Play();
        var confButton = nameField.GetComponentInChildren<Button>();
        confButton.onClick.AddListener(() => OnSubmitName(task, cg));
        nameField.onSubmit.AddListener((x)=>OnSubmitName(task,cg));
    }

    [SerializeField] AudioClip accepted;
    [SerializeField] AudioClip denyed;
    void OnSubmitName(InteraptTask task, CanvasGroup cg)
    {
        if (nameField.text != "")
        {
            name = nameField.text;
            data.name = name;
            BattleAnimKantoku.audioSource.PlayOneShot(accepted);
            var tw = cg.DOFade(0, 0.3f);
            tw.onComplete += () =>
             {
                 nameField.gameObject.SetActive(false);
                 task.compleate = true;
             };

            tw.Play();
            PlayerPrefs.SetString(nameKey, name);
            onUpdate();
        }
        else
        {
            BattleAnimKantoku.audioSource.PlayOneShot(denyed);
        }
    }

    public void SetRating(int rating)
    {
        _rating = new NumberScore(rating);
    }
}

