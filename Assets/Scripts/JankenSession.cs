using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class JankenSession
{
    public bool finished { get { return IfFinished(); } }
    //直前の宇宙拳の結果
    public TurnResult lastResult { get { return _lastResult; } }
    TurnResult _lastResult;
    List<JankenPlayer> playerList = new List<JankenPlayer>();
    public int playerCount { get { return playerList.Count; } }
    public int roundCount{get;private set;}
    public int? winner{get;private set;}
    System.Action<JankenState> jankenCallback;

    List<System.Action> actionQueue = new List<System.Action>();

    public void KillSession()
    {
        foreach (var player in playerList)
        {
            JankenManager.instance.OnJankenEvent -= jankenCallback;
            MonoBehaviour.Destroy(player.gameObject);
        }
    }

    public JankenSession()
    {
        jankenCallback = OnJankenEvent;
        JankenManager.instance.OnJankenEvent += jankenCallback;
        _lastResult = new TurnResult();
        roundCount = 1;
    }

    public int AddPlayer(JankenPlayer player)
    {
        playerList.Add(player);

        return playerList.Count;
    }

    bool IfFinished()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if(playerCount!=2)
            {
                throw new System.NotImplementedException();
            }

            if (playerList[i].hp <= 0)
            {
                winner = (i-1)*-1;
                return true;
            }
        }
        winner = null;
        return false;
    }

    void OnJankenEvent(JankenState state)
    {
        
        if (state == JankenState.ChooseHandStart)
        {
            JankenManager.instance.StartCoroutine(WaitForPlayers());
        }
        else if (state == JankenState.JudgeStart)
        {
            JudgePlayers();
        }
        else if(state == JankenState.SolveResult)
        {
            var task = new InteraptTask();
            JankenManager.instance.RegisterInterapt(task);
            foreach(var queue in actionQueue)
            {
                queue();
            }
            actionQueue.Clear();
            task.compleate = true;
            roundCount++;
        }
        else if(state == JankenState.TurnStart)
        {
            var source = BattleAnimKantoku.audioSource;
            ViewRouter.instance.HideAll(source);
            foreach(var player in playerList)
            {
                player.AreadySet = false;
            }
        }
    }

    IEnumerator WaitForPlayers()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);
        foreach (var player in playerList)
        {
            yield return new WaitUntil(() => player.AreadySet);
        }

        task.compleate = true;
    }

    void JudgePlayers()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);
        var result = playerList[0].lastHand - playerList[1].lastHand;

        if (Mathf.Abs(result) == 3)
        {
            result *= -1;
            result /= 3;
        }

        if (result == 0)
        {
            _lastResult.result = JankenResult.aiko;
        }
        else if (Mathf.Abs(result) == 1)
        {
            _lastResult.result = JankenResult.settled;
            if (result == 1)
            {
                _lastResult.winner = 1;
                _lastResult.loser = 0;
            }
            else
            {
                _lastResult.winner = 0;
                _lastResult.loser = 1;
            }
            actionQueue.Add(playerList[_lastResult.winner].OnWin);
        }
        else
        {
            var sample = playerList[0].lastHand;
            if (sample == QuantumHand.怒 || sample == QuantumHand.哀)
            {
                actionQueue.Add(playerList[0].BadEntangle);
                actionQueue.Add(playerList[1].BadEntangle);
                _lastResult.result = JankenResult.NEntangle;
            }
            else
            {
                actionQueue.Add(playerList[0].GoodEntangle);
                actionQueue.Add(playerList[1].GoodEntangle);
                _lastResult.result = JankenResult.PEntangle;
            }
        }

        task.compleate = true;
    }

    public JankenPlayer GetPlayer(int id)
    {
        return playerList[id];
    }

    public int GetID(JankenPlayer player)
    {
        return playerList.IndexOf(player);
    }
}