using UnityEngine;

public struct TurnResult
{
    public JankenResult result{get;set;}
    public int winner;
    public int loser;
}

public enum JankenResult
{
    none,
    settled,  //勝ち負けがついたとき
    PEntangle, //正のねじれ
    NEntangle,  //負のねじれ
    aiko        //aiko
}