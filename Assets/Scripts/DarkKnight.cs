using UnityEngine;

public class DarkKnight:JankenPlayer
{
    [SerializeField] int _enhanceAmount = 5;
    public int enhanceAmount{get{return _enhanceAmount;}}

    public override void GoodEntangle()
    {
        base.BadEntangle();
        BattleUtility.AttackSelf(this,(x) => {return attackEnhance;});
    }

    public override void BadEntangle()
    {
        attackEnhance += enhanceAmount;
        base.GoodEntangle();
    }

    public override void OnWin()
    {
        var aEnh = attackEnhance;
        base.OnWin();
        attackEnhance = aEnh;
    }

}