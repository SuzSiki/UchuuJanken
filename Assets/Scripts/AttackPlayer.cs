using UnityEngine;
using System;
using System.Collections.Generic;

public class AttackPlayer : JankenPlayer
{
    [SerializeField] int _enhanceAmount;
    public int enhanceAmount { get { return _enhanceAmount; } }

    public override void GoodEntangle()
    {
        attackEnhance += enhanceAmount;
        base.GoodEntangle();
    }

    public override void BadEntangle()
    {
        attackEnhance = 0;
        base.BadEntangle();
    }

}