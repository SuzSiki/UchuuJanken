using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnhanceType
{
    none,
    Attack,
    Diffence
}

public abstract class JankenPlayer : MonoBehaviour
{
    [SerializeField] CharacterInformation _information;
    public CharacterInformation information{get{return _information;}}
    [SerializeField] Sprite _cutinSprite;
    public QuantumHand lastHand { get; protected set; }
    new public string name{get{return _information.name;}}
    int _attackEnhance;
    int _diffenceEnhance;
    public int attackEnhance
    {
        get { return _attackEnhance; }
        protected set
        {
            OnEnhance(EnhanceType.Attack,value);
            _attackEnhance = value;
        }
    }
    public int diffenceEnhance { get{return _diffenceEnhance;} protected set
    {
        OnEnhance(EnhanceType.Diffence,value);
        _diffenceEnhance = value;
    } }
    /// <summary>
    /// intは遷移後の値
    /// </summary>
    public event System.Action<EnhanceType, int> OnEnhance;
    public virtual void GoodEntangle()
    {
        OnAction(PlayerActionType.PositiveEntangle);
    }
    public virtual void BadEntangle()
    {
        OnAction(PlayerActionType.NegativeEntangle);
    }
    public event System.Action<PlayerActionType> OnAction;

    public Sprite cutInSprite { get { return _cutinSprite; } }

    void Start()
    {
        hp = JankenManager.instance.settingProfile.defaultHP;
        lastHand = (QuantumHand)Random.Range(1,5);
    }

    public bool SetHand(QuantumHand input)
    {
        if (!AreadySet)
        {
            AreadySet = true;
            lastHand = input;
            return true;
        }

        return false;
    }

    public virtual void OnWin()
    {
        if (attackEnhance > 0)
        {
            TriggerAction(PlayerActionType.AttackBuff);
        }
        OnAction(PlayerActionType.Attack);
        BattleUtility.AttackEnemy(this, (x) => { return x + attackEnhance; });

        attackEnhance = 0;
    }

    protected void TriggerAction(PlayerActionType type)
    {
        OnAction(type);
    }

    public virtual void ModifyHP(int amount)
    {
        if (amount < 0)
        {
            if (diffenceEnhance > 0)
            {
                //OnAction(PlayerActionType.DiffenceBuff);
                amount += diffenceEnhance;
                if (amount > 0)
                {
                    amount = 0;
                }
                diffenceEnhance = 0;
            }

            OnAction(PlayerActionType.Damage);
        }
        else
        {
            OnAction(PlayerActionType.Heal);
        }
        hp += amount;
    }


    public bool AreadySet { get; set; }
    public int hp { get; protected set; }
}

public enum QuantumHand
{
    none,
    喜,
    怒,
    楽,
    哀
}