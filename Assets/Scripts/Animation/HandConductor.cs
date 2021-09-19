using UnityEngine;
using TMPro;

public class HandConductor : AnimationConductor
{
    TMP_Text[] texts;
    AnimatorJankenateExchanger jankenator;
    Color[] defaultColors{get{return JankenManager.instance.settingProfile.handColors;}}

    void Start()
    {
        var animator = GetComponent<Animator>();
        texts = GetComponentsInChildren<TMP_Text>();
        jankenator = new AnimatorJankenateExchanger(animator);
    }

    public override IJankenAnimation GetAnimation(AnimationType type)
    {
        var anime = jankenator.GetTriggerAction(type.ToString());
        return anime;
    }

    public void OnPlayAnimation()
    {
        var session = JankenManager.instance.nowSession;

        var res0 = session.GetPlayer(0).lastHand;
        var res1 = session.GetPlayer(1).lastHand;
        var tex1 = GetHandString(res0);
        var tex2 = GetHandString(res1);

        texts[0].text = tex1;
        texts[1].text = GetOperator();
        texts[2].text = tex2;
    }

    string GetOperator()
    {
        var result = JankenManager.instance.nowSession.lastResult;
        if (result.result == JankenResult.settled)
        {
            if (result.winner == 0)
            {
                return "→";
            }
            else
            {
                return "←";
            }
        }
        else if (result.result == JankenResult.NEntangle)
        {
            return "ε";
        }
        else if (result.result == JankenResult.PEntangle)
        {
            return "Ε";
        }
        else
        {
            return "△";
        }
    }
    string GetHandString(QuantumHand hand)
    {
        string baString = "<color=#";
        if (hand == QuantumHand.喜)
        {
            baString += ColorUtility.ToHtmlStringRGBA(defaultColors[0]) + ">";
        }
        else if (hand == QuantumHand.怒)
        {
            baString += ColorUtility.ToHtmlStringRGBA(defaultColors[1]) + ">";
        }
        else if (hand == QuantumHand.哀)
        {
            baString += ColorUtility.ToHtmlStringRGBA(defaultColors[2]) + ">";
        }
        else if (hand == QuantumHand.楽)
        {
            baString += ColorUtility.ToHtmlStringRGBA(defaultColors[3]) + ">";
        }

        return baString + hand.ToString() + "</color>";

    }
}