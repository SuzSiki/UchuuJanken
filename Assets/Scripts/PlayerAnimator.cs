using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimator : MonoBehaviour
{
    AnimatorJankenateExchanger animator;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip diffenceSound;
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip positiveEntangle;
    [SerializeField] AudioClip negativeEntangle;
    [SerializeField] AudioClip positiveBuff;
    public event System.Action<PlayerActionType> OnAnimationContact;
    AudioSource source{get{return BattleAnimKantoku.audioSource;}}

    void Start()
    {
        var anim = GetComponent<Animator>();
        animator = new AnimatorJankenateExchanger(anim);
    }


    //なければNullJankenAnimeを返す
    //今はただtriggerをsetするばかり
    public IJankenAnimation GetActionAnime(PlayerActionType type)
    {
        var anime = animator.GetTriggerAction(type.ToString());

        return anime;
    }

    //アニメ上で実際に効果が出たときに呼ばれる
    public void OnContact(PlayerActionType action)
    {
        switch (action)
        {
            case PlayerActionType.Attack:
                source.PlayOneShot(attackSound);
                break;

            case PlayerActionType.Damage:
                source.PlayOneShot(damageSound);
                break;
            case PlayerActionType.NegativeEntangle:
                source.PlayOneShot(negativeEntangle);
                break;
            case PlayerActionType.PositiveEntangle:
                source.PlayOneShot(positiveEntangle);
                break;
            case PlayerActionType.AttackBuff:
                source.PlayOneShot(positiveBuff);
                break;
            case PlayerActionType.DiffenceBuff:
                source.PlayOneShot(diffenceSound);
                break;
        }

        if(OnAnimationContact != null)
        {
            OnAnimationContact(action);
        }
    }
}