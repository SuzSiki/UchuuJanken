using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.Serialization;

namespace naichilab
{
    /// <summary>
    /// ランキング読み込みクラス
    /// </summary>
    public class RankingLoader : Singleton<RankingLoader>
    {
        /// <summary>
        /// リーダーボード一覧
        /// </summary>
        [SerializeField] public RankingBoards RankingBoards;

        /// <summary>
        /// 表示対象のボード
        /// </summary>
        [NonSerialized] public RankingInfo CurrentRanking;

        /// <summary>
        /// 直前のスコア
        /// </summary>
        [NonSerialized] public IScore nowRating;
        [SerializeField] int defaultBoard = 0;

        void Start()
        {
            //Class名重複をチェック
            RankingBoards.CheckDuplicateClassName();
            CurrentRanking = RankingBoards.GetRankingInfo(defaultBoard);
        }


        /// <summary>
        /// 時間型スコアの送信とランキング表示を行います
        /// </summary>
        /// <param name="time"></param>
        /// <param name="boardId"></param>
        public void SendScoreAndShowRanking(TimeSpan time, int boardId = 0)
        {
            var board = RankingBoards.GetRankingInfo(boardId);
            var sc = new TimeScore(time, board.CustomFormat);
            SendScoreAndShowRanking(sc, board);
        }

        public void SendScore(double score, int boardId = 0)
        {
            var board = RankingBoards.GetRankingInfo(boardId);
            var sc = new NumberScore(score, board.CustomFormat);
            SendScoreAndShowRanking(sc, board);
        }

        /// <summary>
        /// 数値型スコアの送信とランキング表示を行います
        /// </summary>
        /// <param name="score"></param>
        /// <param name="boardId"></param>
        public void SendScoreAndShowRanking(double score, int boardId = 0)
        {
            var board = RankingBoards.GetRankingInfo(boardId);
            var sc = new NumberScore(score, board.CustomFormat);
            SendScoreAndShowRanking(sc, board);
        }

        private void SendScoreAndShowRanking(IScore score, RankingInfo board)
        {
            if (board.Type != score.Type)
            {
                throw new ArgumentException("スコアの型が違います。");
            }

            CurrentRanking = board;
            nowRating = score;
            SceneManager.LoadScene("Ranking", LoadSceneMode.Additive);
        }
    }
}