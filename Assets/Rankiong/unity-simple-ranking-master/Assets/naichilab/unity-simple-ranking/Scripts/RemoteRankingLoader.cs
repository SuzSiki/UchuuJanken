using UnityEngine;
using System.Collections;
using NCMB;
using NCMB.Extensions;
using naichilab;
using System.Linq;
using System.Collections.Generic;

public class RemoteRankingLoader : Singleton<RemoteRankingLoader>
{
    public float lastUpdate { get; private set; }
    public float? sinceLastUpdate
    {
        get
        {
            if (lastUpdate == 0)
            {
                return null;
            }
            else
            {
                return Time.realtimeSinceStartup - lastUpdate;
            }
        }
    }
    public const string COLUMN_SCORE = "score";
    public const string COLUMN_NAME = "name";
    public const string OBJECT_ID = "objectId";
    const string COLUMN_CHARACTER = "character";
    private NCMBObject _ncmbRecord;
    private string _objectid = null;

    public string OBJECT_COLUMN { get { return OBJECT_ID; } }

    public string ObjectID
    {
        get { return _objectid ?? (_objectid = PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null)); }
        set
        {
            if (_objectid == value)
                return;
            PlayerPrefs.SetString(BoardIdPlayerPrefsKey, _objectid = value);
        }

    }

    public string BoardIdPlayerPrefsKey
    {
        get { return string.Format("{0}_{1}_{2}", "board", boards.GetRankingInfo(defaultBoard).ClassName, OBJECT_COLUMN); }
    }
    [SerializeField] int defaultBoard = 0;
    [SerializeField] RankingBoards boards;
    List<RankingData> _rankingdata = new List<RankingData>();
    public List<RankingData> rankingData { get { return _rankingdata; } private set { rankingData = value; } }
    List<RankingData> rankingDataBuffer;
    RankingInfo _CurrentRanking;
    //RankingInfo _board { get { return boards.GetRankingInfo(defaultBoard); } }
    string playerName { get { return PlayerManager.instance.name; } }
    IScore _nowRating { get { return PlayerManager.instance.rating; } }
    public int? playerPosition { get { return FindPosition(); } }



    public IEnumerator SendScoreEnumerator(int boardNum = 0)
    {
        var _board = boards.GetRankingInfo(boardNum);
        //ハイスコア送信
        if (_ncmbRecord == null)
        {
            _ncmbRecord = new NCMBObject(_board.ClassName);
            _ncmbRecord.ObjectId = ObjectID;
        }

        _ncmbRecord[COLUMN_NAME] = playerName;
        _ncmbRecord[COLUMN_SCORE] = _nowRating.Value;
        var maxValue = PlayerManager.instance.data.pickCount.Max(x => x.count);
        var maxChara = PlayerManager.instance.data.pickCount.First(x => x.count == maxValue);
        _ncmbRecord[COLUMN_CHARACTER] = maxChara.id;
        NCMBException errorResult = null;

        yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error);

        if (errorResult != null)
        {
            //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生する（らしい）
            _ncmbRecord.ObjectId = null;
            yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error); //新規として送信
        }

        //ObjectIDを保存して次に備える
        ObjectID = _ncmbRecord.ObjectId;
    }



    //更新するなの
    public IEnumerator ReloadRanking()
    {
        yield return StartCoroutine(LoadRanking());
    }

    //渡されたプレイヤーのポジションを返す
    //nullなら自分のポジション
    int? FindPosition(PlayerData data = null)
    {
        if (data == null)
        {
            data = PlayerManager.instance.data;
        }

        int position = 0;
        for (int i = 0; i < rankingData.Count; i++)
        {
            position++;
            if (data.rating > rankingData[i].data.rating)
            {
                break;
            }
        }

        return position;
    }


    //ランキングを読み込み、プレイヤーの順位を特定
    private IEnumerator LoadRanking(int boardNum = 0)
    {
        lastUpdate = Time.realtimeSinceStartup;
        var _board = boards.GetRankingInfo(boardNum);

        var so = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
        so.Limit = 50;
        if (_board.Order == ScoreOrder.OrderByAscending)
        {
            so.OrderByAscending(COLUMN_SCORE);
        }
        else
        {
            so.OrderByDescending(COLUMN_SCORE);
        }

        yield return so.FindAsync();
        var position = 0;
        rankingData.Clear();
        foreach (var data in so.Result)
        {
            var rData = new RankingData();
            position++;

            rData.ID = data.ObjectId;
            rData.position = position;
            var pData = new PickData();
            EncodeToData(data, ref rData);
            rankingData.Add(rData);
        }
    }

    RankingData EncodeToData(NCMBObject rawData, ref RankingData data, int boardNum = 0)
    {
        var cID = rawData[COLUMN_CHARACTER].ToString();

        var pData = new PickData();

        pData.id = int.Parse(cID);
        pData.count = 1;

        data.data.pickCount.Add(pData);

        data.data.name = rawData[COLUMN_NAME].ToString();
        var rawScore = rawData[COLUMN_SCORE].ToString();

        //ちょっと注意。
        data.data.rating = (int)boards.GetRankingInfo(boardNum).BuildScore(rawScore).Value;

        return data;
    }

}

public class RankingData
{
    public string ID;
    public int position;
    public PlayerData data = new PlayerData();
}