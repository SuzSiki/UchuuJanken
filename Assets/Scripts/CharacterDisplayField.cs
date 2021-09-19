using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class CharacterDisplayField:MonoBehaviour
{
    [SerializeField]Image tachie;
    TMP_Text[] textFields;
    AnimatorJankenateExchanger animator;

    IJankenAnimation closeAnimation;
    IJankenAnimation openAnimation;

    void Start()
    {
        animator = new AnimatorJankenateExchanger(GetComponent<Animator>());
        closeAnimation = animator.GetTriggerAction("Exit");
        openAnimation = animator.GetTriggerAction("Enter");
        textFields = GetComponentsInChildren<TMP_Text>();
    }

    public void Hide()
    {
        closeAnimation.Play(BattleAnimKantoku.audioSource);
    }

    public void LoadPlayer(JankenPlayer player)
    {
        SwapCharacter(player);
    }

    void SwapCharacter(JankenPlayer player)
    {
        closeAnimation.onCompleate +=() =>
        {
            SwapAndShow(player);
        };
        
        closeAnimation.Play(BattleAnimKantoku.audioSource);
    }
    
    void SwapAndShow(JankenPlayer player)
    {
        tachie.sprite = player.GetComponentInChildren<Image>().sprite;

        textFields[0].text = player.information.name;
        textFields[1].text = "PE: "+ player.information.positiveEntangle;
        textFields[2].text = "NE: "+player.information.negativeEntangle;
        textFields[3].text = player.information.introduction;


        openAnimation.Play(BattleAnimKantoku.audioSource);
    }
}