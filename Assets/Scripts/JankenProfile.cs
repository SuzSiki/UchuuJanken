using UnityEngine;

[CreateAssetMenu]
public class JankenProfile:ScriptableObject
{
    [SerializeField] int _defaultHP;
    [SerializeField] float _flashDurationUnit;
    [SerializeField] int _defaultDamage; 
    [SerializeField] Color[] _handColors;
    public int defaultHP{get{return _defaultHP;}}

    //単位Duration。すべてのFlashアニメ及びAnimationはこれに丸め込まれる。
    public float unitDuration{get{return _flashDurationUnit;}}

    public int dafaultDamage{get{return _defaultDamage;}}
    public Color[] handColors{get{return _handColors;}}
}