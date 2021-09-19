public enum JankenState
{
    none,
    TurnStart,
    ChooseHandStart,  //プレイヤーたちが手札を選ぶの刻P
    JudgeStart,       //結果の算出までを行う刻
    SolveResult,      //結果から計算をするの刻
    PrepareAnimation, //Animationに必要なものを集め、設置するの刻
    AnimationStart,   //Animationが再生されるの刻
    Result
}

public enum SystemState
{
    none,
    systemInitialize,
    playerRegister,
    viewInitialize,
    inJanken,
    jankenEnd,
    ResultScreen
}