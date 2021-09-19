using UnityEngine;

public class DiffencePlayer:JankenPlayer
{
    [SerializeField] int _enhanceAmount = 3;
    public int enhanceAmount{get{return _enhanceAmount;}}

    public override void GoodEntangle()
    {
        diffenceEnhance+= enhanceAmount;
        base.GoodEntangle();
    }

    public override void BadEntangle()
    {
        diffenceEnhance = 0;
        base.BadEntangle();
    }
}