using UnityEngine;

public class DarkDiffencer : JankenPlayer
{
    [SerializeField] int enhanceAmount = 3;

    public override void GoodEntangle()
    {
        if (diffenceEnhance > 0)
        {
            var enh = diffenceEnhance;
            diffenceEnhance = 0;
            BattleUtility.AttackSelf(this, (x) => enh);
        }
        base.BadEntangle();
    }

    public override void BadEntangle()
    {
        diffenceEnhance += enhanceAmount;
        base.GoodEntangle();
    }

    public override void ModifyHP(int amount)
    {
        int dim = 0;
        if (diffenceEnhance > 0)
        {
            dim = diffenceEnhance;
        }
        base.ModifyHP(amount);
        if (dim != 0)
        {
            diffenceEnhance = 0;
            BattleUtility.AttackEnemy(this, (x) => dim);
        }
    }
}