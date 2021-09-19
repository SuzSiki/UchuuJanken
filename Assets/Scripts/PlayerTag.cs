using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;
[RequireComponent(typeof(CanvasGroup))]
public class PlayerTag : MonoBehaviour
{
    [SerializeField] Image CharacterThumbNail;
    [SerializeField] TMP_Text nameField;
    [SerializeField] TMP_Text rateField;
    public CanvasGroup canvasGroup { get; private set; }

    //画面に最初から設置してあるタイプ。
    // Updateで自プレイヤーの情報を自動で読み込む。
    public bool forSelfPlayer = true;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (forSelfPlayer)
        {
            PlayerManager.instance.onUpdate += () =>
            {
                LoadSelfPlayerData();
            };
        }
    }

    void LoadSelfPlayerData()
    {
        var data = PlayerManager.instance.data;
        LoadPlayerData(data);
    }

    public void LoadPlayerData(PlayerData data)
    {
        nameField.text = data.name;
        rateField.text = "Rating:" + data.rating;

        JankenPlayer chara = null;
        if (data.pickCount.Count == 0)
        {
            chara = PlayerSetter.instance.players[0];
        }
        else
        {
            var cCount = data.pickCount.Max(x => x.count);
            var id = data.pickCount.Find(x => x.count == cCount).id;
            chara = PlayerSetter.instance.players.First(x => x.information.ID == id);
        }
        var sprite = chara.cutInSprite;

        CharacterThumbNail.sprite = sprite;
    }
}